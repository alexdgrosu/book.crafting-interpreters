# Crafting Interpreters

Personal repository for following along with the _"Crafting Interpreters"_ book by Robert Nystrom [^1].

> Crafting Interpreters contains everything you need to implement a full-featured, efficient scripting language. You’ll learn both high-level concepts around parsing and semantics and gritty details like bytecode representation and garbage collection. Your brain will light up with new ideas, and your hands will get dirty and calloused. It’s a blast.
>
> Starting from main(), you build a language that features rich syntax, dynamic typing, garbage collection, lexical scope, first-class functions, closures, classes, and inheritance. All packed into a few thousand lines of clean, fast code that you thoroughly understand because you write each one yourself.

# Repository Structure

This repository contains/_will eventually contain_:

- `sharplox`: a C# implementation of a [Lox](https://craftinginterpreters.com/the-lox-language.html) interpreter  
  (_called_ `jlox` _in the book & implemented in Java_) [^2]
- `clox`: a C implementation of a Lox compiler [^3]

## `sharplox`

Implementation is in progress.  
`sharplox` currently supports the following context-free grammar:

```shell
expression    → equality ;
equality      → comparison ( ( "!=" | "==" ) comparison )* ;
comparison    → term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
term          → factor ( ( "-" | "+" ) factor )* ;
factor        → unary ( ( "/" | "*" ) unary )* ;
unary         → ( "!" | "-" ) unary
              | primary ;
primary       → NUMBER | STRING | "true" | "false" | "nil"
              | "(" expression ")" ;
```

### Usage

`$ sharplox [script].lox`

You can also enter an interactive prompt by not providing an input source file:

```shell
$ sharplox
> 2 + 2;
4
> "4" + 2;
42
> ...
```

### Notes on Implementation

- I've taken the liberty to sprinkle in some C# features wherever I saw fit
  - _e.g._ [C# Source Generators](./sharplox/Lox.Generator)
  - _e.g._ [C# `switch` Expressions](sharplox/Lox.Interpreter/Interpreter.cs#L165)

```csharp
private static bool IsTruthy(object obj)
{
  return obj switch
  {
    null => false,
    bool boolean => boolean,
    _ => true
  };
}
```

- I've structured `sharplox` in a way that makes sense to _me_ during my learning journey; not advocating this is how you should structure an interpreter

## `clox`

Implementation not started.

[^1]: [Web version of _"Crafting Interpreters"_](https://craftinginterpreters.com/)
[^2]: [§1.3 The First Interpreter](https://craftinginterpreters.com/introduction.html#the-first-interpreter)
[^3]: [§1.4 The Second Interpreter](https://craftinginterpreters.com/introduction.html#the-second-interpreter)
