using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record ConstStatement(
    string Name, 
    LangType? DeclaredType,
    Expr Value,
    SourceSpan  Span
    ) : Statement(Span);
