using SAFTLang.AST.Statements;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Parser.ParseExpressions;
using SAFTLang.Parser.ParseTypes;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private readonly ParserState _state;
    private readonly ExpressionParser _expressionParser;
    private readonly TypeParser _typeParser;
    private readonly DiagnosticBag _diagnostics;

    public StatementParser(ParserState state, ExpressionParser expressionParser, DiagnosticBag diagnostics, TypeParser typeParser)
    {
        _state = state;
        _expressionParser = expressionParser;
        _diagnostics = diagnostics;
        _typeParser = typeParser;
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
            TokenType.For => ParseForStatement(),
            TokenType.Break => ParseBreakStatement(),
            TokenType.Import => ParseImportStatement(),
            
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
    
}