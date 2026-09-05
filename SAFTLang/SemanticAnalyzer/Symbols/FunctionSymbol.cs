using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.Symbols;

public record FunctionSymbol(
    string Name,
    string QualifiedName,
    IReadOnlyList<LangType> ParameterTypes,
    LangType ReturnType
);