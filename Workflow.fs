module Workflow

type SystemBotCommand =
    | BotAddedToConversation
    | BotRemovedFromConversation
    | Others
    
type UserBotCommand =
    | CreateNewList of string
    | NotSupported
    
type BotCommand =
    | User of UserBotCommand
    | System of SystemBotCommand
    | NotACommand
    
type Workflow = BotCommand -> Option<string>

//let nullWorkflow (c: BotCommand) = None

let chooseFirst (a: Workflow) (b: Workflow) : Workflow =
     fun c ->
        match (a c) with
            | Some result -> Some result
            | None -> b c
            
let (||>) = chooseFirst