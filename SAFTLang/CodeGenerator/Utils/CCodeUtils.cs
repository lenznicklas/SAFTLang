namespace SAFTLang.CodeGenerator.Utils;

internal static class CCodeUtils
{
    public static string EscapeCString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    public static string GenerateVariableIdentifier(string name)
    {
        return $"saft_v_{name}";
    }
    
    public static string GenerateFunctionIdentifier(string name)
    {
        return $"saft_f_{name}";
    }

    public static string GenerateIdentifier(string name)
    {
        return $"saft_i_{name}";
    }
}