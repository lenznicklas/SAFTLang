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
        Else,
        
        True,
        False,
        
        Number,
        
        Colon,

        Plus,
        Minus,
        Star,
        Slash,

        Equal,
        Semicolon,
        Comma,
        Newline,
        
        LParen,
        RParen,
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