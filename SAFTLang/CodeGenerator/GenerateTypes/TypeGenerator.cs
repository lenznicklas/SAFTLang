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
            LangTypeKind.Char => "char",
            LangTypeKind.Void => "void",
            LangTypeKind.Array => "saft_array",
            _ => throw new InvalidOperationException($"Unknown type {type}")
        };
    }

    public string GenerateEqualityFunction(LangType type)
    {
        return type.Kind switch
        {
            LangTypeKind.Int => "saft_equal_int",
            LangTypeKind.Bool => "saft_equal_bool",
            LangTypeKind.String => "saft_equal_string",
            LangTypeKind.Char => "saft_equal_char",
            LangTypeKind.Array => "saft_equal_array",

            _ => throw new InvalidOperationException($"Type {type} is not comparable")
        };
    }

}