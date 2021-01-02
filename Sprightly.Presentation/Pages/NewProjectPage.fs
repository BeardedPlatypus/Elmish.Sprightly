namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly
open Sprightly.Presentation

module public NewProjectPage =
    type public Model = 
        { ProjectName : string option
          DirectoryPath : Common.Path.T option
          CreateNewDirectory : bool
        }

    type public Msg = 
        | SetProjectName of string
        | SetDirectoryPath of Common.Path.T
        | SetCreateNewDirectory of bool
        | SetFullPath of Common.Path.T
        | RequestOpenFilePicker
        | RequestMoveToStartingPage
        | RequestCreateNewProject
        | NoOp

    type public CmdMsg = 
        | OpenFilePicker
        | MoveToStartingPage
        | CreateNewProject of Model

    let private openFilePickerCmd () : Cmd<Msg> =
        let config = Components.Dialogs.FileDialogConfiguration(addExtension = true,
                                                                checkIfFileExists = false,
                                                                dereferenceLinks = true,
                                                                filter = "Sprightly solution files (*.sprightly)|*.sprightly|All files (*.*)|*.*",
                                                                filterIndex = 1, 
                                                                multiSelect = false,
                                                                restoreDirectory = false, 
                                                                title = "Select a new sprightly solution location")
        Components.Dialogs.FileDialog.showDialogCmd 
            SetFullPath
            (fun _ -> NoOp)
            (fun _ -> NoOp)
            Components.Dialogs.FileDialog.DialogType.Open
            config

    let public toCmd (toParentCmd : Msg -> 'ParentMsg)
                      (moveToStartingPageCmd : unit -> Cmd<'ParentMsg>)
                      (createNewProjectCmd : Model -> Cmd<'ParentMsg>)
                      (cmdMsg: CmdMsg) : Cmd<'ParentMsg> =
        match cmdMsg with 
        | OpenFilePicker -> 
            openFilePickerCmd () |> Cmd.map toParentCmd
        | MoveToStartingPage ->
            moveToStartingPageCmd ()
        | CreateNewProject model ->
            createNewProjectCmd model

    let public init () : Model * CmdMsg list = 
        { ProjectName = None 
          DirectoryPath = None 
          CreateNewDirectory = true
        }, []

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | SetProjectName projectName ->
            { model with ProjectName = Some projectName }, []
        | SetDirectoryPath directoryPath -> 
            { model with DirectoryPath = Some directoryPath }, []
        | SetCreateNewDirectory createNewDirectory ->
            { model with CreateNewDirectory = createNewDirectory }, []
        | SetFullPath path ->
            { model with ProjectName = Some (Common.Path.name path)
                         DirectoryPath = Some (Common.Path.parentDirectory path)
            }, []
        | RequestOpenFilePicker -> 
            model, [ OpenFilePicker ]
        | RequestMoveToStartingPage ->
            model, [ MoveToStartingPage ]
        | RequestCreateNewProject ->
            model, [ CreateNewProject model ]
        | NoOp -> 
            model, []

    let private isValidNewSolutionDescription (model: Model): bool =
        match model.DirectoryPath, model.ProjectName with
        | Some directoryPath, Some projectName ->
            Common.Path.isValid directoryPath &&
            Common.Path.isRooted directoryPath && 
            projectName.Length > 0
        | _ -> 
            false

    let public bindings () = 
        [ // Field Bindings
          "ProjectName" |> Binding.twoWay(
            (fun (m: Model) -> m.ProjectName |> Option.defaultValue "" ),
            (fun (v: string) _ -> SetProjectName v))
          "DirectoryPath" |> Binding.twoWay(
            (fun (m: Model) -> m.DirectoryPath |> Option.map Common.Path.toString
                                               |> Option.defaultValue ""),
            (fun (v: string) _ -> Common.Path.fromString v |> SetDirectoryPath))
          "CreateNewDirectory" |> Binding.twoWay(
            (fun (m: Model) -> m.CreateNewDirectory),
            (fun (v: bool) _ -> SetCreateNewDirectory v))

          // Command Bindings
          "RequestOpenFilePicker" |> Binding.cmd RequestOpenFilePicker
          "RequestStartPageCommand" |> Binding.cmd RequestMoveToStartingPage
          "RequestNewProjectCommand" |> Binding.cmdIf 
            (fun m -> if isValidNewSolutionDescription m then Some RequestCreateNewProject else None )
        ]
