using SAFTLang.AST.Statements;
using SAFTLang.Diagnostics;

namespace SAFTLang.Modules;

internal sealed class ImportBinder
{
    private readonly DiagnosticBag _diagnostics;

    public ImportBinder(DiagnosticBag diagnostics)
    {
        _diagnostics = diagnostics;
    }

    public void BindImports(Module module)
    {
        foreach (ImportStatement import in module.Statements.OfType<ImportStatement>())
        {
            BindImport(module, import);
        }
    }

    private void BindImport(Module module, ImportStatement import)
    {
        if (import.Members is not null)
        {
            BindMembers(module, import);
            return;
        }

        string localName = import.Alias ?? import.Path[^1];
        
        AddBinding(module, new ImportBinding(localName, import.Path, null), import);
    }

    private void BindMembers(Module module, ImportStatement import)
    {
        if (import.Alias is not null)
        {
            _diagnostics.ReportError(import.Span, "Member imports cannot have an alias");
            return;
        }

        foreach (string member in import.Members!)
        {
            AddBinding(module, new ImportBinding(member, import.Path, member), import);
        }
    }

    private void AddBinding(Module module, ImportBinding binding, ImportStatement import)
    {
        if (module.Imports.Any(existing =>
                existing.LocalName == binding.LocalName))
        {
            _diagnostics.ReportError(
                import.Span,
                $"Import name '{binding.LocalName}' is already defined"
            );
            return;
        }
        
        module.Imports.Add(binding); 
    }
}