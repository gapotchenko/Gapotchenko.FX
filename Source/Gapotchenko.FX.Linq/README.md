# Gapotchenko.FX.Linq

`Gapotchenko.FX.Linq` module provides pure primitives for functional processing of data sequences.

What does "pure" mean, you may ask?
A pure function is a function where the return value is only determined by its input values, without observable side effects.


A side effect is anything that goes beyound the state of a function itself.
For example, if a function reads or writes to disk than it is not pure because it consumes or changes the external state.
The same goes to functions that share a common state between the calls. They have a side effect, so they are not pure.

## Why Pure Functions Are Important?

Pure functions are very easy to reason about, debug and combine.
They provide an inherent thread-safety.
But when combined together, they allow to build a sophisticated functionality, often just by lifting a finger.

For example, all math functions like `+`, `-`, `sin`, `cos` are pure.
That's why they are versatile and can be reused again and again.
Be it physics, construction, computer science or any other aspect of an everyday life.

The same goes to LINQ which roughly represents the math of data processing in .NET.

## A Dirty Secret of a Performant Functional Code

When LINQ first came out in 2007, a lot of people scratched their heads:
an algorithm implemented in LINQ was in average 5 times slower than an equivalent imperative implementation.
This still remains a true observation today, but only for simple algorithms.

Once an algorithm reaches a certain point of complexity,
the following properties become more important and desirable:

- Parallelization
- Cache locality
- Laziness

Those are tough targets for imperative algorithms.
And this is where functional code starts to shine,
delivering much faster implementations comparing to their imperative equivalents.

How much faster?
If we take away the parallelization and compare a thread-to-thread performance then we get an average number of 5.
The table has turned.

.NET supports parallelization with tasks and PLINQ.
Cache locality is an inherent property of a functional code.
But what about laziness? .NET does a lot of right things, but lacks a crucial ingredient &mdash; a LINQ memoization primitive.

## Memoization

Memoization is a pinnacle of functional data processing.
You already met it quite often albeit in somewhat masked forms.
