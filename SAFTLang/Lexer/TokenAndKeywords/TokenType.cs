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
        
        And,
        Or,
        
        True,
        False,
        
        Number,
        
        Colon,

        Plus,
        Minus,
        Star,
        Slash,
        Modulo,
        Not,

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
        
        CommentHashtag,
        
        BadToken,
    }