using ExpressionTreeToString;
using Quaero;
using Spectre.Console;
using Superpower;

const string prompt = "query> ";

Console.Write(prompt);
var line = Console.ReadLine();

while (line != null)
{
    if (!string.IsNullOrEmpty(line))
    {
        try
        {
            var filter = Filter.Parse(line).Optimize();

            AnsiConsole.MarkupLineInterpolated($"Graph: [blue]{filter.ToMicrosoftGraphFilter()}[/]");
            AnsiConsole.MarkupLineInterpolated($"LDAP: [purple]{filter.ToLdapFilter()}[/]");
            AnsiConsole.MarkupLineInterpolated($"SCIM: [green]{filter.ToScimFilter()}[/]");
            AnsiConsole.MarkupLineInterpolated($"Expression: [cyan]{filter.ToExpression<Person>().ToString(BuiltinRenderer.CSharp)}[/]");
        }
        catch (ParseException e)
        {
            Console.Write(new string(' ', prompt.Length + e.ErrorPosition.Column - 1));
            AnsiConsole.MarkupLine("[red]↑[/]");
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message}[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{e.Message}[/]");
        }
    }

    Console.WriteLine();
    Console.Write(prompt);
    line = Console.ReadLine();
}

public record Person(string GivenName, string FamilyName, int Age);
