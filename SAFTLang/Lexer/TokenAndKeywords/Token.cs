using SAFTLang.Lexer.Text;

namespace SAFTLang.Lexer.TokenAndKeywords;

public record Token(
    TokenType Type, 
    string Value, 
    SourceSpan Span
    );
