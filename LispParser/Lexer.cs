namespace LispParser;

public class Lexer
{
    private int _position;
    private string _input = null!;
    private int _start;
    
    public IEnumerable<Token> Parse(string input)
    {
        _position = 0;
        _input = input;

        while (HasNext())
        {
            // consume any leading whitespace
            while (HasNext() && char.IsWhiteSpace(Peek()))
            {
                Consume();
            }

            if (!HasNext())
            {
                break;
            }
            
            _start = _position;
            yield return ParseToken();
        }

        yield return new Token(TokenType.EOF, _start, _position);
    }

    private Token ParseToken()
    {
        var c = Peek();
        if (c == '"')
        {
            return ConsumeString();
        }
        if (c == '(')
        {
            Consume();
            return new Token(TokenType.LeftParen, _start, _position);
        }
        if (c == ')')
        {
            Consume();
            return new Token(TokenType.RightParen, _start, _position);
        }

        while (HasNext() && !MatchIsWhitespace() && !Match('(', ')'))
        {
            Consume();
        }

        var literal = _input[_start.._position];
        
        return new Token(TokenType.Atom,_start, _position, literal);
    }

    private bool Match(params char[] expected)
    {
        if (!HasNext())
        {
            return false;
        }

        var next = Peek();
        foreach(var c in expected)
        {
            if (next == c)
            {
                return true;
            }     
        }

        return false;
    }

    private bool MatchIsWhitespace()
    {
        if (!HasNext())
        {
            return false;
        }

        var next = Peek();
        return char.IsWhiteSpace(next);
    }
    
    private Token ConsumeString()
    {
        Consume('"');
        while (HasNext() && !Match('"'))
        {
            var c = Consume();
            // escape char, take the next char literally
            if (c == '\\' && HasNext())
            {
                Consume();
            }
        }
        Consume('"');
        
        var literal = _input[(_start+1)..(_position-1)];
        return new Token(TokenType.String, _start, _position, literal);
    }
    
    private char Peek()
    {
        return _input[_position];
    }
    
    private char Consume()
    {
        if (!HasNext())
        {
            throw new ParseException("Tried to consume token past input length");
        }
        var c = _input[_position];
        _position++;
        return c;
    }
    
    private void Consume(char expected)
    {
        if (!HasNext())
        {
            throw new ParseException($"Tried to consume token past input length. Expected '{expected}'");
        }
        var c = Consume();
        if (c != expected)
        {
            throw new ParseException($"Unexpected token. Expected '{expected}' but found '{c}' at {_position-1}");
        }
    }
    
    private bool HasNext()
    {
        return _position < _input.Length;
    }
}

public enum TokenType
{
    EOF,
    Atom,
    String,
    LeftParen,
    RightParen,
}

public record Token(TokenType Type, int Start = 0, int End = 0, string? Value = null);
