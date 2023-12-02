# 关于

![image](../assets/fun-blazor%3D.png)

这是一个旨在让 F# 开发者更容易编写 Blazor 应用的项目。

其功能包括：

1. 允许使用 F# 进行 Blazor 开发
2. 使用计算表达式 (CE) 风格的 DSL 用于内部和第三方 Blazor 库
3. 使用依赖注入（html.inject）
4. 利用 [Adaptive](https://github.com/fsprojects/FSharp.Data.Adaptive) 模型（adaptiview/AdaptiveForm）(**推荐**), 或 [elmish](https://github.com/elmish/elmish) 模型 (html.elmish)
5. 实现了 Giraffe 风格的路由 (html.route/blazor 官方风格)
6. 提供了使用 Fun.Css，为CSS编辑真假类型安全
7. 使用 [Fun.Dev.Tools](https://slaveoftime.github.io/Fun.DevTools.Docs) 将 HTML 转换为 CE 风格


## 简单演示

Class 风格的计数器：

{{CounterClassStyle}}

这是一个使用自适应模型的基本计数器：

{{Counter}}

另一个使用 html.inject 的演示：

{{BlazorStyleComp}}

## 这是如何工作的？

Fun.Blazor 为 Blazor 提供了一系列委托进行处理。例如，当您编写：
```fsharp
let demo =
    div {
        class' "cool"
    }
```

这段代码本质上变成了：

```fsharp
let demo =
    NodeRenderFragment(fun comp builder index ->  // 委托
        builder.OpenElement(index, "div")
        bulder.AddAttribute(index + 1, "class", "cool")
        builer.CloseElement()
        index + 2
    )

type NodeRenderFragment = delegate of root: IComponent * builder: RenderTreeBuilder * sequence: int -> int
```

本质上，你已经创建了一个委托，该委托将传递给一个组件，该组件将管理 DOM 树的渲染或构建。这种方法类似于 Razor 引擎在 C# 世界中生成的内容。

可以使用 `adaptiview`、`html.inject` 等创建组件。这些组件是从 `ComponentBase` 继承的普通 Blazor 组件。

## 在使用 Fun.Blazor 之前要考虑以下几点：

1. F# 编译器在某些大型计算表达式 (CE) 的智能提示方面存在性能问题。最好减小单个 CE 块或文件，或使用 `seq`、`list` 或 `array` 与 `childContent` 以获得更好的智能提示效果：

    ```fsharp
    div {
        attributes ...
        childContent [ // ✅ 建议对于多个子元素时使用
            div { "hi" }
            ...a lot of child items
        ]
    }
    ```

    而不是以下格式：

    ```fsharp
    div {
        attributes ...
        div { "hi" }
        ...很多子项 ❌
    }
    ```

2. 热重载

    默认的模板已提供有限的热重载支持。 在过多的文件情况下会减慢热重载过程，因此为了取得最佳效果，应在想启用热重载的文件顶部添加 `// hot-reload` 。更多信息，请参见 [Fun.Blazor 热重载](https://www.slaveoftime.fun/blog/d959e36a-f4fe-4a10-88af-5e738633db0f?title=%20Hot-reload%20in%20Fun.Blazor) 博客文章或 [文档](https://slaveoftime.github.io/Fun.Blazor.Docs/?doc=/Hot%20Reload).

3. 对于组件元素中的属性和子元素，最好按以下方式排列：
    ```fsharp
    div {
        attributes ...
        ref (fun x -> ()) // ✅
        childContent [ ... ]
    }
    ```
    或者：

    ```fsharp
    div {
        attributes ...
        ref (fun x -> ()) // ✅
        div { 1 }
        div { 1 }
        // ...
    }
    ```


## 基准测试

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
