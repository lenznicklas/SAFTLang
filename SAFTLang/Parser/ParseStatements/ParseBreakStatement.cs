using SAFTLang.AST.Statements;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private BreakStatement ParseBreakStatement()
    {
        Token breakToken = _state.Consume(TokenType.Break);
        
        _state.ConsumeStatementEnd();

        return new BreakStatement(breakToken.Span);
    }
    
}