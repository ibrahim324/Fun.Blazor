namespace Fun.Blazor.Docs.Wasm.Pages.HelperFunctions

open System
open FSharp.Data.Adaptive
open Microsoft.AspNetCore.Components
open MudBlazor
open Fun.Blazor


type SlChangeEventArgs() =
    inherit EventArgs()

    member val Value = null with get, set


[<EventHandler("onsl-change", typeof<SlChangeEventArgs>, enableStopPropagation = true, enablePreventDefault = true)>]
[<AbstractClass; Sealed>]
type EventHandlers = class end



[<AutoOpen>]
module HtmlTemplateDemo =
    let htmlTemplateDemo =
        adaptiview () {
            let! count, setCount = cval(1).WithSetter()

            let increaseBy2 () = setCount (count + 2)

            let increaseBtn =
                MudButton'() {
                    OnClick(fun _ -> setCount (count + 1))
                    Variant Variant.Filled
                    Color Color.Primary
                    childContent "Mud Increase Btn"
                }

            let congratulations = Template.html $"<div>Congratulations! You made it ❤️</div>"

            Template.html $"""
                <div>
                    {congratulations}
                    <p>
                        You need to bring <span style="font-weight: bold; color: green; padding: 5px;">Fun.Blazor.HtmlTemplate</span> package to make this work!!!
                    </p>
                    <p style="background-color: hotpink; padding: 10px; color: white;">
                        The whole fsharp quotation will be evaluated to a bolero node dom tree and rendered by blazor
                    </p>
                    <br/>
                    Here is the count: {count}
                    <div>
                        <button onclick="{fun _ -> setCount (count + 1)}">Increase</button>
                        <button onclick="{ignore >> increaseBy2}">Increase by 2</button>
                        {increaseBtn}
                    </div>
                    {match count with
                     | 1 -> html.div "Match 1"
                     | _ -> html.div "Match _"}
                    <div style="color: {if count = 3 then "red" else "green"};" class="test-class">Cool!!</div>
                    <div style="{if count = 4 then "color: red;" else "color: green;"}">Cool!!</div>

                    <input type="checkbox" {if count > 4 then "checked" else ""} >
                    <input type="number" value="{count}" onchange="{fun (e: ChangeEventArgs) -> e.Value |> string |> int |> setCount}">

                    <sl-button onclick="{ignore >> increaseBy2}">Increase by 2</sl-button>


                    <p>
                        Add prefix 'on' for none standard event which do not start with onxxx, for example sl-change should be written as onsl-change.
                        But blazor 5 do not support custom event args, so the you can not get useful info in sl-change.
                        With blazor 6, you can follow https://docs.microsoft.com/en-us/aspnet/core/blazor/components/event-handling?view=aspnetcore-6.0#custom-clipboard-paste-event-example as example
                        Below is the example:
                        <ul>
                            <li>Define SlChangeEventArgs and attach to EventHandler</li>
                            <li>Register with javascript to map the event fields</li>
                            <li>Use it together with callback util</li>
                        </ul>
                    </p>
                    <sl-input type="number" onsl-change="{callback (fun (e: SlChangeEventArgs) -> e.Value |> string |> int |> setCount)}" value="{count}"></sl-input>
                    <script>
                        Blazor.registerCustomEventType('sl-change', {{
                            browserEventName: 'sl-change',
                            createEventArgs: event => {{
                                return {{
                                    value: event.target.value
                                }};
                            }}
                        }});
                    </script>


                    <sl-input type="number" oninput="{fun (e: ChangeEventArgs) -> e.Value |> string |> int |> setCount}" value="{count}"></sl-input>

                    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.62/dist/themes/light.css">
                    <script type="module" src="https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.62/dist/shoelace.js"></script>

                    <p>style or script tag will be rendered as raw 'html'</p>
                    <style>
                        .test-class {{
                            background-color: red;
                        }}
                    </style>
                    <script>
                        function test() {{
                            return false;
                        }}
                    </script>
                </div>
                """
        }
