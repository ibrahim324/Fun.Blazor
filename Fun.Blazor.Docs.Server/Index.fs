namespace Fun.Blazor.Docs.Server

open Fun.Blazor


type Index () =
    inherit FunBlazorComponent()

    override _.Render() = Docs.Wasm.App.app

    static member page =
        Template.html $"""
            <!DOCTYPE html>
            <html>
            
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
                <title>Fun Blazor</title>
                <base href="/" />
            </head>
            
            <body>
                {Bolero.Server.Html.rootComp<Index>}
                <script src="_framework/blazor.server.js"></script>
                <link rel="stylesheet" href="css/google-font.css" />
                <link rel="stylesheet" href="css/drag-drop.css" />
                <link rel="stylesheet" href="_content/MudBlazor/MudBlazor.min.css" />
                <script src="_content/MudBlazor/MudBlazor.min.js"></script>
            
                <link rel="stylesheet" href="css/github-markdown.css" />
                <link rel="stylesheet" href="css/prism-night-owl.css" />
                <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/components/prism-core.min.js"></script>
                <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/plugins/autoloader/prism-autoloader.min.js"></script>
            
                <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/mobile-drag-drop@2.3.0-rc.2/default.css">
                <script src="https://cdn.jsdelivr.net/npm/mobile-drag-drop@2.3.0-rc.2/index.min.js"></script>
            
                <script src="_content/AntDesign/js/ant-design-blazor.js"></script>
                <script src="https://unpkg.com/@antv/g2plot@latest/dist/g2plot.min.js"></script>
                <script src="_content/AntDesign.Charts/ant-design-charts-blazor.js"></script>
            
                <script src="https://unpkg.com/@fluentui/web-components" type="module"></script>
            </body>
            
            </html>
        """
