namespace SAFTLang.Lexer
{
    public enum TokenType
    {
        EOF,
        Identifier,
        String,
        Let,
        Const,
        If,
        
        True,
        False,
        
        Number,

        Plus,
        Minus,
        Star,
        Slash,

        Equal,
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
        
        BoolType,
        IntType,
        StringType,
    }}