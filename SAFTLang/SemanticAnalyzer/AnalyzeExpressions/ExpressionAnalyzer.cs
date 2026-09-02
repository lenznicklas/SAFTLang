using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Diagnostics;
using SAFTLang.SemanticAnalyzer.Symbols;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;

namespace SAFTLang.SemanticAnalyzer.AnalyzeExpressions;

internal sealed partial class ExpressionAnalyzer
{
    private readonly SemanticAnalyzerState _state;
    private readonly DiagnosticBag _diagnostics;

    public ExpressionAnalyzer(SemanticAnalyzerState state, DiagnosticBag diagnostics)
    {
        _state = state;
        _diagnostics = diagnostics;
    }
    
    public LangType AnalyzeExpression(Expr expr, LangType? expectedType = null)
    {
        LangType type = expr switch
        {
            IntegerExpr =>
                LangType.Int,
            BoolExpr =>
                LangType.Bool,
            StringExpr =>
                LangType.String,
            IdentifierExpr ident =>
                AnalyzeIdentifier(ident),
            BinaryExpr binary =>
                AnalyzeBinary(binary),
            CallExpr call =>
                AnalyzeCall(call),
            ArrayExpr array => 
                AnalyzeArray(array, expectedType),
            IndexExpr index =>
                AnalyzeIndex(index),
            ErrorExpr =>
                LangType.Error,
            _ => ReportUnknownExpression(expr)
        
        };

        _state.SetExpressionType(expr, type);

        return type;
    }
    
    private bool RequireTypes(
        TokenType op,
        LangType left,
        LangType right,
        LangType expected,
        SourceSpan span)
    {
        if (left == LangType.Error ||
            right == LangType.Error)
        {
            return false;
        }
        if (left != expected || right != expected)
        {
            _diagnostics.ReportError(
                span,
                $"Operator {op} requires type {expected} but got {left} and {right}"
            );
            return false;
        }
        return true;
    }
    
    private LangType ReportUnknownExpression(Expr expr)
    {
        _diagnostics.ReportError(
            expr.Span,
            $"Unknown expression: {expr.GetType().Name}"
        );
        return LangType.Error;
    }



}