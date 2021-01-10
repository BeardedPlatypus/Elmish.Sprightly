namespace Sprightly

open Elmish
open Elmish.WPF

open Sprightly

/// <summary>
/// The <see cref="CmdMapping"/> module takes care of mapping the <see cref="Sprightly.Presentation.Main.CmdMsg"/>
/// to their corresponding commands. This takes care of the inversion of control, such that the Presentation layer
/// does not need to call the other layers directly.
/// </summary>
module public CmdMapping =
    open Sprightly.Presentation.Main

    let private loadRecentProjectsCmd () : Cmd<Msg> = 
        Cmd.OfFunc.perform 
            (Application.Project.loadRecentProjects Persistence.RecentProject.loadRecentProjects)
            ()
            (Msg.PageMsg << PageMsg.StartingPage << Presentation.Pages.StartingPage.UpdateRecentProjects)

    let private createNewSolutionCmd (newSolutionPath: Common.Path.T): Cmd<Msg> =
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

    let private toProjectPageCmd (slnPath: Common.Path.T) :  Cmd<Msg> =
        MoveToProjectPage slnPath |> Cmd.ofMsg

    let private loadProjectFromDiskCmd (path: Common.Path.T) : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()

            let textureFolder = Persistence.Texture.textureFolder (Common.Path.parentDirectory path)
            
            let toTextureDescription (t: Persistence.Texture.DataAccessRecord) : (Application.Texture.TextureDescription) =
                { Name = t.Name 
                  Id = { Str = t.idString; Index = t.idIndex }
                  Path = Common.Path.combine textureFolder (Common.Path.fromString t.FileName)
                }

            let fRetrieveTexturePathsFromSolution (p: Common.Path.T) : (Application.Texture.TextureDescription list) option =
                Persistence.SolutionFile.read p
                |> Option.map (fun dao -> dao.Textures |> List.map toTextureDescription)

            let fRetrieveTextureData (storeId: int) 
                                     (texDescr: Application.Texture.TextureDescription) : Domain.Textures.Texture.T option =
                // Todo
                { Domain.Textures.Texture.T.Id = Domain.Textures.Texture.InternalStoreId.Id ((uint) storeId)
                  Domain.Textures.Texture.T.Data = 
                    { Id = texDescr.Id 
                      Name = texDescr.Name |> Domain.Textures.Texture.Name
                      Path = texDescr.Path
                      MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                   Height = Domain.Textures.MetaData.Pixel 512
                                   DiskSize = Domain.Textures.MetaData.Size 12.0
                                 }
                      Sprites = []
                         }
                } |> Some

            let fLoadTexture (tex: Domain.Textures.Texture.T) : unit =
                ()

            return Application.Project.loadProject fRetrieveTexturePathsFromSolution
                                                   fRetrieveTextureData
                                                   fLoadTexture
                                                   path
                   |> Option.map (fun (proj: Domain.Project) -> Msg.PageMsg (PageMsg.ProjectPage (Presentation.Pages.ProjectPage.Msg.UpdateStore (None, proj.TextureStore))))
                   |> Option.defaultValue Msg.MoveToStartingPage
        } |>Cmd.OfAsync.result

    let private moveProjectToTopOfRecentProjectsCmd (recentProjectPath: Common.Path.T) =
        async {
            do! Async.SwitchToThreadPool ()

            Application.Project.moveProjectToTopOfRecentProjects 
                Persistence.RecentProject.loadRecentProjects
                Persistence.RecentProject.saveRecentProjects
                recentProjectPath
            return NoOp
        } |> Cmd.OfAsync.result

    let private loadProjectCmd (slnPath: Common.Path.T) =
        Cmd.batch (seq { loadProjectFromDiskCmd slnPath; moveProjectToTopOfRecentProjectsCmd slnPath })

    let private mapStartingPageCmd : (Presentation.Pages.StartingPage.CmdMsg -> Cmd<Msg>) = 

        Presentation.Pages.StartingPage.toCmd
            (Msg.PageMsg << PageMsg.StartingPage)
            toProjectPageCmd
            (fun () -> Cmd.ofMsg MoveToNewProjectPage)
            loadRecentProjectsCmd

    let private mapNewProjectPageCmd : (Presentation.Pages.NewProjectPage.CmdMsg -> Cmd<Msg>) =
        Presentation.Pages.NewProjectPage.toCmd
            (Msg.PageMsg << PageMsg.NewProjectPage)
            (fun () -> Cmd.ofMsg MoveToStartingPage)
            createNewSolutionCmd

    let private addTextureCmd (path: Common.Path.T) (store: Domain.Textures.Texture.Store) : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool () 
            
            let fCopyTextureIntoSolution : Application.Texture.CopyTextureIntoSolutionFunc = 
                fun (_) -> Some (Common.Path.T "temp")

            let fRetrieveTextureMetaData : Application.Texture.RetrieveTextureMetaDataFunc = 
                fun (_) ->
                    Some { Width = Domain.Textures.MetaData.Pixel 256 
                           Height = Domain.Textures.MetaData.Pixel 512
                           DiskSize = Domain.Textures.MetaData.Size 12.0
                         }

            let fLoadTexture (tex: Domain.Textures.Texture.T) : unit =
                ()

            let fSaveStore (store: Domain.Textures.Texture.Store) : unit =
                ()

            return Application.Texture.addNewTextureToStore
                       fCopyTextureIntoSolution
                       fRetrieveTextureMetaData
                       fLoadTexture
                       fSaveStore
                       path 
                       store
                   |> Option.map (fun (id, store) -> (Msg.PageMsg (PageMsg.ProjectPage (Presentation.Pages.ProjectPage.UpdateStore ((Some (Presentation.Pages.ProjectPage.SelectedId.Texture id)), store) ) )))
                   |> Option.defaultValue NoOp
                
        } |> Cmd.OfAsync.result

    let private removeTextureCmd (id: Domain.Textures.Texture.InternalStoreId) (store: Domain.Textures.Texture.Store ) : Cmd<Msg> =
        let newStore = List.filter (fun (t: Domain.Textures.Texture.T) -> t.Id <> id) store
        Cmd.ofMsg (Msg.PageMsg (PageMsg.ProjectPage (Presentation.Pages.ProjectPage.UpdateStore (None, newStore))))

    let private mapProjectPageCmd : (Presentation.Pages.ProjectPage.CmdMsg -> Cmd<Msg>) =
        Presentation.Pages.ProjectPage.toCmd
            (Msg.PageMsg << PageMsg.ProjectPage)
            loadProjectCmd
            addTextureCmd
            removeTextureCmd

    let private mapPageCmd (cmdMsg: PageCmdMsg) : Cmd<Msg> = 
        match cmdMsg with 
        | PageCmdMsg.StartingPage startingPageCmdMsg ->
            mapStartingPageCmd startingPageCmdMsg
        | PageCmdMsg.NewProjectPage newProjectPageCmdMsg ->
            mapNewProjectPageCmd newProjectPageCmdMsg
        | PageCmdMsg.ProjectPage projectPageCmdMsg ->
            mapProjectPageCmd projectPageCmdMsg

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
            toProjectPageCmd slnPath
        | CmdMsg.PageCmdMsg pageCmdMsg ->
            mapPageCmd pageCmdMsg
