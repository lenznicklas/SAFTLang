using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private LangType AnalyzeIdentifier(IdentifierExpr ident)
    {
        VariableSymbol? symbol = _state.ResolveVariable(ident.Name, ident.Span);
        if (symbol is null)
        {
            return LangType.Error;
        }
        
        return symbol.Type;
    }

}