namespace SAFTLang.CodeGenerator.Utils;

internal static class CCodeUtils
{
    public static string EscapeCString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    public static string GenerateIdentifier(string name)
    {
        return $"saft_{name}";
    }

}