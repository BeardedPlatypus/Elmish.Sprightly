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

            let inspector : Domain.Textures.Inspector = Persistence.Textures.InspectorImpl() :> Domain.Textures.Inspector
            let fRetrieveTextureData (storeId: int) 
                                     (texDescr: Application.Texture.TextureDescription) : Domain.Textures.Texture.T option =
                Persistence.Texture.loadDomainTexture
                    inspector
                    ((uint) storeId |> Domain.Textures.Texture.InternalStoreId.Id)
                    texDescr.Name
                    texDescr.Id
                    texDescr.Path

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

    let private saveStore (slnPath: Common.Path.T) (store: Domain.Textures.Texture.Store) : unit =
        let textureDARs = store |> List.map Persistence.Texture.toDataAccessRecord
        Persistence.SolutionFile.updateTexturesOnDisk textureDARs slnPath

    let private addTextureCmd (descr: Presentation.Pages.ProjectPage.AddTextureDescription) : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool () 
            
            let solutionDirectoryPath = Common.Path.parentDirectory descr.SolutionPath
            let fCopyTextureIntoSolution : Application.Texture.CopyTextureIntoSolutionFunc = 
                Persistence.Texture.copyTextureIntoTextureFolder solutionDirectoryPath
            
            let inspector : Domain.Textures.Inspector = Persistence.Textures.InspectorImpl() :> Domain.Textures.Inspector
            let fRetrieveTextureMetaData (path: Common.Path.T) : Domain.Textures.MetaData.T option = 
                inspector.ReadMetaData(path)

            let fLoadTexture (tex: Domain.Textures.Texture.T) : unit =
                ()

            return Application.Texture.addNewTextureToStore
                       fCopyTextureIntoSolution
                       fRetrieveTextureMetaData
                       fLoadTexture
                       ( saveStore descr.SolutionPath )
                       descr.TexturePath
                       descr.Store
                   |> Option.map (fun (id, store) -> (Msg.PageMsg (PageMsg.ProjectPage (Presentation.Pages.ProjectPage.UpdateStore ((Some (Presentation.Pages.ProjectPage.SelectedId.Texture id)), store) ) )))
                   |> Option.defaultValue NoOp
                
        } |> Cmd.OfAsync.result

    let private removeTextureCmd (slnPath: Common.Path.T)
                                 (id: Domain.Textures.Texture.InternalStoreId) 
                                 (store: Domain.Textures.Texture.Store ) : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()

            let newStore =
                Application.Texture.removeTextureFromStore
                    Persistence.Texture.removeTextureFromSolution
                    (fun _ -> ())
                    (saveStore slnPath)
                    id 
                    store

            return Msg.PageMsg (PageMsg.ProjectPage (Presentation.Pages.ProjectPage.UpdateStore (None, newStore)))
        } |> Cmd.OfAsync.result

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
