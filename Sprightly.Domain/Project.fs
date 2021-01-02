namespace Sprightly.Domain

open Sprightly.Common
open Sprightly.Domain.Textures

/// <summary>
/// The <see cref="Project"/> type describes a single project.
/// </summary>
type Project = 
    { TextureStore : Texture.Store
      SolutionPath : Path.T
    }
