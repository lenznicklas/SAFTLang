using SAFTLang.AST;

namespace SAFTLang.SemanticAnalyzer.Symbols;

public record VariableSymbol(string Name, LangType Type, bool IsConst);