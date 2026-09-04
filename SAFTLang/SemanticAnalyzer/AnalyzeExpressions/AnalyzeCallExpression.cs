using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeCall(CallExpr call)
    {
        if (call.Callee is not IdentifierExpr identifier)
        {
            _diagnostics.ReportError(
                call.Callee.Span,
                "Expression is not callable"
            );

            return LangType.Error;
        }

        if (identifier.Name == "print")
        {
            if (call.Arguments.Count != 1)
            {
                _diagnostics.ReportError(
                    call.Span,
                    $"Function 'print' expects exactly one argument"
                );

                return LangType.Error;
            }

            AnalyzeExpression(call.Arguments[0]);
            
            return LangType.Void;
        }

        if (identifier.Name == "len")
        {
            return AnalyzeLenCall(call);
        }

        FunctionSymbol? function = _state.ResolveFunction(identifier.Name, identifier.Span);

        if (function is null)
        {
            return LangType.Error;
        }
        
        if (call.Arguments.Count != function.ParameterTypes.Count)
        {
            _diagnostics.ReportError(
                call.Span,
                $"Function '{function.Name}' expects  argument"
            );

            return LangType.Error;
        }

        for (int i = 0; i < call.Arguments.Count; i++)
        {
            Expr argument = call.Arguments[i];

            LangType expectedType = function.ParameterTypes[i];
            
            LangType actualType = AnalyzeExpression(argument, expectedType);

            if (actualType != LangType.Error &&
                actualType != expectedType)
            {
                _diagnostics.ReportError(
                    argument.Span,
                    $"Argument {i + 1} of function '{function.Name}' expects {expectedType} but got {actualType}"
                );
            }
        }
        
        return function.ReturnType;
    }

}