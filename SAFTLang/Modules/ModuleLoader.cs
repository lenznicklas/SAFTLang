using SAFTLang.AST.Statements;
using SAFTLang.Diagnostics;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Modules;

internal sealed class ModuleLoader
{
    private readonly string _projectRoot;

    private readonly DiagnosticBag _diagnostics = new();

    private readonly Dictionary<string, Module> _loadedModules = new(StringComparer.Ordinal);
    
    private readonly HashSet<string> _loadingModules = new(StringComparer.Ordinal);

    private readonly List<Module> _moduleOrder = [];

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics.Diagnostics;
    
    public bool HasErrors => _diagnostics.HasErrors;

    public IReadOnlyList<Module> Modules => _moduleOrder;

    public ModuleLoader(string projectRoot)
    {
        _projectRoot = Path.GetFullPath(projectRoot);
    }

    public Module? LoadEntry(string sourcePath)
    {
        string fullSourcePath = Path.GetFullPath(sourcePath);

        string moduleName = Path.GetFileNameWithoutExtension(fullSourcePath);

        return LoadModule(["project", moduleName], fullSourcePath, null);
    }

    private Module? LoadProjectModule(IReadOnlyList<string> modulePath, SourceSpan importSpan)
    {
        if (modulePath.Count < 2)
        {
            _diagnostics.ReportError(importSpan, "Project import must specify a module");
            return null;
        }

        string relativePath = Path.Combine(modulePath.Skip(1).ToArray()) + ".sft";

        string filePath = Path.GetFullPath(Path.Combine(_projectRoot, relativePath));
        
        return LoadModule(modulePath, filePath, importSpan);
    }

    private Module? LoadModule(IReadOnlyList<string> modulePath, string filePath, SourceSpan? importSpan)
    {
        string moduleName = string.Join("::", modulePath);

        if (_loadedModules.TryGetValue(moduleName, out Module? existing))
        {
            return existing;
        }

        if (!_loadingModules.Add(moduleName))
        {
            if (importSpan is not null)
            {
                _diagnostics.ReportError(importSpan, $"Circular import involving '{moduleName}'");
            }
            return null;
        }

        try
        {
            if (!File.Exists(filePath))
            {
                if (importSpan is not null)
                {
                    _diagnostics.ReportError(importSpan, $"File '{filePath}' does not exist");
                }

                return null;
            }

            string source;

            try
            {
                source = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                if (importSpan is not null)
                {
                    _diagnostics.ReportError(importSpan, $"Could not read module '{moduleName}': {e.Message}");
                }

                return null;
            }

            var lexer = new Lexer.Lexer(source);
            var tokens = lexer.Tokenize();
            _diagnostics.AddRange(lexer.Diagnostics);

            if (lexer.HasErrors)
            {
                return null;
            }

            var parser = new Parser.Parser(tokens);

            List<Statement> statements = parser.Parse();

            _diagnostics.AddRange(parser.Diagnostics);

            if (parser.HasErrors)
            {
                return null;
            }

            var module = new Module(modulePath.ToArray(), statements);
            var importBinder = new ImportBinder(_diagnostics);

            importBinder.BindImports(module);

            bool importsLoaded = true;

            foreach (ImportStatement import in statements.OfType<ImportStatement>())
            {
                if (import.Path.Count == 0)
                {
                    continue;
                }

                string root = import.Path[0];

                switch (root)
                {
                    case "project":
                    {
                        Module? importedModule = LoadProjectModule(import.Path, import.Span);

                        if (importedModule is null)
                        {
                            importsLoaded = false;
                        }

                        break;
                    }

                    case "std":
                        break;

                    default:
                        _diagnostics.ReportError(import.Span, $"Unknown import root '{root}'");
                        importsLoaded = false;
                        break;
                }
            }

            if (!importsLoaded)
            {
                return null;
            }

            _loadedModules.Add(moduleName, module);

            _moduleOrder.Add(module);

            return module;
        }
        finally
        {
            _loadingModules.Remove(moduleName);
        }
    }
}