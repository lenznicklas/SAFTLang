namespace SAFTLang.Lexer.TokenAndKeywords;
    public enum TokenType
    {
        EOF,
        Identifier,
        String,
        Let,
        Const,
        If,
        Else,
        Func,
        Return,
        For,
        In,
        Break,
        
        True,
        False,
        
        Number,
        
        Colon,

        Plus,
        Minus,
        Star,
        Slash,
        Modulo,

        Equal,
        Semicolon,
        Comma,
        Newline,
        
        LParen,
        RParen,
        LBrace,
        RBrace,
        LBracket,
        RBracket,
        
        EqualEqual,
        NotEqual,
        Less,
        LessEqual,
        Greater,
        GreaterEqual,
        
        BoolType,
        IntType,
        StringType,
        VoidType,
        
        BadToken,
    }