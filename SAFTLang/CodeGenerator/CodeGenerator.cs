using System.Text;
using SAFTLang.AST.Statements;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private readonly SemanticAnalyzer.SemanticAnalyzer _analyzer;

    public CodeGenerator(SemanticAnalyzer.SemanticAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }
    
    public string Generate(List<Statement> statements)
    {
        var output = new StringBuilder();

        output.AppendLine("#include <stdio.h>");
        output.AppendLine("#include <stdbool.h>");
        output.AppendLine("#include <stdlib.h>");
        output.AppendLine("#include <string.h>");
        output.AppendLine();

        GenerateRuntime(output);
        
        List<FunctionStatement> functions = statements.OfType<FunctionStatement>().ToList();

        foreach (FunctionStatement function in functions)
        {
            GenerateFunctionPrototype(output, function);
        }

        if (functions.Count > 0)
        {
            output.AppendLine();
        }

        foreach (FunctionStatement function in functions)
        {
            GenerateFunctionDefinition(output, function);
        }
        
        output.AppendLine("int main(void)");
        output.AppendLine("{");
        output.AppendLine("    saft_main();");
        output.AppendLine();
        output.AppendLine("    return 0;");
        output.AppendLine("}");
        return output.ToString();
    }

}