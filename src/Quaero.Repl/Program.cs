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

            AnsiConsole.MarkupLineInterpolated($"Graph: [blue]{filter.ToMicrosoftGraphQuery()}[/]");
            AnsiConsole.MarkupLineInterpolated($"LDAP: [purple]{filter.ToLdapQuery()}[/]");
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
