namespace Sprightly.Application

open Sprightly.Common
open Sprightly.Domain

/// <summary>
/// The <see cref="Project"/> module contains all logic for interacting with
/// projects, including retrieving recent projects, saving and loading existing
/// projects and creating new projects.
/// </summary>
module public Project =
    /// <summary>
    /// The description of the LoadRecentProjects function.
    /// </summary>
    type public LoadRecentProjectsFunc = unit -> (RecentProject.T list) option

    /// <summary>
    /// The description of the SaveRecentProjects function.
    /// </summary>
    type public SaveRecentProjectsFunc = RecentProject.T list -> unit

    /// <summary>
    /// Load the recent projects.
    /// </summary>
    /// <param name="fLoadRecentProjects">
    /// Function to load the recent projects.
    /// </param>
    /// <returns>
    /// The list of <see cref="RecentProject"/> records describing the recent lists
    /// ordered by date.
    /// </returns>
    let public loadRecentProjects (fLoadRecentProjects: LoadRecentProjectsFunc) () : (RecentProject.T list) option =
        fLoadRecentProjects ()

    /// <summary>
    /// Update the stored recent project such that the specified 
    /// <paramref name="recentProject"/> is moved to the top of the list.
    /// </summary>
    /// <param name="fLoadRecentProjects">
    /// Function to load the recent projects.
    /// </param>
    /// <param name="fSaveRecentProjects">
    /// Function to save the recent projects.
    /// </param>
    /// <param name="recentProject">
    /// The recent project to move to the top of recent project.
    /// </param>
    /// <remarks>
    /// If <paramref name="recentProject"/> exists within the recent 
    /// projects, it is moved to the top. If it does not exist within
    /// the recent project it is added as a new element to the top of
    /// the recent projects.
    /// </remarks>
    let public moveProjectToTopOfRecentProjects (fLoadRecentProjects: LoadRecentProjectsFunc) 
                                                (fSaveRecentProjects: SaveRecentProjectsFunc)
                                                (recentProject: RecentProject.T) : unit =
        fLoadRecentProjects () 
        |> Option.defaultValue []
        |> List.filter (fun x -> x.Id <> recentProject.Id)
        |> (fun l -> recentProject :: l)
        |> fSaveRecentProjects

    /// <summary>
    /// Function to retrieve texture paths from the solution file at the specified path.
    /// </summary>
    type public RetrieveTexturePathsFromSolutionFunc = Path.T -> (Texture.TextureDescription list) option

    /// <summary>
    /// Load the project at the given <paramref name="solutionFilePath"/>.
    /// </summary>
    /// <param name ="fRetrieveTexturePathsFromSolution"/>
    /// Function to retrieve the relevant texture paths from the <paramref name="solutionFilePath"/>.
    /// </param>
    /// <param name="fRetrieveTextureData>
    /// Function to retrieve the texture from the given texture path.
    /// </param>
    let public loadProject (fRetrieveTexturePathsFromSolution: RetrieveTexturePathsFromSolutionFunc)
                           (fRetrieveTextureData: Texture.RetrieveTextureDataFunc)
                           (fLoadTexture: Texture.LoadTextureFunc)
                           (solutionFilePath: Path.T) : Project option =
        
        let texturePaths = fRetrieveTexturePathsFromSolution solutionFilePath

        texturePaths |> Option.map (fun paths ->
            let textureStore : Textures.Texture.Store = 
                List.map fRetrieveTextureData paths
                |> List.choose id
                |> Textures.Texture.addTexturesToStore (Textures.Texture.emptyStore ())

            for t in textureStore do fLoadTexture t

            { TextureStore = textureStore
              SolutionPath = solutionFilePath
            })

    /// <summary>
    /// Function to write an empty solution file.
    /// </summary>
    type public WriteEmptySolutionFunc = Path.T -> unit

    /// <summary>
    /// Create a new empty solution file and corresponding file structure at
    /// the specified <paramref name="slnFile"/>.
    /// </summary>
    /// <param name="fWriteEmptySolution">Function to write an empty solution file.</param>
    /// <param name="slnFile">The path to the solution file to write.</param>
    let public createNewProject (fWriteEmptySolution) 
                                (slnFile: Path.T) : unit = 
        fWriteEmptySolution slnFile
