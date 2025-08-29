# Quaero

> A web-friendly query language that transforms into Microsoft Graph, Active Directory, and SCIM filters

[![NuGet](https://img.shields.io/nuget/v/Quaero.svg)](https://www.nuget.org/packages/Quaero)
[![Build Status](https://img.shields.io/github/actions/workflow/status/khellang/Quaero/ci.yml?branch=main)](https://github.com/khellang/Quaero/actions)

## What is Quaero?

Quaero is a small, intuitive query language designed to unify filtering across different systems.
Instead of learning multiple query syntaxes for Microsoft Graph, LDAP, and SCIM, you write queries once in Quaero and transform them into the target system's native filter format.

**Key Benefits:**
- **Universal syntax** - Write queries once, use everywhere
- **Web-friendly** - Uses readable operators like `eq`, `gt`, `and` instead of symbols
- **Type-safe** - Built for .NET with strong typing support
- **Optimizable** - Automatic query optimization removes redundancies

## Quick Start

### Installation

```bash
# Core library
dotnet add package Quaero

# Target-specific packages
dotnet add package Quaero.MicrosoftGraph
dotnet add package Quaero.Ldap  
dotnet add package Quaero.Scim
```

### Basic Usage

```csharp
using Quaero;

// Parse a filter from string
Filter filter = Filter.Parse("age gt 42 and department eq \"Engineering\"");

// Or build programmatically
Filter filter = Filter.And(
    Filter.GreaterThan("age", 42),
    Filter.Equals("department", "Engineering")
);

// Transform to target system
string microsoftGraph = filter.ToMicrosoftGraphFilter();
string ldapFilter = filter.ToLdapFilter(); 
string scimFilter = filter.ToScimFilter();

// Use in-memory
Func<Employee, bool> predicate = filter.ToPredicate<Employee>();
```

## Supported Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `eq` | Equals | `name eq "John"` |
| `ne` | Not equals | `status ne "inactive"` |
| `gt` | Greater than | `age gt 21` |
| `ge` | Greater than or equal | `salary ge 50000` |
| `lt` | Less than | `score lt 100` |
| `le` | Less than or equal | `rating le 5` |
| `sw` | Starts with | `email sw "admin"` |
| `ew` | Ends with | `domain ew ".com"` |
| `co` | Contains | `title co "Manager"` |
| `in` | In list | `status in ["active", "pending"]` |
| `pr` | Present (not null) | `phoneNumber pr` |
| `and` | Logical AND | `age gt 18 and verified eq true` |
| `or` | Logical OR | `role eq "admin" or role eq "moderator"` |
| `not` | Logical NOT | `not(status eq "deleted")` |

## Translation Examples

Here's how the same Quaero query translates across different systems:

| System | Query |
|-------|---------------------------------------------|
| **Quaero** | `age gt 42 and department eq "Engineering"` |
| **Microsoft Graph** | `age gt 42 and department eq 'Engineering'` |
| **LDAP** | `(&(age>=43)(department=Engineering))` |
| **SCIM** | `age gt 42 and department eq "Engineering"` |

### Complex Query Example

```csharp
// Quaero query
string query = @"(department in [""Sales"", ""Marketing""] and salary ge 75000) 
                 or (role eq ""Senior Developer"" and experience gt 5)";

Filter filter = Filter.Parse(query);

// Microsoft Graph: ((department in ('Sales', 'Marketing') and salary ge 75000) or (role eq 'Senior Developer' and experience gt 5))
// LDAP: (|((&(|(department=Sales)(department=Marketing))(salary>=75000))(&(role=Senior Developer)(experience>=6))))
// SCIM: (department in ["Sales", "Marketing"] and salary ge 75000) or (role eq "Senior Developer" and experience gt 5)
```

## Advanced Features

### Query Optimization

```csharp
Filter filter = Filter.Parse("name eq \"John\" and name eq \"John\"");
Filter optimized = filter.Optimize(); // Removes redundant conditions
```

### In-Memory Evaluation

```csharp
record Employee(string Name, int Age, string Department);

var employees = new List<Employee> 
{
    new("Alice", 30, "Engineering"),
    new("Bob", 45, "Sales"),
    new("Carol", 28, "Engineering")
};

Filter filter = Filter.Parse("age gt 35 or department eq \"Engineering\"");
Func<Employee, bool> predicate = filter.ToPredicate<Employee>();

var results = employees.Where(predicate).ToList();
// Returns: Alice, Bob, Carol
```

## Error Handling

```csharp
// Safe parsing
if (Filter.TryParse("invalid query", out Filter? filter))
{
    // Success
    string result = filter.ToMicrosoftGraphFilter();
}
else
{
    // Handle parse error
    Console.WriteLine("Invalid query syntax");
}
```

## Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=khellang&utm_medium=Quaero) and [Dapper Plus](https://dapper-plus.net/?utm_source=khellang&utm_medium=Quaero) are major sponsors and proud to contribute to the development of Quaero.

[![Entity Framework Extensions](https://raw.githubusercontent.com/khellang/khellang/refs/heads/master/.github/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=khellang&utm_medium=Quaero)

[![Dapper Plus](https://raw.githubusercontent.com/khellang/khellang/refs/heads/master/.github/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=khellang&utm_medium=Quaero)
