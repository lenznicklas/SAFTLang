namespace SAFTLang.Modules;

public sealed class Module
{
    public IReadOnlyList<string> Path  { get; }
    public IReadOnlyList<string> Statements { get; }
    public List<ImportBinding> Imports { get; } = [];

    public string Name => Path[^1];
    
    public string FullName => string.Join("::", Path);

    public Module(IReadOnlyList<string> path, IReadOnlyList<string> statements)
    {
        if (path.Count == 0)
        {
            throw new ArgumentException("Module path must not be empty");
        }
        
        Path = path;
        Statements = statements;
    }
}