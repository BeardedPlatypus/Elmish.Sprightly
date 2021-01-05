﻿namespace Sprightly

open Elmish
open Elmish.WPF

open Sprightly
open Sprightly.Model


module public App =
    [<RequireQualifiedAccess>]
    type public PageMsg =
        | StartingPage of Presentation.Pages.StartingPage.Msg
        | NewProjectPage of Presentation.Pages.NewProjectPage.Msg

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

    [<RequireQualifiedAccess>]
    type public CmdMsg = 
        | Initialise
        | LoadProject of Common.Path.T
        | PageCmdMsg of PageCmdMsg

    let loadRecentProjectsCmd () : Cmd<Msg> = 
        Cmd.OfFunc.perform 
            (Application.Project.loadRecentProjects Persistence.RecentProject.loadRecentProjects)
            ()
            (Msg.PageMsg << PageMsg.StartingPage << Presentation.Pages.StartingPage.UpdateRecentProjects)

    let createNewSolutionCmd (newSolutionPath: Common.Path.T): Cmd<Msg> =
        async {    
            do! Async.SwitchToThreadPool ()
            
            let fWriteEmptySolution (p: Common.Path.T) = 
            
                let solutionFileDescription : Sprightly.Persistence.SolutionFile.Description = 
                    { FileName = Common.Path.name p
                      DirectoryPath = Common.Path.parentDirectory p
                    }

                Persistence.SolutionFile.writeEmpty (solutionFileDescription |> Sprightly.Persistence.SolutionFile.descriptionToPath)
                Persistence.Texture.createTextureFolder solutionFileDescription.DirectoryPath

            Application.Project.createNewProject fWriteEmptySolution newSolutionPath

            return RequestLoadProject newSolutionPath
        } |> Cmd.OfAsync.result

    let loadProjectFromDiskCmd (slnPath: Common.Path.T) :  Cmd<Msg> =
        MoveToPage PageModel.ProjectPage |> Cmd.ofMsg

    let private moveProjectToTopOfRecentProjectsCmd (recentProjectPath: Common.Path.T) =
        async {
            do! Async.SwitchToThreadPool ()

            Application.Project.moveProjectToTopOfRecentProjects 
                Persistence.RecentProject.loadRecentProjects
                Persistence.RecentProject.saveRecentProjects
                recentProjectPath
            return NoOp
        } |> Cmd.OfAsync.result

    let loadProjectCmd (slnPath: Common.Path.T) =
        Cmd.batch (seq { loadProjectFromDiskCmd slnPath; moveProjectToTopOfRecentProjectsCmd slnPath })

    let private mapStartingPageCmd : (Presentation.Pages.StartingPage.CmdMsg -> Cmd<Msg>) = 

        Presentation.Pages.StartingPage.toCmd
            (Msg.PageMsg << PageMsg.StartingPage)
            loadProjectCmd    
            (fun () -> Cmd.ofMsg (MoveToPage Model.PageModel.NewProjectPage))
            loadRecentProjectsCmd

    let private mapNewProjectPageCmd : (Presentation.Pages.NewProjectPage.CmdMsg -> Cmd<Msg>) =
        Presentation.Pages.NewProjectPage.toCmd
            (Msg.PageMsg << PageMsg.NewProjectPage)
            (fun () -> Cmd.ofMsg (MoveToPage Model.PageModel.StartingPage))
            createNewSolutionCmd

    let private mapPageCmd (cmdMsg: PageCmdMsg) : Cmd<Msg> = 
        match cmdMsg with 
        | PageCmdMsg.StartingPage startingPageCmdMsg ->
            mapStartingPageCmd startingPageCmdMsg
        | PageCmdMsg.NewProjectPage newProjectPageCmdMsg ->
            mapNewProjectPageCmd newProjectPageCmdMsg

    let private initialiseCmd () = 
        Cmd.OfFunc.either
            ( Application.App.initialise Persistence.AppData.initialise )
            ()
            ( fun () -> InitialisationSuccess )
            InitialisationFailure

    let public toCmd (cmdMsg: CmdMsg) : Cmd<Msg> =
        match cmdMsg with 
        | CmdMsg.Initialise ->
            initialiseCmd ()
        | CmdMsg.LoadProject slnPath ->
            loadProjectCmd slnPath
        | CmdMsg.PageCmdMsg pageCmdMsg ->
            mapPageCmd pageCmdMsg

    /// This is used to define the initial state of our application
    let public init () = 
        let startingPageModel, startingPageCmds = Presentation.Pages.StartingPage.init ()

        { PageModel = StartingPage
          StartingPageModel = startingPageModel
          NewProjectPageModel = None
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
            | _ ->
                { model with PageModel = pageModel }, []
        | PageMsg pageMsg ->
            updatePage pageMsg model
        | NoOp ->
            model, []

    let private toCommonPage (pageModel: PageModel) : Sprightly.Presentation.Common.PageType =
        match pageModel with 
        | Model.StartingPage   -> Sprightly.Presentation.Common.PageType.StartingPage
        | Model.NewProjectPage -> Sprightly.Presentation.Common.PageType.NewProjectPage
        | Model.ProjectPage    -> Sprightly.Presentation.Common.PageType.ProjectPage

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
        ]

            
            
