using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer.Symbols;

public record FunctionSymbol(
    string Name,
    IReadOnlyList<LangType> ParameterTypes,
    LangType ReturnType
);