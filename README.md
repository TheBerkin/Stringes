Stringes
========

The Stringe is a wrapper for the .NET String object that provides line, column, and offset info for substrings, relative to their parent string.

Stringes can be created from normal strings.
```cs
var stringe = new Stringe("Hello\nWorld!");
```

You can cut them up with ease.
```cs
var lines = stringe.Split('\n');
```

Each *substringe* can be traced back to where it originally came from.
```cs
foreach(var substringe in lines)
{
    Console.WriteLine("Line {0}: {1}", substringe.Line, substringe);
}
```
```
Line 1: Hello
Line 2: World!
```
