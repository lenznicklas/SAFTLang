using SAFTLang.AST.Statements;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Parser.ParseExpressions;
using SAFTLang.Parser.ParseStatements;
using SAFTLang.Parser.ParseTypes;

namespace SAFTLang.Parser;

public partial class Parser
{
    private readonly ParserState _state;
    private readonly StatementParser _statementParser;
    
    private readonly List<Token> _tokens;
    private readonly DiagnosticBag _diagnostics = new();
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
    
    public bool HasErrors => _diagnostics.HasErrors;
    
    public Parser(List<Token> tokens)
    {
        _state = new ParserState(tokens, _diagnostics);
        
        var expressions = new ExpressionParser(_state,  _diagnostics);
        var type = new TypeParser(_state, _diagnostics);

        _statementParser = new StatementParser(_state, expressions, _diagnostics, type);
        
        _tokens = tokens;
    }

    public List<Statement> Parse()
    {
        var statements = new List<Statement>();
        
        _state.SkipNewLines();
        
        while (!_state.IsAtEnd)
        {
            Statement? statement = _statementParser.ParseStatement();
            if (statement is not null)
            {
                statements.Add(statement);
            }
            _state.SkipNewLines();
        }
        return statements;
    }



}