using SAFTLang.Lexer.Text;

namespace SAFTLang.Lexer;
    

    public partial class Lexer
    {
        private readonly string _source;
        private int _position;
        private int _line;
        private int _column;
        private int _parenthesisDepth;
        
        public Lexer(string source)
        {
            _source = source;
            
            _position = 0;
            _line = 1;
            _column = 1;
            _parenthesisDepth = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (!IsAtEnd())
            {
                int tokenStart = _position;
                int tokenLine = _line;
                int tokenColumn = _column;

                char c = Current();

                // Newline
                if (c == '\n')
                {
                    while (!IsAtEnd() && Current() == '\n')
                    {
                        Advance();
                    }

                    if (_parenthesisDepth == 0)
                    {
                        tokens.Add(
                            CreateToken(
                                TokenType.Newline,
                                "\\n",
                                tokenStart,
                                _position - tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
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

                if (c == '"')
                {
                    tokens.Add(ReadString());
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
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.Plus,
                                "+",
                                tokenStart,
                                tokenLine,
                                tokenColumn)
                        );
                        break;
                    case '-':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.Minus,
                                "-",
                                tokenStart,
                                tokenLine,
                                tokenColumn)
                        );
                        break;
                    case '*':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.Star,
                                "*",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        break;
                    case '/':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.Slash,
                                "/",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        break;
                    case '=':
                        if (Peek() == '=')
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.EqualEqual,
                                    "==",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                            Advance();
                        }
                        else
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.Equal,
                                    "=",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                        }

                        break;
                    case ';':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.Semicolon,
                                ";",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        break;
                    case '(':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.LParen,
                                "(",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        _parenthesisDepth++;
                        break;
                    case ')':
                        if (_parenthesisDepth == 0)
                        {
                            throw new Exception($"{tokenLine}:{tokenColumn}Unexpected closing parenthesis ')'");
                        }

                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.RParen,
                                ")",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        _parenthesisDepth--;
                        break;
                    case '{':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.LBrace,
                                "{",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        break;
                    case '}':
                        tokens.Add(
                            CreateSimpleToken(
                                TokenType.RBrace,
                                "}",
                                tokenStart,
                                tokenLine,
                                tokenColumn
                            )
                        );
                        break;
                    case '<':
                        if (Peek() == '=')
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.LessEqual,
                                    "<=",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                            Advance();
                        }
                        else
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.Less,
                                    "<",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                        }

                        break;
                    case '>':
                        if (Peek() == '=')
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.GreaterEqual,
                                    ">=",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                            Advance();
                        }
                        else
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.Greater,
                                    ">",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                        }

                        break;
                    case '!':
                        if (Peek() == '=')
                        {
                            tokens.Add(
                                CreateSimpleToken(
                                    TokenType.NotEqual,
                                    "!=",
                                    tokenStart,
                                    tokenLine,
                                    tokenColumn
                                )
                            );
                            Advance();
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

            if (_parenthesisDepth != 0)
            {
                throw new Exception($"{_line}:{_column}: Unclosed parens");
            }
            
            tokens.Add(
                new Token(
                    TokenType.EOF,
                    "",
                    new SourceSpan(
                        _position,
                        0,
                        _line,
                        _column
                    )
                )
            );
            return tokens;
        }

    }
