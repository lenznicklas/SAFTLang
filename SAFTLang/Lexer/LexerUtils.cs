namespace SAFTLang.Lexer;

public partial class Lexer
{
    private char Current()
    {
        return _source[_position];
    }

    private void Advance()
    {
        if (IsAtEnd())
        {
            return;
        }

        if (Current() == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }
            
        _position++;
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private char Peek()
    {
        if (_position + 1 >= _source.Length)
        {
            return '\0';
        }
        return _source[_position + 1];
    }

    private Token CreateToken(
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
            new SourceSpan(start, length, line, column)
        );
    }

    private Token CreateSimpleToken(
        TokenType type,
        string value,
        int start,
        int line,
        int column
    )
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

