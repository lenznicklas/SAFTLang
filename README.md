# SAFTLang
`Static And Fully Typed`

## CLI-Usage

`saft build <file.sft>`

`saft run   <file.sft>`

## Types
`bool`
`int`
`string`
`void`
`error`

## (Current) Syntax

### Main-Method

You can only run code from your main method, which you (currently) need to have:

```
func main() void {
  foo()
}
```

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

### Functions

```
func foo(x: int, y: string) void {
  doSomething();
}
```

If there is a return type, as an example a Integer, then:

``` 
func foo() int {
  let x: int = 67
  return x
}
```

You can call a function by simply using: `foo()`