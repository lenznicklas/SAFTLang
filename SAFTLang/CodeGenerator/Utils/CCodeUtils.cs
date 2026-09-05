namespace SAFTLang.CodeGenerator.Utils;

internal static class CCodeUtils
{
    public static string EscapeCString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r");
    }

    public static string GenerateVariableIdentifier(string name)
    {
        return $"saft_v_{name}";
    }
    
    public static string GenerateFunctionIdentifier(string name)
    {
        return $"saft_f_{name}";
    }

    public static string EscapeCChar(char value)
    {
        return value switch
        {
            '\\' => "\\\\",
            '\'' => "\\'",
            '\n' => "\\n",
            '\t' => "\\t",
            '\r' => "\\r",
            '\0' => "\\0",

            _ => value.ToString()
        };
    }
}