# C# Coding Style Guidelines

This document describes the preferred C# coding style in our projects. It is based on the [.NET Runtime Coding Style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) with some targeted modifications to improve readability and maintainability of the codebases, as well as to reduce potential for bugs being overlooked in code review.

The goal of this document is to provide a modern and consistent coding style across the codebase, reducing boilerplate where possible, while considering the readability and maintainability of the code, especially for code reviews where no IntelliSense or other tool-assisted context is available. The fundamental principle is that the code should be easy to read and understand at a glance, without having to search for the type of a variable or method return type. Similarly, it should be easy to distinguish between constants, fields, properties, statics, and other types of members.

---

The general rules we follow are "use Visual Studio defaults" and "readability over convenience", since code is read more often than it is written. Still, we aim to reduce boilerplate where possible and eliminate redundancy in the codebase.

1. We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. The only exceptions to this rule are auto-implemented properties (i.e. `public int Foo { get; set; }`), simple object initializers (i.e. `Person p = new() { Name = "John" };`), and empty-bodied block statements (i.e. `() => {}` (No-op lambda), or `for (node = first; node != null; node = node.Next) {}`).
2. We use four spaces of indentation (no tabs) to maintain visual consistency across different editors and platforms where tab width may vary.
3. We communicate visibility and scope through naming conventions:
    - types, properties, and all methods (including local ones) are in `PascalCase` to distinguish them from fields and locals, ruling them out as potential targets for ByRefs (the `ref` keyword).
    - local variables and parameters are in `camelCase`.
    - non-public instance fields are in `_camelCase` to distinguish them from locals and properties.
        - except: the member is a primary constructor parameter, in which case it is in `camelCase`. For larger classes, consider using explicit private fields in `_camelCase` to avoid confusion with local variables.
    - public fields must only be used in `structs` and only where they are advantageous over properties for performance or interop reasons. Public fields should be named in `PascalCase` with no prefix when used.
    - static fields have an `s_` prefix to distinguish them from instance fields: `s_camelCase`.
    - thread static fields have a `t_` prefix to clearly indicate their scope: `t_camelCase`.
    - thread-local instance fields have a `_th_` prefix to clearly indicate their scope: `_th_camelCase`.
    - async-local instance fields have a `_al_` prefix to clearly indicate their scope: `_al_camelCase`. This can be combined with the static prefix for static async-local fields: `s_al_camelCase`.
    - all compile-time constants are in `SCREAMING_SNAKE_CASE` to communicate their immutability and distinguish them from properties.
