using FluentAssertions;

namespace LispParser.Test;

public class ParserTest
{
    [Test]
    public Task Parse_Simple_Expression()
    {
        var output = Parse("(+ 2 3)");
        return Verify(output);
    }
    
    [Test]
    public Task Parse_Nested_Expression()
    {
        var output = Parse("(first (list 1 (+ 2 3) 9))");
        return Verify(output);
    }
    
    [Test]
    public void Parse_Atom()
    {
        var output = Parse("1");
        output.Should().BeOfType<AtomInteger>().Which.Literal.Should().Be(1);
    }
    
    [Test]
    public void Parse_String()
    {
        var output = Parse("\"hello world\"");
        output.Should().BeOfType<AtomString>().Which.Literal.Should().Be("hello world");
    }

    [TestCase("(list 2a)", "failed to parse '2a' as an integer at [6:8]")]
    [TestCase("(+ 2 3", "Expected to find an expression. Found unexpected token EOF at [5:6]")]
    [TestCase("(+ \" )", "Tried to consume token past input length. Expected '\"'")]
    public void Parse_ThrowsException(string input, string expectedMessage)
    {
        Action f = () => Parse(input);
        f.Should().Throw<ParseException>().WithMessage(expectedMessage);
    }
    
    private Expression Parse(string input)
    {
        return new Parser().Parse(new Lexer().Parse(input).ToList());
    }
}
