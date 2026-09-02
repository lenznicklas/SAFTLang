using SAFTLang.Lexer.TokenAndKeywords;
using SAFTLang.Lexer.Text;
using SAFTLang.AST;
using SAFTLang.AST.Expressions;
using SAFTLang.AST.Statements;
using SAFTLang.AST.Types;

namespace SAFTLang.Parser.ParseStatements;

internal sealed partial class StatementParser
{
    private Statement ParseConstStatement()
    {
        Token constToken = _state.Consume(TokenType.Const);

        Token name = _state.Consume(TokenType.Identifier);

        LangType? type = _typeParser.ParseOptionalType();
        
        _state.Consume(TokenType.Equal);

        Expr value = _expressionParser.ParseExpression();
        
        _state.ConsumeStatementEnd();
        
        SourceSpan span =  SourceSpan.Combine(constToken.Span, value.Span);
        
        return new ConstStatement(name.Value, type, value, span);
    }

}