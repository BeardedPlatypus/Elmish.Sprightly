open System
open Elmish.WPF

open Sprightly.Presentation.Views

type Model = {
  ClickCount: int
  Message: string
}

/// This is used to define the initial state of our application
let init() = {
  ClickCount = 0
  Message = "Hello Elmish.WPF"
}

/// This is a discriminated union of the available messages from the user interface
type MessageType =
  | ButtonClicked
  | Reset

/// This is the Reducer Elmish.WPF calls to generate a new model based on a message and an old model
let update (msg: MessageType) (model: Model) : Model =
  match msg with 
  | ButtonClicked -> { model with ClickCount = model.ClickCount + 1}
  | Reset -> init()

/// Elmish uses this to provide the data context for your view based on a model
let bindings () : Binding<Model, MessageType> list = [
  // One-Way Bindings
  "ClickCount" |> Binding.oneWay (fun m -> m.ClickCount)
  "Message" |> Binding.oneWay (fun m -> m.Message)

  // Commands
  "ClickCommand" |> Binding.cmd ButtonClicked
  "ResetCommand" |> Binding.cmd Reset
]

/// This is the application's entry point. It hands things off to Elmish.WPF
[<EntryPoint; STAThread>]
let main _ =
  Program.mkSimpleWpf init update bindings
  |> Program.runWindow (MainView())
