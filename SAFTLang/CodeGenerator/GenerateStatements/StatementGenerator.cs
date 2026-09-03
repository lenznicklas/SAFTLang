using System.Text;
using SAFTLang.AST.Statements;
using SAFTLang.CodeGenerator.GenerateExpressions;
using SAFTLang.CodeGenerator.GenerateTypes;
using SAFTLang.CodeGenerator.Utils;

namespace SAFTLang.CodeGenerator.GenerateStatements;

internal sealed partial class StatementGenerator
{
    private readonly SemanticAnalyzer.SemanticAnalyzer _analyzer;
    private readonly ExpressionGenerator _expressionGenerator;
    private readonly TypeGenerator _typeGenerator;

    public StatementGenerator(SemanticAnalyzer.SemanticAnalyzer analyzer, ExpressionGenerator expressionGenerator,
        TypeGenerator typeGenerator)
    {
        _analyzer = analyzer;
        _expressionGenerator = expressionGenerator;
        _typeGenerator = typeGenerator;
    }
    
        private void GenerateStatement(
        StringBuilder output,
        Statement statement,
        int indent)
    {
        string indentation = new string(' ', indent * 4);

        switch (statement)
        {
            case LetStatement let:
                GenerateLet(output, let, indentation);
                break;
            
            case ConstStatement constStatement:
                GenerateConst(output, constStatement, indentation);
                break;
            
            case IfStatement ifStatement:
                GenerateIf(output, ifStatement, indentation, indent);
                break;
            
            case ExpressionStatement expressionStatement:
                GenerateExpressionStatement(output, expressionStatement, indentation);
                break;
            
            case BlockStatement blockStatement:
                GenerateBlock(output, blockStatement, indent);
                break;
            
            case AssignmentStatement assignmentStatement:
                GenerateAssignment(output, assignmentStatement, indentation);
                break;
            
            case ReturnStatement returnStatement:
                GenerateReturn(output, returnStatement, indentation);
                break;
            
            case ForStatement forStatement:
                GenerateFor(output, forStatement, indentation, indent);
                break;
            
            case ForEachStatement forEachStatement:
                GenerateForEach(output, forEachStatement, indentation, indent);
                break;
            
            default:
                throw new InvalidOperationException($"Unknown statement {statement.GetType().Name}");
        }
    }
    
}