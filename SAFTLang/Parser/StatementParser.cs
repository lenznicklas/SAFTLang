using SAFTLang.Lexer;
using SAFTLang.AST;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser;

public partial class Parser
{
    private Statement ParseStatement()
    {

        return Current().Type switch
        {
            TokenType.Let => ParseLetStatement(),
            TokenType.Const => ParseConstStatement(),
            TokenType.If => ParseIfStatement(),
            TokenType.Identifier => ParseAssignmentStatement(),
            _ => throw new Exception($"{Current().Span}: Unexpected token {Current().Type}"),
        };
    }

    private Statement ParseLetStatement()
    {
        Token letToken = Consume(TokenType.Let);
        
        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equal);

        Expr value = ParseExpression();

        ConsumeStatementEnd();

        SourceSpan span = SourceSpan.Combine(letToken.Span, value.Span);

        return new LetStatement(name.Value, value, span);
    }

    private Statement ParseConstStatement()
    {
        Token constToken = Consume(TokenType.Const);

        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equal);

        Expr value = ParseExpression();
        
        ConsumeStatementEnd();
        
        SourceSpan span =  SourceSpan.Combine(constToken.Span, value.Span);
        
        return new ConstStatement(name.Value, value, span);
    }

    private Statement ParseIfStatement()
    {
        Token ifToken = Consume(TokenType.If);

        Expr condition = ParseExpression();

        BlockStatement body = ParseBlock();

        SourceSpan span = SourceSpan.Combine(ifToken.Span, body.Span);
        
        return new IfStatement(condition, body, span);
    }

    private BlockStatement ParseBlock()
    {
        Token lBToken = Consume(TokenType.LBrace);

        SkipNewLines();

        var statements = new List<Statement>();

        while (Current().Type != TokenType.RBrace && !IsAtEnd())
        {
            statements.Add(ParseStatement());
            SkipNewLines();
        }

        Token rBToken = Consume(TokenType.RBrace);
        
        SourceSpan span = SourceSpan.Combine(lBToken.Span, rBToken.Span);
            
        BlockStatement block = new BlockStatement(statements, span);
        return block;
    }

    private Statement ParseAssignmentStatement()
    {
        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.Equal);

        Expr value = ParseExpression();
        
        ConsumeStatementEnd();
        
        SourceSpan span =  SourceSpan.Combine(name.Span, value.Span);
        
        return new AssignmentStatement(name.Value, value, span);
    }
    
}