4. We restrict visibility and modification as much as possible: hiding implementation details from the outside by making them `private` or `internal`, and declaring fields as `readonly` where possible. This communicates the intent of the code and reduces the risk of accidental modification.
5. We avoid `this.` unless when accessing primary constructor parameters for a clear distinction between fields and locals.
6. We always specify the visibility, even if it's the default (e.g. `private string _foo` not `string _foo`). Visibility should be the first modifier (e.g. `public abstract` not `abstract public`).
7. Namespace imports should be specified at the top of the file, *outside* of `namespace` declarations, and should be sorted alphabetically. We use file-scoped namespaces (`namespace Foo;` instead of `namespace Foo {...}`) to avoid excessive indentation.
8. Avoid more than one empty line, as well as any number of trailing whitespace. Enable 'View White Space' in Visual Studio to detect these issues.
9. File names should be named after the type they contain, for example `class Foo` should be in `Foo.cs`. Every file should contain at most one top-level type, although it may contain nested types and additional file-local type (e.g. `class Foo { class Bar { } }` and `class Foo {} file class Bar {}`).
    - except: when working with very tightly coupled types, where it makes sense to have them in the same file for easier navigation and understanding. In such cases, it is acceptable to have multiple top-level types in the same file.
    - except: when working generic types that would conflict with non-generic types of the same name, append ``​`N`` to the file name, where `N` corresponds to the arity of the generic type, e.g. `class Foo` in `Foo.cs`, `class Foo<T>` in ``Foo`1.cs``, and `class Foo<TKey, TValue>` in ``Foo`2.cs``. Apart from avoiding naming conflicts, this convention also matches the string representation of the corresponding unbound generic types returned by reflection, which can be helpful when navigating the codebase and searching for types.
10. Avoid `var`, even when the type is obvious (to maintain consistency). Explicit types improve readability, reduce the risk of bugs, and allow reviewers and other developers to understand code at a glance, which is especially important in code reviews where no IntelliSense is available.
    - Why? In conjunction with rule 11, disallowing usage of `var` prevents bugs where the type is not what the developer expected, which is especially common in async contexts. For example:
    ```csharp
    [HttpGet]
    public IActionResult GetFoo()
    {
        var foo = dbContext.Set<Foo>().FirstOrDefaultAsync();
        if (foo == null)
        {
            return NotFound();
        }
        // we expect foo to be a Foo but it's actually a Task that was never awaited :C
        return Ok(foo);
    }
    ```
    - The argument that `var` reduces boilerplate is less relevant in modern C# versions where target-typed `new()` is available, which we use where possible but only when the type is explicitly named on the left-hand side, e.g., `FileStream stream = new(...);`, but not `stream = new(...);` (where the variable was declared on a previous line).
    - in foreach loops, we can use tuple deconstruction to avoid the need for `var` and make the code more readable, e.g., `foreach ((int key, string value) in dictionary) { ... }`.
    - In all other cases, we still prefer explicit types over `var` for readability and consistency, even if it brings the inconvenience of having to type more characters while writing code.
    - The only exception to this rule is when the type is truly anonymous, e.g., `var x = dbContext.Set<Foo>().Select(foo => new { Name, Age }).First()`. Still, consider using a `file`-scoped (struct) type in such cases to make the code more readable and maintainable.
11. When writing task-returning methods, we postfix method names with `Async` (e.g. `public async Task<int> FooAsync()` instead of `public async Task<int> Foo()`). This clearly communicates the asynchronous nature of the method to the caller and helps reduce cases of asynchronous methods not being awaited.
12. We use language keywords instead of BCL types (e.g. `int, string, float` instead of `Int32, String, Single`, etc) for both type references as well as method calls (e.g. `int.Parse` instead of `Int32.Parse`).
13. We use `nameof(...)` instead of `"..."` whenever possible and relevant. Similarly, we use `string.Empty` instead of `""`.
14. Fields should be specified at the top within type declarations. The order of field groups should be `const`, `static`, `readonly`, then `instance` fields. This helps keep the fields organized and keeps fields that change more frequently closer to the implementation details.
15. When including non-ASCII characters in the source code use Unicode escape sequences (`\uXXXX`) instead of literal characters. Literal non-ASCII characters occasionally get garbled by a tool or editor.
16. When using labels (for goto), indent the label one less than the current indentation, and name labels with `SCREAMING_SNAKE_CASE`.
    - We strictly discourage the use of `goto` for control flow, except in the following cases, **if and only if it improves overall readability**:
        - Breaking out of deeply nested loops, where `break` and `continue` don't suffice and the alternative would be to introduce one or more boolean flags that would make the code less readable.
        - In `bool TryGet(out Foo? foo)` methods or similar cases where a distinct failure path with additional cleanup is required (e.g. assigning default values to out parameters) it is acceptable to define a `FAILURE` label after the primary `return true` of the method and use `goto FAILURE` after each check that would result in a failure.
        - In very rare cases where `goto` is the most readable and maintainable solution. You should be prepared to explain your reasoning in code review and consider whether there is a better way to structure the code.
17. Make all internal and private types static or sealed unless derivation from them is required. As with any implementation detail, they can be changed if/when derivation is required in the future.
18. We avoid magic strings and numbers in our code, especially when they are used more than once. Instead, we use constants or readonly fields to define these values. When passing fixed values to a method, we prefer to use named arguments to make the code more readable and self-explanatory, e.g., `DeflateStream deflate = new(fileStream, CompressionMode.Compress, leaveOpen: true)` instead of `DeflateStream deflate = new(fileStream, CompressionMode.Compress, true)`.
19. The recommended order of modifiers is `public`, `private`, `internal`, `protected`, `file`, `static`, `extern`, `const`, `required`, `sealed`, `virtual`, `new`, `async`, `abstract`, `override`, `readonly`, `partial`, `unsafe`, and `volatile`. This order keeps the code consistent and lists modifiers in order of importance when reading the code from left to right. For example, visibility is more important than knowing whether a method is `virtual` or its implementation details require `unsafe` code.
20. We use `#region` and `#endregion` directives sparingly and only in very large files where splitting the file into separate files is not feasible. If used, regions should always be named (`#region Name`, and `#endregion Name`).
21. We use nullable reference types in all new projects globally and for new code in existing projects. We use nullable annotations to indicate when a value can be null and when it cannot. This especially includes `out` parameters of `bool TryGet...(out Foo? value)` where `[NotNullWhen(...)]` or `[MaybeNullWhen(...)]` attributes should be specified for the `out` parameter.
22. All types, members, and variables should be named in a way that clearly communicates their purpose and intent. This includes avoiding abbreviations, using full words instead of acronyms, and using descriptive names that make the code self-explanatory. This is especially important for public APIs and interfaces, where the names should be clear and concise to make the code easy to understand and use. Exceptions to this rule are common acronyms and abbreviations that are widely understood in the context of the codebase and single-letter variable names for loop counters and similar short-lived variables. The larger the scope of the variable, the more descriptive the name should be.
    - When using acronyms, we use PascalCase by default, e.g., `XmlDocument`, `HttpRequest`, etc. Only when the acronym is two letters long, capitalization is also acceptable, e.g., `IPAddress`.
    - Generic type parameters should always start with `T` followed by a descriptive name, e.g., `TKey` and `TValue` for dictionary types. Only in cases without ambiguity, the single letter `T` can be used, e.g., `List<T>`.
    - Interface names should be prefixed with `I`, e.g., `IEnumerable`, `IDisposable`, `IList`, etc.
