using SAFTLang.AST;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private string GenerateType(LangType type)
    {
        return type.Kind switch
        {
            LangTypeKind.Int => "int",
            LangTypeKind.Bool => "bool",
            LangTypeKind.String => "const char*",
            LangTypeKind.Void => "void",
            LangTypeKind.Array => "saft_array",
            _ => throw new Exception($"Unknown type {type}")
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