# FormatWith

[![NuGet](https://img.shields.io/nuget/v/FormatWith)](https://www.nuget.org/packages/FormatWith/)
[![license](https://img.shields.io/github/license/crozone/FormatWith)](https://github.com/crozone/FormatWith/blob/master/License.txt)
![Build status](https://github.com/crozone/FormatWith/workflows/CI/badge.svg)

A set of string extension methods for performing {named} {{parameterized}} string formatting, written for NetStandard 2.0.

## Quick Info

This library provides named string formatting via the string extension .FormatWith(). It formats strings against a lookup dictionary, anonymous type, or handler.

It is written as a Net Standard 2.0 class library, published as a NuGet package, and is fully compatible with any .NET platform that implements NetStandard 2.0. This makes it compatible with .NET Core 2.0, .NET Full Framework 4.6.1, UWP/UAP 10, and most mono/xamarin platforms.

An example of what it can do:

    using FormatWith;
    ...
    string formatString = "Your name is {name}, and this is {{escaped}}, this {{{works}}}, and this is {{{{doubleEscaped}}}}";
    
    // format the format string using the FormatWith() string extension.
    // We can parse in replacement parameters as an anonymous type
    string output = formatString.FormatWith(new { name = "John", works = "is good" });
    
    // output now contains the formatted text.
    Console.WriteLine(output);

Produces:

> "Your name is John, and this is {escaped}, this {is good}, and this is {{doubleEscaped}}"

It can also be fed parameters via an `IDictionary<string, string>` or an `IDictionary<string, object>`, rather than a type.

The value of each replacement parameter is given by whatever the objects `.ToString()` method produces. This value is not cached, so you can get creative with the implementation (the object is fed directly into a StringBuilder).

## How it works

A state machine parser quickly runs through the input format string, tokenizing the input into tokens of either "normal" or "parameter" text. These tokens are simply a struct with an index and length into the original format string - `SubString()` is avoided to prevent unnecessary string allocations. These are fed out of an enumerator right into a `StringBuilder`. Since `StringBuilder` is pre-allocated a small chunk of memory, and only `.Append()`ed relatively large segments of string, it produces the final output string quickly and efficiently.

## Extension methods:

Three extension methods for `string` are defined in `FormatWith.StringExtensions`: `FormatWith()`, `FormattableWith()`, and `GetFormatParameters()`.

### FormatWith

The first, second, and third overload of `FormatWith()` take a format string containing named parameters, along with an object, dictionary, or function for providing replacement parameters. Optionally, missing key behaviour, a fallback value, and custom brace characters can be specified. Two adjacent opening or closing brace characters in the format string are treated as escaped, and will be reduced to a single brace in the output string.

Missing key behaviour is specified by the `MissingKeyBehaviour` enum, which can be `ThrowException`, `ReplaceWithFallback`, or `Ignore`.

`ThrowException` throws a `KeyNotFoundException` if a replacement value for a parameter in the format string could not be found.

`ReplaceWithFallback` inserts the value specified by `fallbackReplacementValue` in place of any parameters that could not be replaced. If an object-based overload is used, `fallbackReplacementValue` is an `object`, and the string representation of the object will be resolved as the value.

`Ignore` ignores any parameters that did not have a corresponding key in the lookup dictionary, leaving the unmodified braced parameter in the output string. This is useful for tiered formatting.

**Examples:**

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });

output: Throws a `KeyNotFoundException` with the message "The parameter \"DoesntExist\" was not present in the lookup dictionary".

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");`

output: "abc Replacement1 FallbackValue"

`string replacement = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

output: "abc Replacement1 {DoesntExist}"

**Using custom brace characters:**

Custom brace characters can be specified for both opening and closing parameters, if required.

`string replacement = "abc <Replacement1> <DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null,'<','>');`

output: "abc Replacement1 <DoesntExist>"

### FormattableWith

The first, second, and third overload of FormattableWith() function much the same way that the FormatWith() overloads do. However, FormattableWith returns a `FormattableString` instead of a `string`. This allows parameters and composite format string to be inspected, and allows a custom formatter to be used if desired.

### Handler overloads

A custom handler can be passed to both FormatWith() and FormattableWith(). The handler is passed the value of each parameter key and format (if applicable). It is responsible for providing a `ReplacementResult` in response. The `ReplacementResult` contains the `Value` which will be substituted, as well as a boolean `Success` parameter indicating whether the replacement was successful. If `Success` is false, the `MissingKeyBehaviour` is followed, as per the other overloads of FormatWith.

This can allow for some neat tricks, and even complex behaviours.

Example:

    "{abcDEF123:reverse}, {abcDEF123:uppercase}, {abcDEF123:lowercase}.".FormatWith(
                (parameter, format) =>
                {
                    switch (format)
                    {
                        case "uppercase":
                            return new ReplacementResult(true, parameter.ToUpper());
                        case "lowercase":
                            return new ReplacementResult(true, parameter.ToLower());
                        case "reverse":
                            return new ReplacementResult(true, new string(parameter.Reverse().ToArray()));
                        default:
                            return new ReplacementResult(false, parameter);
                    }
                });

Produces:

"321FEDcba, ABCDEF123, abcdef123."

### GetFormatParameters

`GetFormatParameters()` can be used to get a list of parameter names out of a format string, which can be used for inspecting a format string before performing other actions on it.

**Example:**

`IEnumerable<string> parameters = "{parameter1} {parameter2} {{not a parameter}}".GetFormatParameters();`

output: The enumerable will return "parameter1","parameter2" during iteration.

## Tests:

A testing project is included that has coverage of most scenarios involving the three extension methods. The testing framework in use is xUnit.

## Performance:

The SpeedTest test function performs 1,000,000 string formats, with a format string containing 1 parameter. On a low end 1.3Ghz mobile i7, this completes in around 700ms, giving ~1.4 million replacements per second.

The SpeedTestBigger test performs a more complex replacement on a longer string containing 2 parameters and several escaped brackets, again 1,000,000 times. On the same hardware, this test completed in around 1 seconds.

The SpeedTestBiggerAnonymous test is the same as SpeedTestBigger, but uses the anonymous function overload of FormatWith. It completes in just under 2 seconds. Using the anonymous overload of FormatWith is slightly slower due to reflection overhead, although this is minimised by caching.

So as a rough performance guide, FormatWith will usually manage about 1 million parameter replacements per second on low end hardware.
