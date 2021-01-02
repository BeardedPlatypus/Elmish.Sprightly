namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly

module public StartingPage =
    type public Model = 
        { RecentProjects : Domain.RecentProject.T list }

    type public RecentProjectMsg =
        | RequestOpenRecentProject

    type public Msg = 
        | RecentProjectMsg of Domain.RecentProject.Id * RecentProjectMsg
        | NoOp

    type public CmdMsg =
        | OpenProject of Common.Path.T

    let public toCmd (toParentCmd : Msg -> 'ParentMsg )
                     (openProjectCmd : Common.Path.T -> Cmd<'ParentMsg>) 
                     (cmdMsg: CmdMsg) : Cmd<'ParentMsg> =
        match cmdMsg with 
        | OpenProject path ->
            openProjectCmd path

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with
        | RecentProjectMsg (id, RequestOpenRecentProject) ->
            let recentProject = List.find (fun (e: Domain.RecentProject.T) -> e.Id = id) model.RecentProjects
            model, [ OpenProject recentProject.Data.Path ]
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
        ]
