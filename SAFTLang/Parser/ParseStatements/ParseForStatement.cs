using SAFTLang.AST.Statements;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseForStatement()
    {
        Token forToken = _state.Consume(TokenType.For);

        BlockStatement block = ParseBlockStatement();

        SourceSpan span = SourceSpan.Combine(forToken.Span, block.Span);
        
        return new ForStatement(block, span);
    }
}