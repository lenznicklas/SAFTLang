using System.Text;

namespace SAFTLang.CodeGenerator.Runtime;

internal static class RuntimeGenerator
{
    public static void GenerateRuntime(
        StringBuilder output)
    {
        // Equality function type
        output.AppendLine(
            "typedef bool (*saft_equals_fn)(" +
            "const void* left, const void* right);"
        );
        output.AppendLine();

        // Array type
        output.AppendLine("typedef struct");
        output.AppendLine("{");
        output.AppendLine("    void* data;");
        output.AppendLine("    int length;");
        output.AppendLine("    size_t element_size;");
        output.AppendLine("    saft_equals_fn equals;");
        output.AppendLine("} saft_array;");
        output.AppendLine();

        // Forward declaration for recursive array equality
        output.AppendLine(
            "static bool saft_array_equal(" +
            "saft_array left, saft_array right);"
        );
        output.AppendLine();

        // Int equality
        output.AppendLine(
            "static bool saft_equal_int(" +
            "const void* left, const void* right)"
        );
        output.AppendLine("{");
        output.AppendLine(
            "    return *(const int*)left == " +
            "*(const int*)right;"
        );
        output.AppendLine("}");
        output.AppendLine();

        // Bool equality
        output.AppendLine(
            "static bool saft_equal_bool(" +
            "const void* left, const void* right)"
        );
        output.AppendLine("{");
        output.AppendLine(
            "    return *(const bool*)left == " +
            "*(const bool*)right;"
        );
        output.AppendLine("}");
        output.AppendLine();

        // String equality
        output.AppendLine(
            "static bool saft_equal_string(" +
            "const void* left, const void* right)"
        );
        output.AppendLine("{");

        output.AppendLine(
            "    const char* left_value = " +
            "*(const char* const*)left;"
        );

        output.AppendLine(
            "    const char* right_value = " +
            "*(const char* const*)right;"
        );

        output.AppendLine();

        output.AppendLine(
            "    return strcmp(" +
            "left_value, right_value) == 0;"
        );

        output.AppendLine("}");
        output.AppendLine();

        // Nested array equality
        output.AppendLine(
            "static bool saft_equal_array(" +
            "const void* left, const void* right)"
        );
        output.AppendLine("{");

        output.AppendLine(
            "    return saft_array_equal("
        );

        output.AppendLine(
            "        *(const saft_array*)left,"
        );

        output.AppendLine(
            "        *(const saft_array*)right"
        );

        output.AppendLine(
            "    );"
        );

        output.AppendLine("}");
        output.AppendLine();

        // Array copy
        output.AppendLine(
            "static saft_array saft_array_copy(" +
            "const void* source, " +
            "size_t element_size, " +
            "int length, " +
            "saft_equals_fn equals)"
        );

        output.AppendLine("{");

        output.AppendLine(
            "    saft_array result = " +
            "{ NULL, length, element_size, equals };"
        );

        output.AppendLine();

        output.AppendLine(
            "    if (length == 0)"
        );

        output.AppendLine("    {");
        output.AppendLine(
            "        return result;"
        );
        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    result.data = malloc(" +
            "element_size * (size_t)length);"
        );

        output.AppendLine();

        output.AppendLine(
            "    if (result.data == NULL)"
        );

        output.AppendLine("    {");

        output.AppendLine(
            "        fprintf(stderr, " +
            "\"SAFT runtime error: out of memory\\n\");"
        );

        output.AppendLine(
            "        exit(1);"
        );

        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    memcpy(" +
            "result.data, source, " +
            "element_size * (size_t)length);"
        );

        output.AppendLine();

        output.AppendLine(
            "    return result;"
        );

        output.AppendLine("}");
        output.AppendLine();

        // Array indexing
        output.AppendLine(
            "static void* saft_array_at(" +
            "saft_array array, int index)"
        );

        output.AppendLine("{");

        output.AppendLine(
            "    if (index < 0 || " +
            "index >= array.length)"
        );

        output.AppendLine("    {");

        output.AppendLine(
            "        fprintf(stderr, " +
            "\"SAFT runtime error: " +
            "array index out of bounds\\n\");"
        );

        output.AppendLine(
            "        exit(1);"
        );

        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    return (char*)array.data + " +
            "(size_t)index * array.element_size;"
        );

        output.AppendLine("}");
        output.AppendLine();

        // Array equality
        output.AppendLine(
            "static bool saft_array_equal(" +
            "saft_array left, saft_array right)"
        );

        output.AppendLine("{");

        output.AppendLine(
            "    if (left.length != right.length)"
        );

        output.AppendLine("    {");
        output.AppendLine(
            "        return false;"
        );
        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    if (left.element_size != " +
            "right.element_size)"
        );

        output.AppendLine("    {");
        output.AppendLine(
            "        return false;"
        );
        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    if (left.equals != right.equals)"
        );

        output.AppendLine("    {");
        output.AppendLine(
            "        return false;"
        );
        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    for (int i = 0; " +
            "i < left.length; i++)"
        );

        output.AppendLine("    {");

        output.AppendLine(
            "        const void* left_element ="
        );

        output.AppendLine(
            "            (const char*)left.data +"
        );

        output.AppendLine(
            "            (size_t)i * " +
            "left.element_size;"
        );

        output.AppendLine();

        output.AppendLine(
            "        const void* right_element ="
        );

        output.AppendLine(
            "            (const char*)right.data +"
        );

        output.AppendLine(
            "            (size_t)i * " +
            "right.element_size;"
        );

        output.AppendLine();

        output.AppendLine(
            "        if (!left.equals("
        );

        output.AppendLine(
            "                left_element,"
        );

        output.AppendLine(
            "                right_element))"
        );

        output.AppendLine("        {");

        output.AppendLine(
            "            return false;"
        );

        output.AppendLine("        }");

        output.AppendLine("    }");

        output.AppendLine();

        output.AppendLine(
            "    return true;"
        );

        output.AppendLine("}");
        output.AppendLine();
    }
}