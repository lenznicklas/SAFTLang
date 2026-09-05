using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateLenCall(CallExpr call)
    {
        Expr argument = call.Arguments[0];

        LangType type = _analyzer.GetExpressionType(argument);
        
        string value = GenerateExpression(argument);

        return type.Kind switch
        {
            LangTypeKind.Array =>
                $"((int)saft_array_len({value}))",

            LangTypeKind.String =>
                $"((int)strlen({value}))",

            _ => throw new InvalidOperationException(
                $"Internal compiler error: cannot generate len for {type}"
            )
        };


    }
}