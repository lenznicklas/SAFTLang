using SAFTLang.AST;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer;

namespace SAFTLang.Parser;

public partial class Parser
{
    private readonly List<Token> _tokens;
    private readonly DiagnosticBag _diagnostics = new();
    
    private int _position;

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
    
    public bool HasErrors => _diagnostics.HasErrors;
    
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public List<Statement> Parse()
    {
        var statements = new List<Statement>();
        
        SkipNewLines();
        
        while (!IsAtEnd())
        {
            Statement? statement = ParseStatement();
            if (statement is not null)
            {
                statements.Add(statement);
            }
            SkipNewLines();
        }
        return statements;
    }



}