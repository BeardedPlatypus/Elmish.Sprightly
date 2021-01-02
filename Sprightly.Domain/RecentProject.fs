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
