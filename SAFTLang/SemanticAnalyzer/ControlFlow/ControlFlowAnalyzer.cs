using SAFTLang.AST.Statements;

namespace SAFTLang.SemanticAnalyzer.ControlFlow;

internal sealed class ControlFlowAnalyzer
{
    public bool AlwaysReturns(Statement statement)
    {
        return statement switch
        {
            ReturnStatement => true,
            BlockStatement block =>
                BlockAlwaysReturns(block),

            IfStatement ifStatement =>
                ifStatement.elseBody is not null &&
                AlwaysReturns(ifStatement.thenBody) &&
                AlwaysReturns(ifStatement.elseBody),

            _ => false
        };
    }

    public bool BlockAlwaysReturns(BlockStatement block)
    {
        foreach (Statement statement in block.Statements)
        {
            if (AlwaysReturns(statement))
            {
                return true;
            }
        }

        return false;
    }
}