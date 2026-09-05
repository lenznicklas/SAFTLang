namespace SAFTLang.Modules;

public record ImportBinding(
    string LocalName,
    IReadOnlyList<string> ModulePath,
    string? MemberName
)
{
    public string ModuleName => string.Join("::", ModulePath);
    public bool IsMemberImport => MemberName is not null;
}