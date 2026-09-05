using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.SemanticAnalyzer.AnalyzeStatements;

internal sealed partial class StatementAnalyzer
{
    private void AnalyzeConstStatement(ConstStatement statement)
    {
        LangType? declaredType = statement.DeclaredType;
        LangType valueType = _expressionAnalyzer.AnalyzeExpression(statement.Value, declaredType);

        if (valueType == LangType.Error)
        {
            return;
        }
        
        if (valueType == LangType.Void)
        {
            _diagnostics.ReportError(statement.Span, "Cannot assign type Void to a variable");
            return;
        }

        if (declaredType is null || declaredType == valueType)
        {
            _state.DeclareVariable(statement.Name, valueType, isConst: true, statement.Span);
            _state.SetStatementType(statement,valueType);
        }
        else if (declaredType != valueType)
        {
            _diagnostics.ReportError(statement.Span, $"Expected {declaredType} but got {valueType}");
        }
    }

}