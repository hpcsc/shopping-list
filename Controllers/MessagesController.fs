namespace shopping_list.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Bot.Connector
open Microsoft.AspNetCore.Authorization
open Microsoft.Extensions.Configuration

open System.Reflection.Metadata
open Workflow
                    
[<Route("api/[controller]")>]
[<ApiController>]
type MessagesController (configuration: IConfiguration) =
    inherit ControllerBase()
    
    let handleCreateNewList command =
        printfn "handle create %A" command
        match command with
            | User (CreateNewList listName) -> Some "create new list"
            | _ -> None
            
    let handleSystemCommand command =
        printfn "system %A" command
        match command with
            | System _ -> Some "system command"
            | _ -> None
                
    let workflow = handleCreateNewList ||> handleSystemCommand
    
    [<HttpGet>]
    member this.Get () =
        this.Ok "hello messages controller"

    [<Authorize(Roles = "Bot")>]
    [<HttpPost>]
    member this.Post ([<FromBody>] activity: Activity) =                
        let appCredentials = new MicrosoftAppCredentials(configuration)
        let client = new ConnectorClient(new Uri(activity.ServiceUrl), appCredentials)
        let botCommand = this.ToBotCommand activity
        
        match workflow botCommand with
            | Some replyText ->
                async { 
                    let reply = activity.CreateReply replyText                                             
                    client.Conversations.ReplyToActivityAsync reply
                        |> Async.AwaitTask 
                        |> ignore
                } |> Async.StartAsTask |> ignore
            | _ -> ()    
        
        this.Ok ()
        
    member private this.ToBotCommand activity: BotCommand =
        match activity.Type with
            | ActivityTypes.Message -> 
                if activity.Text.StartsWith "/" || activity.Text.StartsWith "$" then
                    match activity.Text.Substring 1 with
                        | "create" -> User (CreateNewList <| activity.Text.Substring 7)
                        | _ -> User NotSupported
                else
                    NotACommand
            | ActivityTypes.ContactRelationUpdate ->
                if activity.Action = "add" then
                    System BotAddedToConversation
                else 
                    System BotRemovedFromConversation
            | _ -> NotACommand