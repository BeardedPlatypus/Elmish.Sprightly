namespace Sprightly.Domain

open System
open Sprightly

/// <summary>
/// The <see cref="RecentProject"/> describes a recent project.
/// </summary>
type public RecentProject = 
    { Path : Common.Path.T
      LastOpened : DateTime
    }

