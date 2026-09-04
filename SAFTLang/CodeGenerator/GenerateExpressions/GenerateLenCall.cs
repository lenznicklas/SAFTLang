using SAFTLang.AST.Expressions;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateLenCall(CallExpr call)
    {
        string array =
            GenerateExpression(
                call.Arguments[0]
            );

        return
            $"((int)saft_array_len({array}))";
    }
}