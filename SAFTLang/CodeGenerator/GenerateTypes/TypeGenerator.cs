using SAFTLang.AST.Types;

namespace SAFTLang.CodeGenerator.GenerateTypes;

internal sealed class TypeGenerator
{
    public string GenerateType(LangType type)
    {
        return type.Kind switch
        {
            LangTypeKind.Int => "int",
            LangTypeKind.Bool => "bool",
            LangTypeKind.String => "const char*",
            LangTypeKind.Void => "void",
            LangTypeKind.Array => "saft_array",
            _ => throw new InvalidOperationException($"Unknown type {type}")
        };
    }

}