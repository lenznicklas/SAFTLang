namespace SAFTLang.AST.Types;

public sealed record LangType(LangTypeKind Kind, LangType? ElementType = null)
{
    public static readonly LangType Int = new LangType(LangTypeKind.Int);
    public static readonly LangType Bool = new LangType(LangTypeKind.Bool);
    public static readonly LangType String = new LangType(LangTypeKind.String);
    public static readonly LangType Void = new LangType(LangTypeKind.Void);
    public static readonly LangType Error = new LangType(LangTypeKind.Error);

    public static LangType ArrayOf(LangType elementType)
    {
        return new LangType(LangTypeKind.Array, elementType);
    }
    
    public bool IsArray => Kind == LangTypeKind.Array;
    
    public override string ToString()
    {
        return Kind switch
        {
            LangTypeKind.Int => "int",
            LangTypeKind.Bool => "bool",
            LangTypeKind.String => "string",
            LangTypeKind.Void => "void",
            LangTypeKind.Array => $"{ElementType}[]",
            LangTypeKind.Error => "<error>",

            _ => "<unknown>"
        };
    }
}