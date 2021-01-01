open System
open Elmish.WPF

open Sprightly.Presentation.Views

/// This is the application's entry point. It hands things off to Elmish.WPF
[<EntryPoint; STAThread>]
let main _ =
    Program.mkProgramWpfWithCmdMsg
        Sprightly.App.init 
        Sprightly.App.update 
        Sprightly.App.bindings
        Sprightly.App.toCmd
    |> Program.runWindow (MainView())
