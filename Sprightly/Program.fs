namespace Sprightly

open Elmish
open Elmish.WPF

open Sprightly.Model

module public App =
    let unusedBinding<'a> : 'a =
    #if DEBUG
        failwith "Binding Assumption Violation: this binding should be unused."
    #else
        Unchecked.defaultof<'a>
    #endif

    /// This is a discriminated union of the available messages from the user interface
    type public Msg =
        | RequestNewProject
        | ReturnToStart

    type public CmdMsg = unit

    /// This is used to define the initial state of our application
    let public init () = { PageModel = StartingPage }, []

    /// This is the Reducer Elmish.WPF calls to generate a new model based on a message and an old model
    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | RequestNewProject -> 
            { PageModel = NewProjectPage }, []
        | ReturnToStart ->
            { PageModel = StartingPage }, []

    let public toCmd (cmdMsg: CmdMsg) = 
        Cmd.none

    let private toCommonPage (pageModel: PageModel) : Sprightly.Presentation.Common.PageType =
        match pageModel with 
        | StartingPage   -> Sprightly.Presentation.Common.PageType.StartingPage
        | NewProjectPage -> Sprightly.Presentation.Common.PageType.NewProjectPage

    /// Elmish uses this to provide the data context for your view based on a model
    let bindings () : Binding<Model, Msg> list = 
        [ "PageType" |> Binding.oneWay (fun (m: Model) -> m.PageModel |> toCommonPage)
          "RequestNewProjectPageCommand" |> Binding.cmd RequestNewProject
          //"PageModel" |> Binding.subModel(
          //                  (fun (m: Model) -> m),
          //                  (fun () -> [ "RequestNewProjectPageCommand" |> Binding.cmd RequestNewProject ]))
        ]
