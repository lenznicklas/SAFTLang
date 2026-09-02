using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Parser.ParseExpressions;

namespace SAFTLang.Parser.ParseStatements;

internal sealed class StatementParser
{
    private readonly ParserState _state;
    private readonly ExpressionParser _expressionParser;
    private readonly DiagnosticBag _diagnostics;

    public StatementParser(ParserState state, ExpressionParser expressionParser, DiagnosticBag diagnostics)
    {
        _state = state;
        _expressionParser = expressionParser;
        _diagnostics = diagnostics;
    }
    
    public Statement? ParseStatement()
    {
        return _state.Current.Type switch
        {
            TokenType.Let => ParseLetStatement(),
            TokenType.Const => ParseConstStatement(),
            TokenType.If => ParseIfStatement(),
            TokenType.Identifier => ParseIdentifierStartedStatement(),
            TokenType.Func => ParseFunctionStatement(),
            TokenType.Return => ParseReturnStatement(),
            
            _ =>  UnexpectedToken(_state.Current)
        };
    }
    
    private Statement ParseLetStatement()
    {
        Token letToken = _state.Consume(TokenType.Let);
        
        Token name = _state.Consume(TokenType.Identifier);

        LangType? type = ParseOptionalType();

        _state.Consume(TokenType.Equal);

        Expr value = _expressionParser.ParseExpression();

        _state.ConsumeStatementEnd();

        SourceSpan span = SourceSpan.Combine(letToken.Span, value.Span);

        return new LetStatement(name.Value, type, value, span);
    }
    
    private Statement ParseConstStatement()
    {
        Token constToken = _state.Consume(TokenType.Const);

        Token name = _state.Consume(TokenType.Identifier);

        LangType? type = ParseOptionalType();
        
        _state.Consume(TokenType.Equal);

        Expr value = _expressionParser.ParseExpression();
        
        _state.ConsumeStatementEnd();
        
        SourceSpan span =  SourceSpan.Combine(constToken.Span, value.Span);
        
        return new ConstStatement(name.Value, type, value, span);
    }
    
    private Statement ParseIfStatement()
    {
        Token ifToken = _state.Consume(TokenType.If);

        Expr condition = _expressionParser.ParseExpression();

        BlockStatement thenBody = ParseBlock();

        BlockStatement? elseBody = null;
        
        _state.SkipNewLines();

        if (_state.Current.Type == TokenType.Else)
        {
            _state.Consume(TokenType.Else);
            elseBody = ParseBlock();
        }

        SourceSpan lastSpan = elseBody?.Span ?? thenBody.Span;
        
        SourceSpan span = SourceSpan.Combine(ifToken.Span, lastSpan);

        return new IfStatement(condition, thenBody, elseBody, span);
    }
    
    private Statement ParseIdentifierStartedStatement()
    {
        Expr left = _expressionParser.ParseExpression();

        if (_state.Current.Type == TokenType.Equal)
        {
            _state.Consume(TokenType.Equal);

            Expr value = _expressionParser.ParseExpression();
            
            _state.ConsumeStatementEnd();

            SourceSpan span = SourceSpan.Combine(left.Span, value.Span);

            return new AssignmentStatement(left, value, span);
        }
        
        _state.ConsumeStatementEnd();

        return new ExpressionStatement(left, left.Span);
    }

    private Statement ParseFunctionStatement()
    {
        Token funcToken = _state.Consume(TokenType.Func);
        
        Token name = _state.Consume(TokenType.Identifier);

        _state.Consume(TokenType.LParen);
        
        var parameters = new List<Parameter>();

        if (_state.Current.Type != TokenType.RParen)
        {
            while (true)
            {
                Token parameterName = _state.Consume(TokenType.Identifier);

                _state.Consume(TokenType.Colon);

                LangType parameterType = ParseType();

                parameters.Add(new Parameter(parameterName.Value, parameterType, parameterName.Span));

                if (_state.Current.Type != TokenType.Comma)
                {
                    break;
                }

                _state.Consume(TokenType.Comma);
            }
        }

        _state.Consume(TokenType.RParen);
            
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
        Token returnToken = _state.Consume(TokenType.Return);

        Expr? value = null;
        
        bool hasValue =
            _state.Current.Type != TokenType.RBrace &&
            _state.Current.Type != TokenType.Newline &&
            _state.Current.Type != TokenType.EOF &&
            _state.Current.Type != TokenType.Semicolon;

        if (hasValue)
        {
            value = _expressionParser.ParseExpression();
        }
        
        _state.ConsumeStatementEnd();
        
        return new ReturnStatement(value,  returnToken.Span);
    }

    private Statement UnexpectedToken(Token token)
    {
        _diagnostics.ReportError(
            token.Span,
            $"Unexpected token {token.Type} " +
            $"('{token.Value}') at start of statement"
        );

        int startPosition = _state.Position;
        
        _state.SynchronizeStatement();

        if (_state.Position == startPosition && !_state.IsAtEnd)
        {
            _state.Advance();
        }
        return null;
    }
    
    private BlockStatement ParseBlock()
    {
        Token lBToken = _state.Consume(TokenType.LBrace);

        _state.SkipNewLines();

        var statements = new List<Statement>();

        while (_state.Current.Type != TokenType.RBrace && !_state.IsAtEnd)
        {
            Statement? statement = ParseStatement();

            if (statement is not null)
            {
                statements.Add(statement);
            }
            
            _state.SkipNewLines();
        }

        Token rBToken = _state.Consume(TokenType.RBrace);
        
        SourceSpan span = SourceSpan.Combine(lBToken.Span, rBToken.Span);
            
        BlockStatement block = new BlockStatement(statements, span);
        return block;
    }

    private LangType? ParseOptionalType()
    {
        if (_state.Current.Type != TokenType.Colon)
        {
            return null;
        }

        _state.Consume(TokenType.Colon);
        
        return ParseType();
    }
    
    private LangType ParseType()
    {
        Token token = _state.Current;

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
            
            _state.Advance();

            return LangType.Error;
        }
        
        _state.Advance();

        while (_state.Current.Type == TokenType.LBracket &&
               _state.Peek(1).Type == TokenType.RBracket)
        {
            _state.Consume(TokenType.LBracket);
            _state.Consume(TokenType.RBracket);

            if (type == LangType.Void)
            {
                _diagnostics.ReportError(
                    token.Span,
                    "Array element type cannot be void"
                );

                type = LangType.Error;
                continue;
            }

            if (type != LangType.Error)
            {
                type = LangType.ArrayOf(type);
            }
        }

        return type;
    }





}