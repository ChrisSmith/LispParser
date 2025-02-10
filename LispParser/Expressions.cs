using System.Collections;
using System.Text;

namespace LispParser;

public abstract record Expression
{
    
}

public record ListExpression(IReadOnlyList<Expression> Args) : Expression
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Args = [");
        builder.AppendJoin(", ", Args);
        builder.Append(']');
        return true;
    }
}

public record AtomString(string Literal) : Expression;

public record AtomInteger(int Literal) : Expression;

public record AtomIdentifier(string Identifier) : Expression;
