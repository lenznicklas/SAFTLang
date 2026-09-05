using SAFTLang.AST.Statements;

namespace SAFTLang.SemanticAnalyzer.ControlFlow;

internal sealed class ControlFlowAnalyzer
{
    public bool AlwaysReturns(
        Statement statement)
    {
        return statement switch
        {
            ReturnStatement =>
                true,

            BlockStatement block =>
                BlockAlwaysReturns(block),

            IfStatement ifStatement =>
                ifStatement.elseBody is not null &&
                AlwaysReturns(ifStatement.thenBody) &&
                AlwaysReturns(ifStatement.elseBody),

            ForStatement forStatement =>
                ForAlwaysReturns(forStatement),

            _ =>
                false
        };
    }    
    
    private bool ForAlwaysReturns(
        ForStatement statement)
    {
        if (statement.Condition is not null)
        {
            return false;
        }

        return AlwaysReturns(
            statement.Body
        );
    }

    private bool BlockAlwaysReturns(
        BlockStatement block)
    {
        foreach (Statement statement
                 in block.Statements)
        {
            if (AlwaysReturns(statement))
            {
                return true;
            }
        }

        return false;
    }
}