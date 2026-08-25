namespace SAFTLang.Lexer;

public partial class Lexer
{
    private static readonly Dictionary<string, TokenType>
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
        };
}