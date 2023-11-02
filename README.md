# Quaero

A small query language that can transform queries into system-specific filters.

Quaero is designed to be web-friendly by using letter-based operators instead of symbols. It's very similar to Microsoft Graph (OData) filters, with a few minor differences:
1. Infix `not` operators
2. Infix function operators

See [example queries](#example-queries) for some examples.

This repository contains translation implementations for Microsoft Graph and Active Directory (LDAP) filters, as well as in-memory predicates.

## Supported operators

The language supports a wide range of operators supported by most filter/query languages.

### Equality operators

- Equals (`eq`)
- Not equals (`ne`)
- Logical negation (`not`)
- In (`in`)

### Relational operators

- Less than (`lt`)
- Greater than (`gt`)
- Less than or equal to (`le`)
- Greater than or equal to (`ge`)

### Conditional operators

- And (`and`)
- Or (`or`)

### Functions

- Starts with (`startsWith`)
- Ends with (`endsWith`)

## Example queries

| Quaero                                         | Microsoft Graph                                | LDAP                                                    |
|------------------------------------------------|------------------------------------------------|---------------------------------------------------------|
| `age gt 42`                                    | `age gt 42`                                    | `(age>=43)`                                             |
| `age ge 42`                                    | `age ge 42`                                    | `(age>=42)`                                             |
| `age lt 42`                                    | `age lt 42`                                    | `(age<=41)`                                             |
| `age le 42`                                    | `age le 42`                                    | `(age<=42)`                                             |
| `name eq 'John'`                               | `name eq 'John'`                               | `(name=John)`                                           |
| `name not eq 'John'`                           | `not(name eq 'John')'`                         | `(!(name=John))`                                        |
| `department in ('Retail', 'Sales')`            | `department in ('Retail', 'Sales')`            | `(\|(department=Retail)(department=Sales))`             |
| `isRead eq false`                              | `isRead eq false`                              | `(isRead=FALSE)`                                        |
| `mail endsWith 'outlook.com'`                  | `endsWith(mail, 'outlook.com')`                | `(mail=*outlook.com)`                                   |
| `parent ne null`                               | `parent ne null`                               | `(parent=*)`                                            |
| `id eq '275e50ae-ceb8-4f33-9e68-b3b9dc87ea68'` | `id eq '275e50ae-ceb8-4f33-9e68-b3b9dc87ea68'` | `(id=\AE\50\5E\27\B8\CE\33\4F\9E\68\B3\B9\DC\87\EA\68)` |

## Usage

Either parse a filter expression from a string using `Filter.Parse` or `Filter.TryParse`:

```csharp
Filter filter = Filter.Parse("age gt 42");
```

Or manually construct a filter expression using the factory methods on the `Filter` class:

```csharp
Filter filter = Filter.GreaterThan("age", 42);
```

Once the filter has been successfully parsed, it can be optimized by calling the `Optimize` extension method. This will take care of removing redundancies and shortening the resulting query.

To transform the query into an LDAP filter, reference the `Quaero.Ldap` package and call the `ToLdapFilter` method on it:

```csharp
string result = filter.ToLdapFilter();
```

For Microsoft Graph, reference the `Quaero.MicrosoftGraph` package and call `ToMicrosoftGraphFilter` on the filter:

```csharp
string result = filter.ToMicrosoftGraphFilter();
```

If you want to evaluate a filter in-memory, you can call the `ToPredicate<T>` method on it:

```csharp
record Person(string Name, int Age);
Func<Person, bool> predicate = filter.ToPredicate<Person>();
```
