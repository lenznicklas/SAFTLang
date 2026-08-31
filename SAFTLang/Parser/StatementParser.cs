using SAFTLang.Lexer;
using SAFTLang.AST;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser;

public partial class Parser
{
    private Statement? ParseStatement()
    {
        return Current().Type switch
        {
            TokenType.Let => ParseLetStatement(),
            TokenType.Const => ParseConstStatement(),
            TokenType.If => ParseIfStatement(),
            TokenType.Identifier 
                when Peek(1).Type == TokenType.Equal => 
                ParseAssignmentStatement(),
            TokenType.Identifier
                when Peek(1).Type == TokenType.LParen =>
                ParseExpressionStatement(),
            TokenType.Func => ParseFunctionStatement(),
            TokenType.Return => ParseReturnStatement(),
            
            _ =>  UnexpectedToken(Current())
        };
    }

    private Token Peek(int offset = 0)
    {
        int index = _position + offset;

        if (index >= _tokens.Count)
        {
            return _tokens[^1];
        }

        return _tokens[index];
    }
    
    private Statement UnexpectedToken(Token token)
    {
        _diagnostics.ReportError(
            token.Span,
            $"Unexpected token {token.Type} " +
            $"('{token.Value}') at start of statement"
            );

        int startPosition = _position;
        
        SynchronizeStatement();

        if (_position == startPosition && !IsAtEnd())
        {
            Advance();
        }
        return null;
    }
    
    private Statement ParseLetStatement()
    {
        Token letToken = Consume(TokenType.Let);
        
        Token name = Consume(TokenType.Identifier);

        LangType? type = ParseOptionalType();

        Consume(TokenType.Equal);

        Expr value = ParseExpression();

        ConsumeStatementEnd();

        SourceSpan span = SourceSpan.Combine(letToken.Span, value.Span);

        return new LetStatement(name.Value, type, value, span);
    }

    private Statement ParseConstStatement()
    {
        Token constToken = Consume(TokenType.Const);

        Token name = Consume(TokenType.Identifier);

        LangType? type = ParseOptionalType();
        
        Consume(TokenType.Equal);

        Expr value = ParseExpression();
        
        ConsumeStatementEnd();
        
        SourceSpan span =  SourceSpan.Combine(constToken.Span, value.Span);
        
        return new ConstStatement(name.Value, type, value, span);
    }

    private Statement ParseIfStatement()
    {
        Token ifToken = Consume(TokenType.If);

        Expr condition = ParseExpression();

        BlockStatement thenBody = ParseBlock();

        BlockStatement? elseBody = null;
        
        SkipNewLines();

        if (Current().Type == TokenType.Else)
        {
            Consume(TokenType.Else);
            elseBody = ParseBlock();
        }

        SourceSpan lastSpan = elseBody?.Span ?? thenBody.Span;
        
        SourceSpan span = SourceSpan.Combine(ifToken.Span, lastSpan);

        return new IfStatement(condition, thenBody, elseBody, span);
    }

    private BlockStatement ParseBlock()
    {
        Token lBToken = Consume(TokenType.LBrace);

        SkipNewLines();

        var statements = new List<Statement>();

        while (Current().Type != TokenType.RBrace && !IsAtEnd())
        {
            Statement? statement = ParseStatement();

            if (statement is not null)
            {
                statements.Add(statement);
            }
            
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

    private LangType? ParseOptionalType()
    {
        if (Current().Type != TokenType.Colon)
        {
            return null;
        }

        Consume(TokenType.Colon);
        
        return ParseType();
    }

    private LangType ParseType()
    {
        Token token = Current();

        LangType type = token.Type switch
        {
            TokenType.IntType => LangType.Int,
            TokenType.BoolType => LangType.Bool,
            TokenType.StringType => LangType.String,
            TokenType.VoidType => LangType.Void,
            _ => LangType.Error
        };

        if (type == LangType.Error)
        {
            _diagnostics.ReportError(token.Span,
                $"Expected Type, got {token.Type}"
            );

            return LangType.Error;
        }
        
        Advance();

        return type;
    }

    private Statement ParseExpressionStatement()
    {
        Expr expr = ParseExpression();
        ConsumeStatementEnd();

        return new ExpressionStatement(
            expr,
            expr.Span
        );
    }

    private Statement ParseFunctionStatement()
    {
        Token funcToken = Consume(TokenType.Func);
        
        Token name = Consume(TokenType.Identifier);

        Consume(TokenType.LParen);
        
        var parameters = new List<Parameter>();

        if (Current().Type != TokenType.RParen)
        {
            while (true)
            {
                Token parameterName = Consume(TokenType.Identifier);

                Consume(TokenType.Colon);

                LangType parameterType = ParseType();

                parameters.Add(new Parameter(parameterName.Value, parameterType, parameterName.Span));

                if (Current().Type != TokenType.Comma)
                {
                    break;
                }

                Consume(TokenType.Comma);
            }
        }

        Consume(TokenType.RParen);
            
        LangType returnType = ParseType();
            
        BlockStatement body = ParseBlock();

        SourceSpan span = SourceSpan.Combine(funcToken.Span, body.Span);

        return new FunctionStatement(
            name.Value,
            parameters,
            returnType,
            body,
            span
        );

    }

    private Statement ParseReturnStatement()
    {
        Token returnToken = Consume(TokenType.Return);

        Expr? value = null;
        
        bool hasValue =
            Current().Type != TokenType.RBrace &&
            Current().Type != TokenType.Newline &&
            Current().Type != TokenType.EOF &&
            Current().Type != TokenType.Semicolon;

        if (hasValue)
        {
            value = ParseExpression();
        }
        
        ConsumeStatementEnd();
        
        return new ReturnStatement(value,  returnToken.Span);
    }
    
}