namespace SAFTLang.Lexer
{
    public enum TokenType
    {
        EOF,
        Identifier,
        String,
        Let,
        Const,
        
        True,
        False,
        
        Number,

        Plus,
        Minus,
        Star,
        Slash,

        Equals,
        Semicolon,
        Newline,
        
        LParen,
        RParen,
        LBracket,
        RBracket,
        LBrace,
        RBrace,
        
        EqualEqual,
        NotEqual,
        Less,
        LessEqual,
        Greater,
        GreaterEqual,
    }}