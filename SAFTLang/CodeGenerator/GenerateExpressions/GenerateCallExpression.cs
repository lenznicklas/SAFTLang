using SAFTLang.AST.Expressions;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateCallExpression(CallExpr call)
    {
        string callee;

        if (call.Callee is IdentifierExpr ident &&
            ident.Name == "print")
        {
            return GeneratePrintCall(call);
        }
        else
        {
            callee = GenerateExpression(call.Callee);
        }
        
        string arguments = string.Join(
            ", ",
            call.Arguments.Select(GenerateExpression)
        );
        
        return $"{callee}({arguments})";
    }

}