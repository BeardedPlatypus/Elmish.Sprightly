namespace Sprightly.Presentation.Pages

open Elmish.WPF

open Sprightly

module NewProjectPage =
    type public Model = 
        { ProjectName : string option
          DirectoryPath : Common.Path.T option
          CreateNewDirectory : bool
        }

    type public Msg = 
        | SetProjectName of string
        | SetCreateNewDirectory of bool
        | RequestStartPage
        | RequestOpenFilePicker

    type public CmdMsg = unit

    let init () : Model = // * CmdMsg list = 
        { ProjectName = None 
          DirectoryPath = None 
          CreateNewDirectory = true
        }//, []

    let update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | SetProjectName projectName ->
            { model with ProjectName = Some projectName }, []
        | SetCreateNewDirectory createNewDirectory ->
            { model with CreateNewDirectory = createNewDirectory }, []
        | _ -> 
            model, []

    let bindings () = 
        [ "ProjectName" |> Binding.twoWay(
            (fun (m: Model) -> m.ProjectName |> Option.defaultValue "" ),
            (fun (v: string) _ -> SetProjectName v))
          "CreateNewDirectory" |> Binding.twoWay(
            (fun (m: Model) -> m.CreateNewDirectory),
            (fun (v: bool) _ -> SetCreateNewDirectory v))
        ]


