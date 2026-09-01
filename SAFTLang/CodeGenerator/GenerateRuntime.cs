using System.Text;

namespace SAFTLang.CodeGenerator;

public partial class CodeGenerator
{
    private void GenerateRuntime(StringBuilder output)
    {
        output.AppendLine("typedef struct");
        output.AppendLine("{");
        output.AppendLine("    void* data;");
        output.AppendLine("    int length;");
        output.AppendLine("} saft_array;");
        output.AppendLine();

        output.AppendLine(
            "static saft_array saft_array_copy(" +
            "const void* source, size_t element_size, int length)"
        );

        output.AppendLine("{");
        
        output.AppendLine("    saft_array result = { NULL, length };");
        
        output.AppendLine();
        output.AppendLine("    if (length == 0)");
        output.AppendLine("    {");
        output.AppendLine("        return result;");
        output.AppendLine("    }");
        
        output.AppendLine();
        
        output.AppendLine("    result.data = malloc(element_size * (size_t)length);");
        
        output.AppendLine();

        output.AppendLine("    if (result.data == NULL)");
        output.AppendLine("    {");

        output.AppendLine(
            "        fprintf(stderr, " +
            "\"SAFT runtime error: out of memory\\n\");"
        );
        
        output.AppendLine("        exit(1);");
        output.AppendLine("    }");
        output.AppendLine();

        output.AppendLine(
            "    memcpy(" +
            "result.data, source, " +
            "element_size * (size_t)length);"
        );
        
        output.AppendLine();
        output.AppendLine("    return result;");
        output.AppendLine("}");
        output.AppendLine();
        
        output.AppendLine(
            "static void* saft_array_at(" +
            "saft_array array, int index, " +
            "size_t element_size)"
        );

        output.AppendLine("{");

        output.AppendLine(
            "    if (index < 0 || index >= array.length)"
        );

        output.AppendLine("    {");

        output.AppendLine(
            "        fprintf(stderr, " +
            "\"SAFT runtime error: array index out of bounds\\n\");"
        );

        output.AppendLine(
            "        exit(1);"
        );

        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    return (char*)array.data + " +
            "(size_t)index * element_size;"
        );

        output.AppendLine("}");
        output.AppendLine();
    }
}