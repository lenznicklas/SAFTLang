using SAFTLang.AST.Expressions;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GeneratePrintCall(CallExpr call)
    {
        Expr expr = call.Arguments[0];

        return expr switch
        {
            BoolExpr => $"printf(\"%s\\n\", {GenerateExpression(expr)} ? \"true\" : \"false\")",
            
            StringExpr => $"printf(\"%s\\n\", {GenerateExpression(expr)})",
            
            IntegerExpr => $"printf(\"%d\\n\", {GenerateExpression(expr)})",
            
            _ => $"printf(\"%d\\n\", {GenerateExpression(expr)})"
        };
    }

}