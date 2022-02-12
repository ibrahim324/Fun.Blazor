﻿[<AutoOpen>]
module Fun.Blazor.Utils

open System
open System.Diagnostics
open System.Reactive.Linq
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Components
open Microsoft.Extensions.ObjectPool


module Internal =
    let inline emptyAttr () = AttrRenderFragment(fun _ _ i -> i)
    let inline emptyNode () = NodeRenderFragment(fun _ _ i -> i)


    let objectPoolProvider = DefaultObjectPoolProvider()
    let stringBuilderPool = objectPoolProvider.CreateStringBuilderPool()


    let makeStyles (rules: (string * string) seq) =
        let sb = stringBuilderPool.Get()

        for (k, v) in rules do
            sb.Append(k).Append(": ").Append(v).Append("; ") |> ignore

        let result = sb.ToString()
        stringBuilderPool.Return sb
        result


    type ILogger with

        member this.LogDebugForPerf fn =
    #if DEBUG
            let sw = Stopwatch.StartNew()
            this.LogDebug($"Function started")
            let result = fn ()
            this.LogDebug($"Function finished in {sw.ElapsedMilliseconds}")
            result
    #else
            fn ()
    #endif


module Observable =
    let ofTask (x: Task<_>) =
        Observable.FromAsync(fun (token: Threading.CancellationToken) -> x)


type IComponent with

    member comp.Render(fragment: NodeRenderFragment) =
        RenderFragment(fun builder -> fragment.Invoke(comp, builder, 0) |> ignore)

    member comp.Callback<'T>(fn: 'T -> unit) = EventCallback.Factory.Create<'T>(comp, fn)
    member comp.Callback<'T>(fn: 'T -> Task) = EventCallback.Factory.Create<'T>(comp, fn)