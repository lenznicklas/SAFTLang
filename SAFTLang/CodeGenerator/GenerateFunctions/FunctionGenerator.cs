using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.CodeGenerator.GenerateStatements;
using SAFTLang.CodeGenerator.GenerateTypes;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator;

internal sealed class FunctionGenerator
{
    private readonly TypeGenerator _typeGenerator;
    private readonly StatementGenerator _statementGenerator;

    public FunctionGenerator(TypeGenerator typeGenerator, StatementGenerator statementGenerator)
    {
        _typeGenerator = typeGenerator;
        _statementGenerator = statementGenerator;
    }
    
    private string GenerateFunctionSignature(FunctionStatement function)
    {
        string returnType = _typeGenerator.GenerateType(function.ReturnType);

        string name = CCodeUtils.GenerateIdentifier(function.Name);

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
                    $"{_typeGenerator.GenerateType(parameter.Type)} " +
                    $"{CCodeUtils.GenerateIdentifier(parameter.Name)}"
                )
            );
        }

        return $"{returnType} {name}({parameters})";
    }

    public void GenerateFunctionPrototype(
        StringBuilder output,
        FunctionStatement function)
    {
        output.AppendLine($"{GenerateFunctionSignature(function)};");
    }

    public void GenerateFunctionDefinition(
        StringBuilder output,
        FunctionStatement function)
    {
        output.AppendLine(GenerateFunctionSignature(function));
        
        _statementGenerator.GenerateBlock(output, function.Body, 0);

        output.AppendLine();
    }
}