using LispParser;

var lexer = new Lexer();
var parser = new Parser();

Console.WriteLine("Enter a single line program");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    try
    {
        if (input != null)
        {
            var tokens = lexer.Parse(input).ToList();
            var expression = parser.Parse(tokens);
            Console.WriteLine(expression);
        }
    }
    catch (ParseException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

