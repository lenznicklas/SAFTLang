namespace SAFTLang.Lexer
{
    public record Token(TokenType Type, string Value);

    public class Lexer
    {
        private readonly string _source;
        private int _position;

        public Lexer(string source)
        {
            _source = source;
            _position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            
            while (!IsAtEnd())
            {
                char c = Current();
                
                // Newline
                if (c == '\n')
                {
                    tokens.Add(new Token(TokenType.Newline, "\\n"));
                    while (!IsAtEnd() && Current() == '\n')
                    {
                        Advance();
                    }
                    continue;
                }
                
                // Other Whitespaces ignore
                if (char.IsWhiteSpace(c))
                {
                    Advance();
                    continue;
                }
                
                // Number
                if (char.IsDigit(c))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }

                // Identifier / Keywords
                if (char.IsLetter(c) || c == '_')
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                switch (c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus, "+"));
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.Minus, "-"));
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Star, "*"));
                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Slash, "/"));
                        break;
                    case '=':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.EqualEqual, "=="));
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Equals, "="));
                        }
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon, ";"));
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LParen, "("));
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RParen, ")"));
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LBracket, "["));
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RBracket, "]"));
                        break;
                    case '{':
                        tokens.Add(new Token(TokenType.LBrace, "{"));
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RBrace, "}"));
                        break;
                    case '<':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.LessEqual, "<="));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Less, "<"));
                        }
                        break;
                    case '>':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.GreaterEqual, ">="));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Greater, ">"));
                        }
                        break;
                    case '!':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.NotEqual, "!="));
                        }
                        else
                        {
                            throw new Exception($"Unexpected character {Current()}");
                        }
                        break;
                    default:
                        throw new Exception($"Unexpected character '{c}'");
                }

                Advance();

            }

            tokens.Add(new Token(TokenType.EOF, ""));
            return tokens;
        }

        private Token ReadIdentifier()
        {
            int start = _position;

            while (!IsAtEnd() && (char.IsLetterOrDigit(Current()) || Current() == '_'))
            {
                Advance();
            }

            string value = _source[start.._position];

            switch (value)
            {
                case "let":
                    return new Token(TokenType.Let, "let");
                case "const":
                    return new Token(TokenType.Const, "const");
                case "true":
                    return new Token(TokenType.True, "true");
                case "false":
                    return new Token(TokenType.False, "false");
            }

            return new Token(TokenType.Identifier, value);
        }

        private Token ReadNumber()
        {
            int start = _position;

            while (!IsAtEnd() && (char.IsDigit(Current()) || Current() == '_'))
            {
                Advance();
            }

            string value = _source[start.._position];
            return new Token(TokenType.Number, value);
        }


        private char Current()
        {
            return _source[_position];
        }

        private void Advance()
        {
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
    }
}
