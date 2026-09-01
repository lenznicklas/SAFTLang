using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer;

internal sealed class LexerState
{
    public string Source { get; }
    
    public int Position { get; private set; }
    public int Line { get; private set; } = 1;
    public int Column { get; private set; } = 1;

    public LexerState(string source)
    {
        Source = source;
    }
    
    public bool IsAtEnd => Position >= Source.Length;
    
    public char Current => IsAtEnd ? '\0' : Source[Position];

    public char Peek => Position + 1 >= Source.Length ? '\0' : Source[Position + 1];

    public void Advance()
    {
        if (IsAtEnd)
        {
            return;
        }

        if (Current == '\n')
        {
            Line++;
            Column = 1;
        }
        else
        {
            Column++;
        }
        
        Position++;
    }

    public Token CreateToken(
        TokenType type,
        string value,
        int start,
        int length,
        int line,
        int column)
    {
        return new Token(
            type,
            value,
            new SourceSpan(
                start,
                length,
                line,
                column
            )
        );
    }

    public Token CreateSimpleToken(
        TokenType type,
        string value,
        int start,
        int line,
        int column)
    {
        return CreateToken(
            type,
            value,
            start,
            value.Length,
            line,
            column
        );

    }

}