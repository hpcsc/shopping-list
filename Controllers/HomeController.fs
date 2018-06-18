namespace shopping_list.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

[<Route("[controller]")>]
type HomeController () =
    inherit Controller()

    [<HttpGet>]
    member this.Index () =
        this.View()

    [<HttpGet("about")>]
    member this.About () =
        this.ViewData.["Message"] <- "Your application description page."
        this.View()

    [<HttpGet("contact")>]
    member this.Contact () =
        this.ViewData.["Message"] <- "Your contact page."
        this.View()

    [<HttpGet("error")>]
    member this.Error () =
        this.View();
