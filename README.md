# SAFTLang
`Static And Fully Typed`

## CLI-Usage

`saft build <file.sft>`

`saft run   <file.sft>`

## Types
`bool`
`int`
`string`
`array`
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

You can create array by using the following syntax:

`let arr: int[] = [1, 2, 3]`

`let arr2 = ["a", "b", "c"]` ==> string[]

If you create an empty array, you have to declare the type.

`let arr3 = []` ==> error

You can create primary variables from array indexes by using:

`let n = arr[1]`

If you want to change a value in a mutable array:

`arr[0] = 10`

You can also create immutable arrays by changing `let` to `const`

`const arr4 = [1, 2, 3, 4]`

### Comparisons

Comparison between bool, int, string and array is possible

`[1, 2, 3] == [1, 2, 3]` ==> true

### If/Else

```
if x > 10 {
  foo()
} else {
  foo2()
}
```

### for

In SAFT there is no while or never ending loop. You can create them both with `for`.

If you need a never ending loop you can just write:

```
for {
  foo()
}
```

If you want a classical while statement you can use:

```
for x > y {
  foo()
}
```

And when you need a foreach loop you can use the regular syntax you might know from Python or C#:

```
for item in array {
  foo(item)
}
```

Regular for loops, like in C or Java, are not supported yet.

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
