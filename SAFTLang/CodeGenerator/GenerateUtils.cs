using SAFTLang.AST;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private string GenerateType(LangType type)
    {
        return type switch
        {
            LangType.Int => "int",
            LangType.Bool => "bool",
            LangType.String => "const char*",
            _ => throw new Exception($"Unknown type {type.GetType().Name}")
        };
    }

    private string EscapeCString(string str)
    {
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private string GenerateIdentifier(string name)
    {
        return $"saft_{name}";
    }

}