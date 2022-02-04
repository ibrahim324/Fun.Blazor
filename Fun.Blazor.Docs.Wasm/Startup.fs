open System
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Majorsoft.Blazor.WebAssembly.Logging.Console
open MudBlazor.Services
open Plk.Blazor.DragDrop
open Fun.Blazor.Docs.Wasm

let builder = WebAssemblyHostBuilder.CreateDefault(Environment.GetCommandLineArgs())
        
builder
    .AddFunBlazor("#app", app)
    .Logging.AddBrowserConsole().SetMinimumLevel(LogLevel.Debug)
    .Services
        .AddFunBlazorWasm()
        .AddAntDesign()
        .AddMudServices()
        .AddBlazorDragDrop()
    |> ignore
        
builder.Build().RunAsync() |> ignore
