module Fun.Blazor.Tests.RouterTests

open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open Microsoft.Extensions.DependencyInjection
open Moq
open Xunit
open Bunit
open Fun.Blazor
open Fun.Blazor.Router


let private createTestContext () =
    let textContext = new TestContext()

    textContext.Services.AddScoped<INavigationInterception>(fun _ -> Mock.Of<INavigationInterception>())
    |> ignore

    textContext


[<Fact>]
let ``Giraffe style routes normal cases`` () =
    let formatQueries =
        Map.toList >> List.sortBy fst >> List.map (fun (k, v) -> $"{k}={v}") >> String.concat "&"

    let route =
        html.route [
            routeCi "/r1" (html.text "/r1")
            routeCif "/r1/%i" (fun x -> html.text $"/r1/{x}")
            routeCif "/r1/r2/%s" (fun x -> html.text $"/r1/r2/{x}")
            subRouteCi "/r2" [
                routeCi "/r3" (html.text "/r2/r3")
                routeCif "/r3/%i" (fun x -> html.text $"/r2/r3/{x}")
                routeCif "/r3/r4/%s" (fun x -> html.text $"/r2/r3/r4/{x}")
            ]
            routeCiWithQueries "/r3" (fun x -> html.text $"/r3?{formatQueries x}")
            routeCifWithQueries "/r3/%i" (fun x q -> html.text $"/r3/{x}?{formatQueries q}")

            routeCi "/citest" (html.text "/CiTest")

            routeCi "/application/greate test1" (html.text "/application/greate%20test1")
            routeCif
                "/application/%s"
                (fun x ->
                    if x = "greate test2" then
                        html.text "/application/greate%20test2"
                    else
                        html.text "failed"
                )

            routeCif "/tail/%s{*}" (fun _ -> html.text $"/tail/1/tail")
            routeCi "/tail2{*}" (html.text $"/tail2/2/tail2")

            fun x -> failwith $"No route matched for {x}"
        ]

    use testContext = createTestContext ()

    let testRoute url =
        testContext.Services.GetService<NavigationManager>().NavigateTo(url)
        let result = testContext.RenderNode route
        result.MarkupMatches(url)

    testRoute "/r1"
    testRoute "/r1/1"
    testRoute "/r1/r2/hi"
    testRoute "/r2/r3"
    testRoute "/r2/r3/3"
    testRoute "/r2/r3/r4/hi"
    testRoute "/r3?a=a1&b=123"
    testRoute "/r3/3?a=a1&b=123"
    testRoute "/CiTest"
    testRoute "/application/greate%20test1"
    testRoute "/application/greate%20test2"
    testRoute "/tail/1/tail"
    testRoute "/tail2/2/tail2"


[<Fact>]
let ``Giraffe style routes should work with no key pages`` () =
    let withNoKey1 = html.injectWithNoKey (fun () -> html.text "/withNoKey1")

    let withNoKey2 = html.injectWithNoKey (fun () -> html.text "/withNoKey2")

    let node = div {
        html.route [ routeCi "/withNoKey1" withNoKey1; routeCi "/withNoKey2" withNoKey2 ]
        html.inject (fun (nav: NavigationManager) -> fragment {
            button {
                id "withNoKey1"
                on.click (fun _ -> nav.NavigateTo("/withNoKey1"))
            }
            button {
                id "withNoKey2"
                on.click (fun _ -> nav.NavigateTo("/withNoKey2"))
            }
        })
    }

    use testContext = createTestContext ()
    let result = testContext.RenderNode node

    result.Find("#withNoKey1").Click()
    result.MarkupMatches
        """
        <div>
            /withNoKey1
            <button id="withNoKey1" ></button>
            <button id="withNoKey2" ></button>
        </div>
        """

    result.Find("#withNoKey2").Click()
    result.MarkupMatches
        """
        <div>
            /withNoKey2
            <button id="withNoKey1" ></button>
            <button id="withNoKey2" ></button>
        </div>
        """

    result.Find("#withNoKey1").Click()
    result.MarkupMatches
        """
        <div>
            /withNoKey1
            <button id="withNoKey1" ></button>
            <button id="withNoKey2" ></button>
        </div>
        """
