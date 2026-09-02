using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Parser.ParseExpressions;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
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