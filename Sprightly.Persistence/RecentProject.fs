namespace Sprightly.Persistence

open System
open Sprightly

/// <summary>
/// <see cref="RecentProject"/> module defines the methods related
/// to recent projects.
/// </summary>
module public RecentProject =
    type DAR = 
        { Path : string 
          LastOpened : DateTime
        }

    let toDAR (data: Domain.RecentProject.Data) : DAR =
        { Path = data.Path |> Common.Path.toString 
          LastOpened = data.LastOpened
        }

    let fromDAR (data: DAR) : Domain.RecentProject.Data = 
        { Path = data.Path |> Common.Path.fromString
          LastOpened = data.LastOpened 
        }

    let private recentProjectsKey = "recent_projects"

    /// <summary>
    /// Load the recent projects from the appdata.
    /// </summary>
    /// <returns>
    /// The list of <see cref="RecentProject.T"/> records if they can
    /// be retrieved from the app data, an empty list otherwise.
    /// </returns>
    let public loadRecentProjects () : Domain.RecentProject.T list =
        let data = AppData.retrieveFieldWithDefault<DAR list> [] recentProjectsKey
                   |> List.map fromDAR
        List.mapi ( fun (i: int) (data: Domain.RecentProject.Data) -> { Id = (Domain.RecentProject.Id i); Data = data }) data

    /// <summary>
    /// Save the specified <paramref name="recentProjects.T"/> to the app properties.
    /// </summary>
    /// <param name="recentProjects">The recent projects to save.</param>
    let public saveRecentProjects (recentProjects: Domain.RecentProject.T list) : unit =
        try 
            let darList = recentProjects |> List.map (fun rp -> toDAR rp.Data)
            AppData.updateField recentProjectsKey darList
         with _ -> 
             do ()
