using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private string GenerateFunctionSignature(FunctionStatement function)
    {
        string returnType = GenerateType(function.ReturnType);

        string name = GenerateIdentifier(function.Name);

        string parameters;

        if (function.Parameters.Count == 0)
        {
            parameters = "void";
        }
        else
        {
            parameters = string.Join(
                ", ",
                function.Parameters.Select(parameter =>
                    $"{GenerateType(parameter.Type)} " +
                    $"{GenerateIdentifier(parameter.Name)}"
                )
            );
        }

        return $"{returnType} {name}({parameters})";
    }

    private void GenerateFunctionPrototype(
        StringBuilder output,
        FunctionStatement function)
    {
        output.AppendLine($"{GenerateFunctionSignature(function)};");
    }

    private void GenerateFunctionDefinition(
        StringBuilder output,
        FunctionStatement function)
    {
        output.AppendLine(GenerateFunctionSignature(function));
        
        GenerateBlockStatement(output, function.Body, 0);

        output.AppendLine();
    }
}