namespace Sprightly.Presentation

open Elmish
open Elmish.WPF

open Sprightly

module Main =
    [<RequireQualifiedAccess>]
    type public PageModel = 
        | StartingPage
        | NewProjectPage
        | ProjectPage

    type public Model = 
        { PageModel : PageModel
          StartingPageModel : Presentation.Pages.StartingPage.Model
          NewProjectPageModel : Presentation.Pages.NewProjectPage.Model option
          ProjectPageModel : Presentation.Pages.ProjectPage.Model option
        }

    [<RequireQualifiedAccess>]
    type public PageMsg =
        | StartingPage of Presentation.Pages.StartingPage.Msg
        | NewProjectPage of Presentation.Pages.NewProjectPage.Msg
        | ProjectPage of Presentation.Pages.ProjectPage.Msg

    /// This is a discriminated union of the available messages from the user interface
    type public Msg =
        | InitialisationSuccess
        | InitialisationFailure of exn
        | RequestLoadProject of Common.Path.T
        | MoveToPage of PageModel
        | PageMsg of PageMsg
        | NoOp

    [<RequireQualifiedAccess>]
    type public PageCmdMsg =
        | StartingPage of Presentation.Pages.StartingPage.CmdMsg
        | NewProjectPage of Presentation.Pages.NewProjectPage.CmdMsg
        | ProjectPage of unit

    [<RequireQualifiedAccess>]
    type public CmdMsg = 
        | Initialise
        | LoadProject of Common.Path.T
        | PageCmdMsg of PageCmdMsg

    /// This is used to define the initial state of our application
    let public init () = 
        let startingPageModel, startingPageCmds = Presentation.Pages.StartingPage.init ()

        { PageModel = PageModel.StartingPage
          StartingPageModel = startingPageModel
          NewProjectPageModel = None
          ProjectPageModel = None
        }, [ CmdMsg.Initialise ] @ (List.map (CmdMsg.PageCmdMsg << PageCmdMsg.StartingPage) startingPageCmds)

    let updatePage (msg: PageMsg) (model: Model) : Model * CmdMsg list =
        match msg, model.PageModel with 
        | (PageMsg.StartingPage pageMsg, PageModel.StartingPage) ->
            let newPageModel, newPageCmdMsg = Presentation.Pages.StartingPage.update pageMsg model.StartingPageModel

            { model with StartingPageModel = newPageModel 
            }, List.map (CmdMsg.PageCmdMsg << PageCmdMsg.StartingPage) newPageCmdMsg
        | (PageMsg.NewProjectPage pageMsg, PageModel.NewProjectPage) when model.NewProjectPageModel.IsSome ->
            let newPageModel, newPageCmdMsgs = Presentation.Pages.NewProjectPage.update pageMsg model.NewProjectPageModel.Value

            { model with NewProjectPageModel = Some newPageModel 
            }, List.map (CmdMsg.PageCmdMsg << PageCmdMsg.NewProjectPage) newPageCmdMsgs
        | (PageMsg.ProjectPage pageMsg, PageModel.ProjectPage) when model.ProjectPageModel.IsSome -> 
            let newPageModel, _ = Presentation.Pages.ProjectPage.update pageMsg model.ProjectPageModel.Value

            { model with ProjectPageModel = Some newPageModel 
            }, []
        | _ ->
            model, []

    /// This is the Reducer Elmish.WPF calls to generate a new model based on a message and an old model
    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | InitialisationSuccess -> 
            model, [ CmdMsg.PageCmdMsg (PageCmdMsg.StartingPage Presentation.Pages.StartingPage.CmdMsg.LoadRecentProjects) ]
        | InitialisationFailure _ ->
            model, []
        | RequestLoadProject projPath ->
            model, [ CmdMsg.LoadProject projPath ]
        | MoveToPage pageModel -> 
            match pageModel with
            | PageModel.NewProjectPage -> 
                let newProjectPageModel, initCmds = 
                    model.NewProjectPageModel
                    |> Option.map (fun m -> m, [])
                    |> Option.defaultValue (Presentation.Pages.NewProjectPage.init ())
                { model with PageModel = pageModel 
                             NewProjectPageModel = Some newProjectPageModel 
                }, List.map (CmdMsg.PageCmdMsg << PageCmdMsg.NewProjectPage) initCmds
            | PageModel.ProjectPage ->
                let projectPageModel = Pages.ProjectPage.init (Common.Path.T "slnPath")

                { model with PageModel = pageModel
                             ProjectPageModel = Some projectPageModel
                }, []
            | _ ->
                { model with PageModel = pageModel }, []
        | PageMsg pageMsg ->
            updatePage pageMsg model
        | NoOp ->
            model, []

    let private toCommonPage (pageModel: PageModel) : Presentation.Common.PageType =
        match pageModel with 
        | PageModel.StartingPage   -> Presentation.Common.PageType.StartingPage
        | PageModel.NewProjectPage -> Presentation.Common.PageType.NewProjectPage
        | PageModel.ProjectPage    -> Presentation.Common.PageType.ProjectPage

    /// Elmish uses this to provide the data context for your view based on a model
    let bindings () : Binding<Model, Msg> list = 
        [ // Bindings
          "PageType" |> Binding.oneWay (fun (m: Model) -> m.PageModel |> toCommonPage)

          // SubViewModels
          "NewProjectPageModel" |> 
            Binding.subModelOpt(
                (fun (m: Model) -> m.NewProjectPageModel ), 
                (fun (_, m) -> m),
                Msg.PageMsg << PageMsg.NewProjectPage, 
                Presentation.Pages.NewProjectPage.bindings)

          "StartingPageModel" |>
            Binding.subModel(
                (fun (m: Model) -> m.StartingPageModel ),
                (fun (_, m) -> m), 
                Msg.PageMsg << PageMsg.StartingPage,
                Presentation.Pages.StartingPage.bindings)
          "ProjectPageModel" |> 
            Binding.subModelOpt(
                (fun (m: Model) -> m.ProjectPageModel ),
                (fun (_, m) -> m), 
                Msg.PageMsg << PageMsg.ProjectPage,
                Presentation.Pages.ProjectPage.bindings)
        ]
