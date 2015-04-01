Stringes
========

##What is a *stringe*?!

The *Stringe* is a wrapper for the .NET String object that tracks line, column, offset, and other metadata for substrings.

Stringes can be created from normal strings, either explicitly or implicitly.
```cs
Stringe stringeA = new Stringe("Hello\nWorld!");
Stringe stringeB = "Hello\nWorld!";
```

###Support for native System.String methods

The `Stringe` class supports many of the same fabulous methods that regular strings have. I plan on eventually getting them all implemented.
Unlike the String type, however, methods like `Stringe.Split` return an `IEnumerable<Stringe>` instead of an array. The result of this is that these methods use lazy evaluation, which can improve performance in cases where the user does not need all of the returned data.
```cs
IEnumerable<Stringe> lines = stringe.Split('\n');
IEnumerable<Stringe> words = stringe.Split(' ');
```

###Finding the parent string from a substringe

Each *substringe* can be traced back to the string it originally came from.
```cs
Stringe parent = "The quick brown fox jumps over the lazy dog";
Stringe substr = parent.Substringe(16, 3); // "fox"
Console.WriteLine(substr.ParentString); // "The quick brown fox jumps over the lazy dog"
```

###Location tracking

Substringes keep track of the line, column, and index on which they appear. This information can be easily accessed through properties. This is **especially** useful when writing lexers, so that errors in compiled code can be traced back to the exact place where the associated tokens were read.

```cs
var lines = new Stringe("Hello\nWorld!").Split('\n');
foreach(var substringe in lines)
{
    Console.WriteLine("Line {0}: {1}", substringe.Line, substringe);
}
```
```
Line 1: Hello
Line 2: World!
```

##Lexers

The Stringes library contains all the tools you need to write a lexer. The lexer-specific classes are:
* `Lexer<T>`: The main lexer class, which generates tokens according to user-specified rules.
* `Token<T>`: The token class, which wraps the `Stringe` class, includes information identifying the token type using a user-specified type. An enum is recommended for the type parameter.

**For an example of how to make a lexer, see the [LexerExample](https://github.com/TheBerkin/Stringes/blob/master/LexerExample/Program.cs) source!**
