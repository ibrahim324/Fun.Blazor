﻿namespace Fun.Blazor

open System


type FunBlazorNode =
    | Elt of tag: string * FunBlazorNode list
    | Attr of key: string * value: Choice<string, bool>
    | Fragment of FunBlazorNode list
    | Text of string
    | BoleroNode of Bolero.Node
    | BoleroAttr of Bolero.Attr
    | BoleroAttrs of Bolero.Attr list

    static member GetBoleroNodesAndAttrs nodes =
        let rec getBoleroNodeAndAttrs nodes =
            nodes 
            |> Seq.fold
                (fun (nodes, attrs) x ->
                    match x with
                    | Attr (k, Choice1Of2 v) -> nodes, attrs@[ Bolero.Attr (k, v) ]
                    | Attr (k, Choice2Of2 true) -> nodes, attrs@[ Bolero.Attr (k, null) ]
                    | Attr _ -> nodes, attrs
                    | BoleroAttr x -> nodes, attrs@[x]
                    | BoleroAttrs x -> nodes, attrs@x
                    | node ->
                        let node =
                            match node with
                            | Elt (tag, nodes) ->
                                let nodes, attrs = getBoleroNodeAndAttrs nodes
                                Bolero.Elt(tag, attrs, nodes)
                            | Fragment nodes ->
                                let nodes, _ = getBoleroNodeAndAttrs nodes
                                Bolero.Concat nodes
                            | Text x ->
                                Bolero.Text x
                            | BoleroNode x ->
                                x
                            | BoleroAttr _
                            | BoleroAttrs _
                            | Attr _ ->
                                Bolero.Empty
                        nodes@[node], attrs)
                ([], [])
        getBoleroNodeAndAttrs nodes

    static member ToBolero node =
        let nodes, _ = FunBlazorNode.GetBoleroNodesAndAttrs [ node ]
        Bolero.ForEach nodes

    member this.ToBolero () = FunBlazorNode.ToBolero this


type [<Struct>] GenericFunBlazorNode<'T> =
    { Node: FunBlazorNode }

    static member create x: GenericFunBlazorNode<'T> = { Node = x }


type IComponentHook =
    //abstract OnParametersSet: IEvent<unit>
    //abstract OnInitialized: IEvent<unit>
    abstract OnAfterRender: IEvent<bool>
    abstract OnDispose: IEvent<unit>
    abstract AddDispose: IDisposable -> unit
    abstract AddDisposes: IDisposable seq -> unit
    abstract StateHasChanged: unit -> unit


type IStore<'T> =
    abstract Publish: ('T -> 'T) -> unit
    abstract Publish: 'T -> unit
    abstract Observable: IObservable<'T>
    abstract Current: 'T


type ILocalStore =
    /// Create an IStore and hold in component and dispose it after component disposed
    abstract Create: 'T -> IStore<'T>


type IShareStore =
    /// Create an IStore and share between components and dispose it after session disposed
    abstract Create: string * 'T -> IStore<'T>

    /// Create an IStore and share between components and dispose it after session disposed
    abstract Create: 'T -> IStore<'T>
    