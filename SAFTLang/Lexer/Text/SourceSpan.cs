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

    public static SourceSpan Combine(
        SourceSpan first,
        SourceSpan last)
    {
        int start = first.Start;
        int end = last.End;
        
        return new SourceSpan(start, end - start, first.Line,first.Column);
    }
}
