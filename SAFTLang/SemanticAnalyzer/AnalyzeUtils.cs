using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer;

public record VariableSymbol(
    string Name,
    LangType Type,
    bool IsConst
);