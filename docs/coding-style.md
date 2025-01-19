# C# Coding Style

This document describes the C# coding style used in this project. It is based on the [.NET Runtime Coding Style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) with some modifications to improve readability and maintainability of the codebase.

---

The general rule we follow is "use Visual Studio defaults", "be explicit", and "reduce boilerplate".

1. We use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. The only exceptions to this rule are auto-implemented properties (i.e. `public int Foo { get; set; }`), simple object initializers (i.e. `Person p = new() { Name = "John" };`), and empty bodied block statements (i.e. `() => {}`, or `for (node = first; node != null; node = node.Next) {}`).
2. We use four spaces of indentation (no tabs).
3. We use `_camelCase` for internal and private fields and use `readonly` where possible. Prefix internal and private instance fields with `_`, static fields with `s_`, thread static fields with `t_`, and non-public thread-local fields with `_th_`. When used on static fields, `readonly` should come after `static` (e.g. `static readonly` not `readonly static`). Public fields should only be used in `structs` and only where they are advantageous over properties. Public fields should use PascalCasing with no prefix when used.
4. We avoid `this.` unless absolutely necessary.
5. We always specify the visibility, even if it's the default (e.g.
   `private string _foo` not `string _foo`). Visibility should be the first modifier (e.g.
   `public abstract` not `abstract public`).
6. Namespace imports should be specified at the top of the file, *outside* of
   `namespace` declarations, and should be sorted alphabetically. We use file-scoped namespaces (`namespace Foo;` instead of `namespace Foo {...}`) to avoid excessive indentation (see rule 1).
7. Avoid more than one empty line at any time. For example, do not have two
   blank lines between members of a type.
8. Avoid trailing free spaces.
   For example avoid `if (someVar == 0)...`, where the dots mark the spurious free spaces.
   Consider enabling "View White Space (Ctrl+R, Ctrl+W)" or "Edit -> Advanced -> View White Space" if using Visual Studio to aid detection.
9. File names should be named after the type they contain, for example `class Foo` should be in `Foo.cs`. Every file should contain at most one top-level type, although it may contain nested types and additional file-local type (e.g. `class Foo { class Bar { } }` and `class Foo {} file class Bar {}`).
    - In cases of generic types that would conflict with non-generic types of the same name, append `.T` to the file name, e.g. `class Foo` in `Foo.cs` and `class Foo<T>` in `Foo.T.cs`. For more than one generic type, use `.T1`, `.T2`, etc, where the number corresponds to the number of generic types, e.g. `class Foo<T1, T2>` in `Foo.T2.cs`.
10. We do not use `var`, even when the type can be inferred. This is to maintain a consitent style throughout the codebase and to aid code review where no intellisense is available and reviewers would have to search for the type. This helps to improve readability of the codebase and makes it easier to understand the codebase at a glance. At the same time, code review is made easier as the type is immediately apparent.
    - To reduce boilerplate, we use target-typed `new()` where possible but only when the type is explicitly named on the left-hand side, in a variable definition statement or a field definition statement. e.g. `FileStream stream = new(...);`, but not `stream = new(...);` (where the variable was declared on a previous line).
11. We use language keywords instead of BCL types (e.g. `int, string, float` instead of `Int32, String, Single`, etc) for both type references as well as method calls (e.g. `int.Parse` instead of `Int32.Parse`).
12. We use SCREAMING_SNAKE_CASE for all our constant fields and constant local variables. This clearly distinguishes constants from other types and fields.
13. We use PascalCasing for all method names, including local functions, as well as al type names, and all property names (regardless of visibility).
14. We use `nameof(...)` instead of `"..."` whenever possible and relevant. Similarly, we use `string.Empty` instead of `""`.
15. Fields should be specified at the top within type declarations.
16. When including non-ASCII characters in the source code use Unicode escape sequences (`\uXXXX`) instead of literal characters. Literal non-ASCII characters occasionally get garbled by a tool or editor.
17. When using labels (for goto), indent the label one less than the current indentation, and name labels with SCREAMING_SNAKE_CASE.
18. Make all internal and private types static or sealed unless derivation from them is required. As with any implementation detail, they can be changed if/when derivation is required in the future.
19. When writing task-returning methods, we postfix method names with `Async` (e.g. `public async Task<int> FooAsync()` instead of `public async Task<int> Foo()`). This clearly communicates the asynchronous nature of the method to the caller and helps reduce cases of asynchronous methods not being awaited.

An [EditorConfig](https://editorconfig.org "EditorConfig homepage") file (`.editorconfig`) has been provided at the root of the solution, enabling C# auto-formatting conforming to the above guidelines.

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

2. An exception to rule 14 of using `nameof(...)` is made for key names in save data. These are stored as camelCase string literals to prevent renaming a C# member from breaking save compatibility.

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