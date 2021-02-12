namespace Sprightly.Domain

open System
open Sprightly

/// <summary>
/// The <see cref="Domain.RecentProjec"/> module defines all conceptions and 
/// functionality related to managing recent pojects within the Sprightly
/// application. 
///
/// A recent project is an entity having a certain path on disk  and a date 
/// time of when it was last opened.
/// </summary>
module RecentProject = 
    /// <summary>
    /// The <see cref="Id"/> defines the identifier of a recent project.
    /// </summary>
    type public Id = | Id of int

    /// <summary>
    /// The <see cref="Data"/> defines the data associated with a recent 
    /// project, consisting of a <see cref="Common.Path.T"/> and a
    /// <see cref="System.DateTime"/> when it was last opened.
    /// </summary>
    type public Data = 
        { Path : Common.Path.T
          LastOpened : DateTime 
        }

    /// <summary>
    /// <see cref="RecentProject.T"/> defines a recent project entity
    /// with an <see cref="RecentProject.Id"/> and 
    /// <see cref="RecentProject.Data"/>.
    /// </summary>
    type public T = 
        { Id : Id
          Data : Data
        }

    /// <summary>
    /// Generate an <see cref="RecentProject.Id"/> unique within the provided
    /// <paramref name="recentProjects"/>.
    /// </summary>
    /// <param name="recentProjects">
    /// The list of recent projects in which a unique id should be generated.
    /// </param>
    /// <returns>
    /// An <see cref="RecentProject.Id"/> that does not exist yet within 
    /// <paramref name="recentProjects"/>.
    /// </returns>
    let generateUniqueId (recentProjects: T list) : Id =
        List.map (fun proj -> match proj.Id with Id v -> v) recentProjects 
        |> Set.ofList 
        |> Set.maxElement
        |> (fun maxId -> Id (maxId + 1))
