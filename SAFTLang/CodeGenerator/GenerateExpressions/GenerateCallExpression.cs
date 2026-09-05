using SAFTLang.AST.Expressions;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private string GenerateCallExpression(CallExpr call)
    {

        if (call.Callee is not IdentifierExpr ident)
        {
            throw new InvalidOperationException("Internal compiler error: call target is no identifier");
        }

        if (ident.Name == "print")
        {
            return GeneratePrintCall(call);
        }

        if (ident.Name == "len")
        {
            return GenerateLenCall(call);
        }

        if (ident.Name == "append")
        {
            return GenerateAppendCall(call);
        }
        
        
        string callee = CCodeUtils.GenerateFunctionIdentifier(ident.Name);
        
        
        string arguments = string.Join(
            ", ",
            call.Arguments.Select(GenerateExpression)
        );
        
        return $"{callee}({arguments})";
    }

}