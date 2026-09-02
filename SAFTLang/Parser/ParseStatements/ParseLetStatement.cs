using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.AST;
using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;
using SAFTLang.Lexer.Text;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseLetStatement()
    {
        Token letToken = _state.Consume(TokenType.Let);
        
        Token name = _state.Consume(TokenType.Identifier);

        LangType? type = _typeParser.ParseOptionalType();

        _state.Consume(TokenType.Equal);

        Expr value = _expressionParser.ParseExpression();

        _state.ConsumeStatementEnd();

        SourceSpan span = SourceSpan.Combine(letToken.Span, value.Span);

        return new LetStatement(name.Value, type, value, span);
    }

}