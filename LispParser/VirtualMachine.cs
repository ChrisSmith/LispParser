namespace LispParser;

public class VirtualMachine
{
    private readonly IDictionary<string, Func<List<Expression>, object>> _functions = new Dictionary<string, Func<List<Expression>, object>> { };

    public VirtualMachine()
    {
        _functions.Add("+", Add);
    }

    public object Execute(Expression expression)
    {
        if (expression is ListExpression listExp)
        {
            return ExecuteListExpression(listExp);
        }
        if (expression is AtomIdentifier ident)
        {
            if (_functions.TryGetValue(ident.Identifier, out var func))
            {
                return func;
            }
        }

        if (expression is AtomInteger integer)
        {
            return integer.Literal;
        }

        throw new Exception("Failed to execute expression");
    }

    private object ExecuteListExpression(ListExpression listExp)
    {
        if (listExp.Args.Count > 0)
        {
            // assume first arg is method
            var res = Execute(listExp.Args[0]);
            if (res is Func<List<Expression>, object> func)
            {
                var args = listExp.Args.Skip(1).ToList();
                return func(args);
            }
        }

        throw new Exception("Failed to execute list expression");
    }

    // (+ (+ 1 2) (+ 1 3))
    private object Add(List<Expression> expressions)
    {
        var res = 0;
        foreach (var exp in expressions)
        {
            var curr = (int)Execute(exp);
            res += curr;
        }
        return res;
    }
}