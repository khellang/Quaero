using Quaero;

Console.Write("query> ");
var line = Console.ReadLine();

while (line != null)
{
    if (!string.IsNullOrEmpty(line))
    {
        var filter = Filter.Parse(line).Optimize();

        Console.WriteLine($"Graph: {filter.ToMicrosoftGraphQuery()}");
        Console.WriteLine($"LDAP: {filter.ToLdapQuery()}");
    }

    Console.WriteLine();
    Console.Write("query> ");
    line = Console.ReadLine();
}
