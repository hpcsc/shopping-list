namespace shopping_list.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Bot.Connector
open Microsoft.AspNetCore.Authorization
open Microsoft.Extensions.Configuration

[<Route("api/[controller]")>]
[<ApiController>]
type MessagesController private () =
    inherit ControllerBase()
    
    new (configuration: IConfiguration) as this =
        MessagesController() then
        this._Configuration <- configuration
    
    [<HttpGet>]
    member this.Get () =
        let values = [|"value1"; "value2"|]
        ActionResult<string[]>(values)

    [<Authorize(Roles = "Bot")>]
    [<HttpPost>]
    member this.Post ([<FromBody>] activity: Activity) =
        async {
            let appCredentials = new MicrosoftAppCredentials(this._Configuration)
            let client = new ConnectorClient(new Uri(activity.ServiceUrl), appCredentials)
            let replyText = this.GetReplyFor activity.Type activity.Text
            let reply = activity.CreateReply replyText
             
            client.Conversations.ReplyToActivityAsync reply 
            |> Async.AwaitTask 
            |> ignore
        } |> Async.StartAsTask
        
        let values = [|"value1"; "value2"|]
        ActionResult<string[]>(values)
        
//        ActionResult<unit>()
//        Ok() |> ignore
        
    member private this.GetReplyFor activityType text =
        match activityType with
            | ActivityTypes.Message -> String.Format("echo: {0}", text)
            | _ -> String.Format("activity type: {0}", text)
        
        
    member val _Configuration : IConfiguration = null with get, set
