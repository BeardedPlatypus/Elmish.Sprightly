open System
open Elmish.WPF

open Sprightly.Presentation.Views

/// This is the application's entry point. It hands things off to Elmish.WPF
[<EntryPoint; STAThread>]
let main _ =
    Program.mkProgramWpfWithCmdMsg
        Sprightly.Presentation.Main.init 
        Sprightly.Presentation.Main.update 
        Sprightly.Presentation.Main.bindings
        Sprightly.CmdMapping.toCmd
    |> Program.runWindow (MainView())
