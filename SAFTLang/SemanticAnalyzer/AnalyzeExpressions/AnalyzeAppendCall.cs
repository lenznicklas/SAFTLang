using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeAppendCall(CallExpr call)
    {
        if (call.Arguments.Count != 2)
        {
            _diagnostics.ReportError(call.Span, "'append' expects exactly 2 arguments");
            return LangType.Error;
        }
        
        Expr arrayExpr = call.Arguments[0];
        Expr valueExpr = call.Arguments[1];
        
        LangType arrayType = AnalyzeExpression(arrayExpr);

        if (arrayType == LangType.Error)
        {
            return LangType.Error;
        }

        if (arrayType.Kind != LangTypeKind.Array)
        {
            _diagnostics.ReportError(call.Arguments[0].Span, "First argument must be an array");
            return LangType.Error;
        }

        IdentifierExpr? root = GetAppendRootIdentifier(arrayExpr);

        if (root is not null)
        {
            VariableSymbol? symbol = _state.ResolveVariable(root.Name, root.Span);

            if (symbol?.IsConst == true)
            {
                _diagnostics.ReportError(arrayExpr.Span, $"Cannot modify const array '{root.Name}'");
            }
        }

        LangType elementType = arrayType.ElementType;

        LangType valueType = AnalyzeExpression(valueExpr, elementType);

        if (valueType == LangType.Error)
        {
            return LangType.Error;
        }

        if (valueType != elementType)
        {
            _diagnostics.ReportError(
                valueExpr.Span,
                $"'append' expects {elementType} but got {valueType}"
            );
            return LangType.Error;
        }

        return LangType.Void;
    }

    private static IdentifierExpr? GetAppendRootIdentifier(Expr expr)
    {
        return expr switch
        {
            IdentifierExpr identifierExpr =>
                identifierExpr,

            IndexExpr indexExpr =>
                GetAppendRootIdentifier(indexExpr.Target),

            _ => null
        };
    }
}