23. We DO NOT use [Hungarian notation](https://en.wikipedia.org/wiki/Hungarian_notation) or other type prefixes in our code. The context of the code should make the type clear, and the use of descriptive names should eliminate the need for type prefixes. The only (and rare) exception to this rule is when working with handles or pointers, where handles should be prefixed with `h` and pointers with `p`, e.g., `IntPtr hInstance`, `void* pData`. This convention comes from the C/C++ world and makes it **absolutely clear in all contexts** that the variable is an unmanaged resource that may require special handling.
24. Exceptions to the above naming rules may be made under special circumstances where other conventions apply, such as in low-level P/Invoke interop scenarios where unmanaged structures and P/Invoke stubs should follow native naming conventions. Note that these internal types should never be exposed to public APIs and even internal calls should be wrapped where possible.

An [EditorConfig](https://editorconfig.org "EditorConfig homepage") file (`.editorconfig`) has been provided alongside this document in the solution directory, enabling C# auto-formatting conforming to the above guidelines.

---

## RimWorld-specific Addendum

Since RimWorld is built on Unity engine, above rules apply with the following exceptions:

1. When interacting with XML-based definitions that rely on fields being named in camelCase, we define the fields in camelCase as private members, encapsulate them in PascalCased properties, and add a `SuppressMessage` attribute to suppress the naming style warning for the containing member. This allows us to maintain the C# coding style while still being adhering to the RimWorld XML naming conventions.

```csharp
// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class ReusabilityProps_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly float destroyChance = default;

    public float DestroyChance => destroyChance;
}
```

For `DefOf` classes, we allow public fields to be used, as they are required for RimWorld to properly load the definitions. We use PascalCasing for these fields.

```csharp
[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_DEF_OF_REQUIRES_FIELD)]
public static class KnownThingDefOf
{
    public static ThingDef Bandage = null!;
    public static ThingDef HemostaticAgent = null!;
    public static ThingDef Splint = null!;
    public static ThingDef SuctionDevice = null!;
    public static ThingDef Tourniquet = null!;
    public static ThingDef Defibrillator = null!;
    public static ThingDef Epinephrine = null!;
    public static ThingDef Morphine = null!;
    public static ThingDef WholeBloodBag = null!;
}
```

2. An exception to rule 13 of using `nameof(...)` is made for key names in save data. These are stored as camelCase string literals to prevent renaming a C# member from breaking save compatibility.

```csharp
public override void ExposeData()
{
    base.ExposeData();
    Scribe_Values.Look(ref _usesDevice, "usesDevice");
    Scribe_Values.Look(ref _pathEndMode, "pathEndMode");
    Scribe_Values.Look(ref _fromInventoryOnly, "fromInventoryOnly");
    Scribe_Values.Look(ref _oneShot, "oneShot");
    Scribe_Values.Look(ref _oneShotUsed, "oneShotUsed");
}
```