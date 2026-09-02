using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.SemanticAnalyzer.Symbols;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeAssignmentStatement(AssignmentStatement statement)
    {
        LangType targetType;

        switch (statement.Target)
        {
            case IdentifierExpr identifier:
            {
                VariableSymbol? symbol = _state.ResolveVariable(identifier.Name, identifier.Span);

                if (symbol is null)
                {
                    return;
                }

                if (symbol.IsConst)
                {
                    _diagnostics.ReportError(
                        statement.Target.Span,
                        $"Cannot assign to const variable '{identifier.Name}'"
                    );
                    return;
                }

                targetType = symbol.Type;
                break;
            }

            case IndexExpr index:
            {
                targetType = _expressionAnalyzer.AnalyzeExpression(index);

                if (targetType == LangType.Error)
                {
                    return;
                }

                IdentifierExpr? root = GetRootIdentifier(index);

                if (root is not null)
                {
                    VariableSymbol? symbol = _state.ResolveVariable(root.Name, root.Span);

                    if (symbol?.IsConst == true)
                    {
                        _diagnostics.ReportError(
                            statement.Target.Span,
                            $"Cannot modify const array '{root.Name}'"
                        );
                        
                        return;
                    }
                }
                
                break;
            }

            default:
            {
                _diagnostics.ReportError(
                    statement.Target.Span,
                    "Left side of assignment is not assignable"
                );
                return;
            }
        }

        LangType valueType = _expressionAnalyzer.AnalyzeExpression(statement.Value, targetType);

        if (valueType == LangType.Error)
        {
            return;
        }

        if (valueType != targetType)
        {
            _diagnostics.ReportError(
                statement.Value.Span,
                $"Cannot assign {valueType} to {targetType}"
            );
        }

    }

}