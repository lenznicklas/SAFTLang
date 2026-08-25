namespace SAFTLang.Lexer;

public partial class Lexer
{
    private Token ReadIdentifier()
    {
        int start = _position;
        int line = _line;
        int column = _column;

        while (!IsAtEnd() && (char.IsLetterOrDigit(Current()) || Current() == '_'))
        {
            Advance();
        }

        string value = _source[start.._position];

        TokenType type = Keywords.TryGetValue(
            value,
            out TokenType keywordType)
            ? keywordType
            : TokenType.Identifier;
            
        return CreateToken(
            type,
            value,
            start,
            _position - start,
            line,
            column
        );
    }

    private Token ReadNumber()
    {
        int start = _position;
        int line = _line;
        int column = _column;

        while (!IsAtEnd() && (char.IsDigit(Current()) || Current() == '_'))
        {
            Advance();
        }

        string value = _source[start.._position];
        return CreateToken(
            TokenType.Number,
            value,
            start,
            _position - start,
            line,
            column
        );
    }

    private Token ReadString()
    {
        int start = _position;
        int line = _line;
        int column = _column;
        
        Advance();

        int valueStart = _position;
        
        while (!IsAtEnd() && Current() != '"')
        {
            Advance();
        }

        if (IsAtEnd())
        {
            throw new Exception($"{line}:{column}: unterminated string");
        }
        
        string value = _source[start.._position];
        
        Advance();

        return CreateToken(
            TokenType.String,
            value,
            start,
            _position - start,
            line,
            column
        );
    }

}