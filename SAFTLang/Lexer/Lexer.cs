using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Readers;
using SAFTLang.Lexer.Text;
using SAFTLang.Lexer.TokenAndKeywords;

namespace SAFTLang.Lexer;
    

    public class Lexer
    {
        private readonly LexerState _state;
        private readonly TokenReader _reader;

        private readonly DiagnosticBag _diagnostics = new();
        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
        public bool HasErrors => _diagnostics.HasErrors;
        
        private int _parenthesisDepth;
        private int _bracketDepth;
        
        public Lexer(string source)
        {
            _state = new LexerState(source);
            _reader = new TokenReader(_state, _diagnostics);
            _parenthesisDepth = 0;
            _bracketDepth = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (!_state.IsAtEnd)
            {
                int tokenStart = _state.Position;
                int tokenLine = _state.Line;
                int tokenColumn = _state.Column;

                char c = _state.Current;

                // Newline
                if (c == '\n')
                {
                    while (!_state.IsAtEnd && _state.Current == '\n')
                    {
                        _state.Advance();
                    }

                    if (_parenthesisDepth == 0 && _bracketDepth == 0)
                    {
                        tokens.Add(
                            _state.CreateToken(
                                TokenType.Newline,
                                "\\n",
                                tokenStart,
                                _state.Position - tokenStart,
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
                    _state.Advance();
                    continue;
                }

                // Number
                if (char.IsDigit(c))
                {
                    tokens.Add(_reader.ReadNumber());
                    continue;
                }

                if (c == '"')
                {
                    tokens.Add(_reader.ReadString());
                    continue;
                }

                if (c == '\'')
                {
                    tokens.Add(_reader.ReadChar());
                    continue;
                }

                // Identifier / Keywords
                if (char.IsLetter(c) || c == '_')
                {
                    tokens.Add(_reader.ReadIdentifier());
                    continue;
                }

                switch (c)
                {
                    case '+':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Plus, "+", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '-':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Minus, "-", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '*':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Star, "*", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '/':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Slash, "/", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '%':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Modulo, "%", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '=':
                        if (_state.Peek == '=')
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.EqualEqual, "==", tokenStart, tokenLine, tokenColumn)
                            );
                            _state.Advance();
                        }
                        else
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.Equal, "=", tokenStart, tokenLine, tokenColumn)
                            );
                        }
                        break;
                    case ';':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Semicolon, ";", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case ':':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Colon, ":", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '(':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.LParen, "(", tokenStart, tokenLine, tokenColumn)
                        );
                        _parenthesisDepth++;
                        break;
                    case ')':
                        if (_parenthesisDepth == 0)
                        {
                            _diagnostics.ReportError(
                                new SourceSpan(
                                    tokenStart,
                                    1,
                                    tokenLine,
                                    tokenColumn
                                    ),
                                "Unexpected closing parenthesis ')'"
                                );
                            break;
                        }

                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.RParen, ")", tokenStart, tokenLine, tokenColumn)
                        );
                        _parenthesisDepth--;
                        break;
                    case '{':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.LBrace, "{", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '}':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.RBrace, "}", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    case '[':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.LBracket, "[", tokenStart, tokenLine, tokenColumn)
                        );
                        _bracketDepth++;
                        break;
                    case ']':
                        if (_bracketDepth == 0)
                        {
                            _diagnostics.ReportError(
                                new SourceSpan(
                                    tokenStart,
                                    1,
                                    tokenLine,
                                    tokenColumn
                                    ),
                                "Unexpected closing bracket ']'"
                                );
                            break;
                        }
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.RBracket, "]", tokenStart, tokenLine, tokenColumn)
                        );
                        _bracketDepth--;
                        break;
                    case '<':
                        if (_state.Peek == '=')
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.LessEqual, "<=", tokenStart, tokenLine, tokenColumn)
                            );
                            _state.Advance();
                        }
                        else
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.Less, "<", tokenStart, tokenLine, tokenColumn)
                            );
                        }

                        break;
                    case '>':
                        if (_state.Peek == '=')
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.GreaterEqual, ">=", tokenStart, tokenLine, tokenColumn)
                            );
                            _state.Advance();
                        }
                        else
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.Greater, ">", tokenStart, tokenLine, tokenColumn)
                            );
                        }

                        break;
                    case '!':
                        if (_state.Peek == '=')
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.NotEqual, "!=", tokenStart, tokenLine, tokenColumn)
                            );
                            _state.Advance();
                        }
                        else
                        {
                            tokens.Add(
                                _state.CreateSimpleToken(TokenType.Not, "!", tokenStart, tokenLine, tokenColumn)
                            );
                        }

                        break;
                    case ',':
                        tokens.Add(
                            _state.CreateSimpleToken(TokenType.Comma, ",", tokenStart, tokenLine, tokenColumn)
                        );
                        break;
                    
                    case '#':
                        while (_state.Current != '\n' &&
                               !_state.IsAtEnd)
                        {
                            _state.Advance();
                        }
                        continue;
                    default:
                    {
                        _diagnostics.ReportError(
                            new SourceSpan(
                                tokenStart,
                                1,
                                tokenLine,
                                tokenColumn
                            ),
                            $"Unexpected character {c}"
                        );
                        
                        break;
                    }
                }

                _state.Advance();

            }

            if (_parenthesisDepth != 0)
            {
                _diagnostics.ReportError(
                    new SourceSpan(
                        _state.Position,
                        0,
                        _state.Line,
                        _state.Column
                    ),
                    "Unclosed parenthesis '('"
                );
            }

            if (_bracketDepth != 0)
            {
                _diagnostics.ReportError(
                    new SourceSpan(
                        _state.Position,
                        0,
                        _state.Line,
                        _state.Column
                    ),
                    "Unclosed bracket '['"
                );
            }
            
            tokens.Add(
                new Token(
                    TokenType.EOF,
                    "",
                    new SourceSpan(_state.Position, 0, _state.Line, _state.Column)
                )
            );
            return tokens;
        }

    }
