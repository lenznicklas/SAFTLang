namespace SAFTLang.Lexer.TokenAndKeywords;

public static class KeywordsDict
{
    public static readonly Dictionary<string, TokenType>
        Keywords = new()
        {
            ["let"] = TokenType.Let,
            ["const"] = TokenType.Const,
            ["true"] = TokenType.True,
            ["false"] = TokenType.False,
            ["if"] = TokenType.If,
            ["int"] = TokenType.IntType,
            ["string"] = TokenType.StringType,
            ["bool"] = TokenType.BoolType,
            ["else"] = TokenType.Else,
            ["void"] = TokenType.VoidType,
            ["return"] = TokenType.Return,
            ["func"] = TokenType.Func,
            ["for"] = TokenType.For,
            ["in"] = TokenType.In,
            ["break"] = TokenType.Break,
            ["and"] = TokenType.And,
            ["or"] = TokenType.Or,
        };
}