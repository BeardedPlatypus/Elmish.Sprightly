namespace Sprightly.Domain

open System
open Sprightly

module RecentProject = 
    type public Id = | Id of int

    type public Data = 
        { Path : Common.Path.T
          LastOpened : DateTime 
        }

    type public T = 
        { Id : Id
          Data : Data
        }

    let generateUniqueId (recentProjects: T list) : Id =
        List.map (fun proj -> match proj.Id with Id v -> v) recentProjects 
        |> Set.ofList 
        |> Set.maxElement
        |> (fun maxId -> Id (maxId + 1))
        
