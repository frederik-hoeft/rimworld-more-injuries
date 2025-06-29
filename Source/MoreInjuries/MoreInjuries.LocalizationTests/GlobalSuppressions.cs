// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    category: "Maintainability",
    checkId: "CA1515:Consider making public types internal", 
    Justification = "Test classes must be public for test discovery.",
    Scope = "namespaceanddescendants", 
    Target = "~N:MoreInjuries.LocalizationTests")]
