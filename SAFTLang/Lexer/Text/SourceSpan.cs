namespace SAFTLang.Lexer.Text;

public record SourceSpan(
    int Start, 
    int Length, 
    int Line, 
    int Column)
{
    public int End => Start + Length;

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }
}
