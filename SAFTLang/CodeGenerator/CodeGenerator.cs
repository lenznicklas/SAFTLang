using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.CodeGenerator.GenerateExpressions;
using SAFTLang.CodeGenerator.GenerateFunctions;
using SAFTLang.CodeGenerator.GenerateStatements;
using SAFTLang.CodeGenerator.GenerateTypes;
using SAFTLang.CodeGenerator.Runtime;

namespace SAFTLang.CodeGenerator;

public sealed class CodeGenerator
{
    private readonly FunctionGenerator _functionGenerator;

    public CodeGenerator(SemanticAnalyzer.SemanticAnalyzer analyzer)
    {
        var types = new TypeGenerator();
        
        var expressions = new ExpressionGenerator(analyzer, types);

        var statements = new StatementGenerator(analyzer, expressions, types);

        _functionGenerator = new FunctionGenerator(types, statements);
    }
    
    public string Generate(List<Statement> statements)
    {
        var output = new StringBuilder();

        output.AppendLine("#include <stdio.h>");
        output.AppendLine("#include <stdbool.h>");
        output.AppendLine("#include <stdlib.h>");
        output.AppendLine("#include <string.h>");
        output.AppendLine();
        
        RuntimeGenerator.GenerateRuntime(output);
        
        List<FunctionStatement> functions = statements.OfType<FunctionStatement>().ToList();

        foreach (FunctionStatement function in functions)
        {
            _functionGenerator.GenerateFunctionPrototype(output, function);
        }

        if (functions.Count > 0)
        {
            output.AppendLine();
        }

        foreach (FunctionStatement function in functions)
        {
            _functionGenerator.GenerateFunctionDefinition(output, function);
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