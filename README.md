# SAFTLang
`Static And Fully Typed`

## Types
`bool`
`int`
`string`
`void`
`error`

## (Current) Syntax

### Variable Declaration

`let x = 1` ==> int

`let s = "Hello World"` ==> string

`let b1 = 4 < 9` => bool

`let b2 = s < 1` ==> error

Also possible as a constant instead of a changeable variable.

`const pi = 5`

Explicit typing is also possible:

`let x: int = 5`

`let y: int = "Hello World"` ==> error

### If/Else

```
if x > 10 {
  foo()
} else {
  foo2()
}
```
