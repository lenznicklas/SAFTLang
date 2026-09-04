using SAFTLang.AST.Expressions;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateCallExpression(CallExpr call)
    {

        if (call.Callee is IdentifierExpr ident)
        {
            if (ident.Name == "print")
            {
                return GeneratePrintCall(call);
            }

            if (ident.Name == "len")
            {
                return GenerateLenCall(call);
            }
        }
        
        
        string callee = GenerateExpression(call.Callee);
        
        
        string arguments = string.Join(
            ", ",
            call.Arguments.Select(GenerateExpression)
        );
        
        return $"{callee}({arguments})";
    }

}