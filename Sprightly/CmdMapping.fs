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

    let private loadProjectFromDiskCmd (slnPath: Common.Path.T) :  Cmd<Msg> =
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

    let private loadProjectCmd (slnPath: Common.Path.T) =
        Cmd.batch (seq { loadProjectFromDiskCmd slnPath; moveProjectToTopOfRecentProjectsCmd slnPath })

    let private mapStartingPageCmd : (Presentation.Pages.StartingPage.CmdMsg -> Cmd<Msg>) = 

        Presentation.Pages.StartingPage.toCmd
            (Msg.PageMsg << PageMsg.StartingPage)
            loadProjectCmd    
            (fun () -> Cmd.ofMsg (MoveToPage PageModel.NewProjectPage))
            loadRecentProjectsCmd

    let private mapNewProjectPageCmd : (Presentation.Pages.NewProjectPage.CmdMsg -> Cmd<Msg>) =
        Presentation.Pages.NewProjectPage.toCmd
            (Msg.PageMsg << PageMsg.NewProjectPage)
            (fun () -> Cmd.ofMsg (MoveToPage PageModel.StartingPage))
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
