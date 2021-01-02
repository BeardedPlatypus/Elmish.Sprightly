namespace Sprightly.Presentation.Pages

open Elmish.WPF

open Sprightly

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
        | RequestStartPage
        | RequestOpenFilePicker

    type public CmdMsg = unit

    let public init () : Model = // * CmdMsg list = 
        { ProjectName = None 
          DirectoryPath = None 
          CreateNewDirectory = true
        }//, []

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | SetProjectName projectName ->
            { model with ProjectName = Some projectName }, []
        | SetDirectoryPath directoryPath -> 
            { model with DirectoryPath = Some directoryPath }, []
        | SetCreateNewDirectory createNewDirectory ->
            { model with CreateNewDirectory = createNewDirectory }, []
        | _ -> 
            model, []

    let public bindings () = 
        [ "ProjectName" |> Binding.twoWay(
            (fun (m: Model) -> m.ProjectName |> Option.defaultValue "" ),
            (fun (v: string) _ -> SetProjectName v))
          "DirectoryPath" |> Binding.twoWay(
            (fun (m: Model) -> m.DirectoryPath |> Option.map Common.Path.toString
                                               |> Option.defaultValue ""),
            (fun (v: string) _ -> Common.Path.fromString v |> SetDirectoryPath))
          "CreateNewDirectory" |> Binding.twoWay(
            (fun (m: Model) -> m.CreateNewDirectory),
            (fun (v: bool) _ -> SetCreateNewDirectory v))
        ]


