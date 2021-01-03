namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly
open Sprightly.Presentation

module public StartingPage =
    type public Model = 
        { RecentProjects : Domain.RecentProject.T list }

    type public RecentProjectMsg =
        | RequestOpenRecentProject

    type public Msg = 
        | RecentProjectMsg of Domain.RecentProject.Id * RecentProjectMsg
        | RequestMoveToNewProjectPage
        | RequestOpenProjectFilePicker
        | RequestOpenProject of Common.Path.T
        | UpdateRecentProjects of Domain.RecentProject.T list
        | NoOp

    type public CmdMsg =
        | OpenProjectFilePicker
        | OpenProject of Common.Path.T
        | MoveToNewProjectPage
        | LoadRecentProjects

    let public init () : Model * CmdMsg list  = { RecentProjects = [] }, []

    let private openProjectFilePickerCmd () =
        let config = Components.Dialogs.FileDialogConfiguration(addExtension = true,
                                                            checkIfFileExists = true,
                                                            dereferenceLinks = true,
                                                            filter = "Sprightly solution files (*.sprightly)|*.sprightly|All files (*.*)|*.*",
                                                            filterIndex = 1, 
                                                            multiSelect = false,
                                                            restoreDirectory = false, 
                                                            title = "Load a sprightly solution")
        Components.Dialogs.FileDialog.showDialogCmd
            RequestOpenProject
            (fun _ -> NoOp)
            (fun _ -> NoOp)
            Components.Dialogs.FileDialog.DialogType.Open
            config

    let public toCmd (toParentCmd : Msg -> 'ParentMsg )
                     (openProjectCmd : Common.Path.T -> Cmd<'ParentMsg>) 
                     (moveToNewProjectPageCmd : unit -> Cmd<'ParentMsg>)
                     (loadRecentProjectsCmd : unit -> Cmd<'ParentMsg>)
                     (cmdMsg: CmdMsg) : Cmd<'ParentMsg> =
        match cmdMsg with 
        | OpenProjectFilePicker ->
            openProjectFilePickerCmd () |> Cmd.map toParentCmd
        | OpenProject path ->
            openProjectCmd path
        | MoveToNewProjectPage ->
            moveToNewProjectPageCmd ()
        | LoadRecentProjects ->
            loadRecentProjectsCmd ()

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with
        | RecentProjectMsg (id, RequestOpenRecentProject) ->
            let recentProject = List.find (fun (e: Domain.RecentProject.T) -> e.Id = id) model.RecentProjects
            model, [ OpenProject recentProject.Data.Path ]
        | RequestMoveToNewProjectPage ->
            model, [ MoveToNewProjectPage ]
        | RequestOpenProjectFilePicker ->
            model, [ OpenProjectFilePicker ]
        | RequestOpenProject path ->
            model, [ OpenProject path ]
        | UpdateRecentProjects recentProjects ->
            { model with RecentProjects = recentProjects }, []
        | NoOp ->
            model, []

    let private selectedToMsg (i: int) (m: Model) =
            RecentProjectMsg ((m.RecentProjects |> List.item i).Id, RequestOpenRecentProject)

    let public bindings () = 
        [ "RecentProjects" |> Binding.subModelSeq(
            (fun (m: Model) -> m.RecentProjects),
            (fun (_, m) -> m), 
            (fun (e: Domain.RecentProject.T) -> e.Id), 
            (fun (id: Domain.RecentProject.Id, msg: RecentProjectMsg) -> RecentProjectMsg (id, msg)),
            fun () -> 
                [ "ProjectName" |> Binding.oneWay (fun m -> m.Data.Path |> Common.Path.name)
                  "ProjectDirectory" |> Binding.oneWay (fun m -> m.Data.Path |> Common.Path.parentDirectory 
                                                                             |> Common.Path.toString)
                  "LastOpened" |> Binding.oneWay (fun m -> $"{m.Data.LastOpened.ToShortDateString()} {m.Data.LastOpened.ToShortTimeString()}")
                ])

          // This is a bit of hack: 
          // we are abusing the SelectedIndex to notify when an element in the RecentProjects
          // is clicked, when this happens we load the correct project. 
          // Upon navigating to this page, nothing should be selected, and when something 
          // is selected we navigate away from the StartingPage, as such we always return -1,
          // i.e. nothing selected.
          "SelectedIndex" |> Binding.twoWay(
            (fun _ -> -1),
            selectedToMsg)

          // Dispatch commands
          "RequestNewProjectPageCommand" |> Binding.cmd RequestMoveToNewProjectPage
          "RequestOpenProjectFilePickerCommand" |> Binding.cmd RequestOpenProjectFilePicker
        ]
