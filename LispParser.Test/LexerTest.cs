using FluentAssertions;

namespace LispParser.Test;

public class LexerTest
{
    [TestCase("")]
    [TestCase("   ")]
    public void Parse_Empty_Strings(string input)
    {
        var output = Parse(input);
        output.Should().BeEquivalentTo(new List<Token>()
        {
            new (TokenType.EOF),
        },
        o => o.Excluding(t => t.Start).Excluding(t => t.End));
    }
    
    [TestCase("(+ 2 3)")]
    [TestCase("( +   2   3 ) ")]
    [TestCase("( + \t\n  2\n3\n)")]
    public void Parse_Simple_Expression(string input)
    {
        var output = Parse(input);
        output.Should().BeEquivalentTo(new List<Token>
            {
                new (TokenType.LeftParen),
                new (TokenType.Atom, Value: "+"),
                new (TokenType.Atom, Value: "2"),
                new (TokenType.Atom, Value: "3"),
                new (TokenType.RightParen),
                new (TokenType.EOF),
            },
            o => o.Excluding(t => t.Start).Excluding(t => t.End)
        );
    }
    
    [Test]
    public void Parse_Nested_Expression()
    {
        var output = Parse("(first (list 1 (+ 2 3) 9))");
        output.Should().BeEquivalentTo(new List<Token>
            {
                new (TokenType.LeftParen),
                new (TokenType.Atom, Value: "first"),
                new (TokenType.LeftParen),
                new (TokenType.Atom, Value: "list"),
                new (TokenType.Atom, Value: "1"),
                new (TokenType.LeftParen),
                new (TokenType.Atom, Value: "+"),
                new (TokenType.Atom, Value: "2"),
                new (TokenType.Atom, Value: "3"),
                new (TokenType.RightParen),
                new (TokenType.Atom, Value: "9"),
                new (TokenType.RightParen),
                new (TokenType.RightParen),
                new (TokenType.EOF),
            },
            o => o.Excluding(t => t.Start).Excluding(t => t.End)
        );
    }
    
    [Test]
    public void Parse_List_Of_String()
    {
        var output = Parse("(list \"fun chars ) (\\\" blah\")");
        output.Should().BeEquivalentTo(new List<Token>
            {
                new (TokenType.LeftParen),
                new (TokenType.Atom, Value: "list"),
                new (TokenType.String, Value: "fun chars ) (\\\" blah"),
                new (TokenType.RightParen),
                new (TokenType.EOF),
            },
            o => o.Excluding(t => t.Start).Excluding(t => t.End)
        );
    }

    private static List<Token> Parse(string input)
    {
        return new Lexer().Parse(input).ToList();
    }
}
