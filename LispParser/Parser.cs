namespace LispParser;

public class Parser
{
    private int _position;
    private IReadOnlyList<Token> _tokens;
    
    public Expression Parse(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
        var result = ParseExpression();

        Consume(TokenType.EOF);
        if (HasNext())
        {
            throw new ParseException(Peek(),
                $"Expected to consume all tokens in the input, but there are still {_tokens.Count - _position} remaining. Next {Peek()}");
        }
        
        return result;
    }

    private Expression ParseExpression()
    {
        var token = Peek();
        
        if (token.Type == TokenType.LeftParen)
        {
            Consume(TokenType.LeftParen);
            var listExp = new ListExpression(ParseArgsList());
            Consume(TokenType.RightParen);
            return listExp;
        }

        if (token.Type == TokenType.String)
        {
            var value = Consume().Value ?? throw new ParseException(token, "string token must have a value"); 
            return new AtomString(value);
        }

        if (token.Type == TokenType.Atom)
        {
            return ConsumeAtom();
        }

        throw new ParseException(token, $"Expected to find an expression. Found unexpected token {token.Type}");
    }

    private Expression ConsumeAtom()
    {
        var token = Consume(TokenType.Atom);
        if (string.IsNullOrWhiteSpace(token.Value))
        {
            throw new ParseException(token, "atom token must have a value");
        }

        var value = token.Value; 
        if (char.IsNumber(value[0]))
        {
            if (int.TryParse(value, out var intValue))
            {
                return new AtomInteger(intValue);
            }
            throw new ParseException(token, $"failed to parse '{value}' as an integer");
        }
        
        return new AtomIdentifier(value);
    }

    private List<Expression> ParseArgsList()
    {
        var list = new List<Expression>();
        while (HasNext() && !Match(TokenType.RightParen))
        {
            list.Add(ParseExpression());
        }

        return list;
    }

    private Token Peek()
    {
        return _tokens[_position];
    }

    private Token Consume(TokenType expected)
    {
        var token = Consume();
        if (token.Type != expected)
        {
            throw new ParseException(token, $"Expected token type {expected} but found {token.Type}");
        }
        return token;
    }

    private bool Match(TokenType expected)
    {
        if (!HasNext())
        {
            return false;
        }

        return Peek().Type == expected;
    }
    
    private Token Consume()
    {
        if (!HasNext())
        {
            throw new ParseException("Tried to consume token past input length");
        }
        var c = _tokens[_position];
        _position++;
        return c;
    }
    
    private bool HasNext()
    {
        return _position < _tokens.Count;
    }
    
}

public class ParseException : Exception
{
    public ParseException(Token token, string message)
        : base($"{message} at [{token.Start}:{token.End}]") { } 
    
    public ParseException(string message): base(message) { }
}
