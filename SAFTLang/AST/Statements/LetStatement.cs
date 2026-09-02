using SAFTLang.AST.Expressions;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.Text;

namespace SAFTLang.AST.Statements;

public record LetStatement(
    string Name,
    LangType? DeclaredType,
    Expr Value,
    SourceSpan Span
) : Statement(Span);
