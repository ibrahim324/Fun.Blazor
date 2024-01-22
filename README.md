# Fun.Blazor [![Nuget](https://img.shields.io/nuget/vpre/Fun.Blazor)](https://www.nuget.org/packages/Fun.Blazor)

![image](./Docs//assets/fun-blazor%3D.png)

This is a project to make F# developer to write blazor easier.

1. Use F# ❤️😊 for blazor
2. Computation expression (CE) style DSL for internal and third party blazor libraries
3. Dependency injection (html.inject)
4. [Adaptive](https://github.com/fsprojects/FSharp.Data.Adaptive) model (adaptiview/AdaptivieForm) (**recommend**), [elmish](https://github.com/elmish/elmish) model (html.elmish)
5. Giraffe style routing (html.route/blazor official style)
6. Type safe style (Fun.Css)
7. Convert html to CE style by [Fun.Dev.Tools](https://slaveoftime.github.io/Fun.DevTools.Docs)

Check the [WASM Docs](https://slaveoftime.github.io/Fun.Blazor.Docs/) for more 🚀

## Start to use

    dotnet new install Fun.Blazor.Templates::3.2.0
    dotnet new fun-blazor -o FunBlazorDemo1

> Requires dotnet 8

## Code samples

```fsharp
// Functional style
let demo (str) = fragment {
    h2 { $"demo {str}" }
    p { "hi here" }
}

// Class style
type Foo() =
    inherit FunComponent()

    let mutable count = 0

    override _.Render() = main {
        h1 { "foo" }
        demo $"hi {count}"
        button {
            on.click (fun _ -> count <- count + 1)
            "Click me"
        }
    }
```

## CE build performance

- There is CE performance [issue](https://github.com/dotnet/fsharp/issues/14429) for inline or nest too much CE block.

There are some tests in [here](https://github.com/albertwoo/CEPerfDemo), in summary, below are some recommend ways for better build time performance (but it can reduce runtime performance because we cannot inline and need to allocate memory on head for creating array or list)

- The best result is **list-with-local-vars** for multiple child items

```fsharp
let demo1 = div {
    class' "font-bold"
    "demo1"
}

let demo2 = div {
    class' "font-bold"
    "demo2"
}

let comp = div {
    style { color "red" }
    childContent [| // 👌✅
        demo1
        demo2
    |]
}
```

- **nested-one** is ok

```fsharp
let comp = div {
    class' "font-bold"
    div { // 👌✅
        class' "font-bold"
        div { "demo1" }
    }
}
```

- **nested-one-one** is not ok (bad for build perf)

```fsharp
let comp = div {
    class' "font-bold"
    div {
        class' "font-bold"
        div { // ⛔🙅
            class' "font-bold"
            div { "demo1" }
        }
    }
}
```

- inline local vars is not ok (bad for build perf)

```fsharp
let comp = div {
    class' "font-bold"
    let temp = div { // ⛔🙅
        class' "font-bold"
        "demo1"
    }
    temp
}
```

## Local development

You can run **dotnet fsi build.fsx -- -h** to check what is available to help you get started.

## Benchmark

BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
Intel Core i7-1065G7 CPU 1.30GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2


| Method                | Mean     | Error    | StdDev    | Median   | Gen0   | Allocated |
|---------------------- |---------:|---------:|----------:|---------:|-------:|----------:|
| RenderWithRazorCSharp | 580.4 ns | 24.14 ns |  70.41 ns | 566.4 ns | 0.0935 |     392 B |
| RenderWithFunBlazorCE | 677.9 ns | 11.49 ns |  18.23 ns | 675.5 ns | 0.1774 |     744 B |
| RenderWithBolero      | 905.4 ns | 34.92 ns | 102.95 ns | 872.0 ns | 0.3567 |    1496 B |
