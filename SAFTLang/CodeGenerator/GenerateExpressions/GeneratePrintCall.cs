using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GeneratePrintCall(CallExpr call)
    {
        Expr expr = call.Arguments[0];

        LangType type = _analyzer.GetExpressionType(expr);

        string value = GenerateExpression(expr);

        return type.Kind switch
        {
            LangTypeKind.Bool => $"printf(\"%s\\n\", {value} ? \"true\" : \"false\")",
            
            LangTypeKind.String => $"printf(\"%s\\n\", {GenerateExpression(expr)})",
            
            LangTypeKind.Char => $"printf(\"%c\\n\", {value})",
            
            LangTypeKind.Int => $"printf(\"%d\\n\", {GenerateExpression(expr)})",
            
            _ => $"printf(\"%d\\n\", {GenerateExpression(expr)})"
        };
    }

}