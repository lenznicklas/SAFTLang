using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.CodeGenerator.GenerateTypes;
using SAFTLang.CodeGenerator.Utils;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.CodeGenerator.GenerateExpressions;

internal sealed partial class ExpressionGenerator
{
    private readonly SemanticAnalyzer.SemanticAnalyzer _analyzer;
    private readonly TypeGenerator _typeGenerator;

    public ExpressionGenerator(SemanticAnalyzer.SemanticAnalyzer analyzer, TypeGenerator typeGenerator)
    {
        _analyzer = analyzer;
        _typeGenerator = typeGenerator;
    }
    
    public string GenerateExpression(Expr expr)
    {
        return expr switch
        {
            IntegerExpr num =>
                num.Value,

            IdentifierExpr ident =>
                CCodeUtils.GenerateIdentifier(ident.Name),

            BinaryExpr binary =>
                GenerateBinaryExpression(binary),
            
            BoolExpr boolean =>
                boolean.Value ? "true" : "false",
            
            StringExpr str =>
                $"\"{CCodeUtils.EscapeCString(str.Value)}\"",
            
            CallExpr call =>
                GenerateCallExpression(call),
            
            ArrayExpr array =>
                GenerateArrayExpression(array),
            
            IndexExpr index =>
                GenerateIndexExpression(index),

            _ => throw new InvalidOperationException($"Unknown expression {expr.GetType().Name}")
        };
    }







}