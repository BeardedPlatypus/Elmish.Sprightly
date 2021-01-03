namespace Sprightly.Persistence

open Sprightly

/// <summary>
/// The <see cref="DataAccess.Texture"/> module provides all methods related to
/// to accessing texture information on disk.
/// </summary>
module public Texture =
    /// <summary>
    /// <see cref='DataAccessRecord"/> defines the data acces information of a 
    /// single texture.
    /// </summary>
    type public DataAccessRecord =
        { Name : string 
          FileName : string
          idString : string 
          idIndex : uint
        }

    /// <summary>
    /// Convert the specified <paramref name="tex"/> to its corresponding 
    /// <see cref="DataAccessRecord"/>.
    /// </summary>
    /// <param name="tex">The texture to convert.</param>
    /// <returns>
    /// The corresponding <see cref="DataAccessRecord"/>.
    /// <returns>
    let public toDataAccessRecord (tex: Domain.Textures.Texture.T) : DataAccessRecord =
        let idString, idIndex = match tex.Id with | Domain.Textures.Texture.Id (idString, idIndex) -> idString, idIndex
        { Name = match tex.Data.Name with | Domain.Textures.Texture.Name name -> name
          FileName = tex.Data.Path |> Common.Path.name
          idString = idString
          idIndex = idIndex
        }

    open Common.Path
    /// <summary>
    /// Get the texture folder based on the provided <paramref name="solutionDirectoryPath"/>.
    /// </summary>
    /// <param name="solutionDirectoryPath">Path to to the solution directory.</param>
    /// <returns>
    /// The folder containing all texture files.
    /// <returns>
    let public textureFolder (solutionDirectoryPath: Common.Path.T) : Common.Path.T =
        solutionDirectoryPath / (fromString "Textures")

    /// <summary>
    /// Create an empty texture folder in the specified <paramref name="solutionDirectoryPath"/>.
    /// </summary>
    /// <param name="solutionDirectoryPath">Path to to the solution directory.</param>
    let public createTextureFolder (solutionDirectoryPath: Common.Path.T) : unit =
        let textureFolder = textureFolder solutionDirectoryPath

        if not <| Common.Path.exists textureFolder then
            System.IO.Directory.CreateDirectory(textureFolder |> Common.Path.toString) |> ignore

    open Domain.Textures.Texture
    /// <summary>
    /// Load the <see cref="Domain.Texture.T"/> from the specified <paramref name="texturePath"/>.
    /// </summary>
    /// <param name="inspector">The texture inspector to retrieve the metadata.</param>
    /// <param name="name">The name of the new texture.</param>
    /// <param name="id">The id of the new texture.</param>
    /// <param name="texturePath">The texture path.</param>
    /// <returns>
    /// The <see cref="Domain.Texture.T"/> if one can be read correctly from the specified
    /// <paramref name="texturePath"/>, else <see cref="Option.None"/>.
    /// </returns>
    let public loadDomainTexture (inspector: Domain.Textures.Inspector)
                                 (name: string)
                                 (id: Id)
                                 (texturePath: Common.Path.T) : T option =
        let metaData = inspector.ReadMetaData texturePath

        metaData |> Option.map (fun md -> { Id = id
                                            Data = { Name = name |> Name
                                                     Path = texturePath
                                                     MetaData= md
                                                   }
                                          })

    /// <summary>
    /// Copy the specified <paramref name="texturePath"/> into the texture folder of
    /// the solution at <paramref name="slnDirectoryPath"/> and return the new path.
    /// </summary>
    /// <param name="slnDirectoryPath">The path to the directory containing the solution.</param>
    /// <param name="texturePath">The path to the texture external of the solution.</param>
    /// <returns>
    /// The path to the copied texture if one was copied.
    /// </returns>
    let public copyTextureIntoTextureFolder (slnDirectoryPath: Common.Path.T)
                                            (texturePath: Common.Path.T) : Common.Path.T option =
        if Common.Path.exists texturePath &&
           Common.Path.exists slnDirectoryPath then 
            let texFolder = textureFolder slnDirectoryPath
            let name = Common.Path.name texturePath
            let newPath = 
                Common.Path.combine texFolder ((Common.Path.generateUniqueName texFolder name) |> Common.Path.fromString)

            System.IO.File.Copy(texturePath |> Common.Path.toString,
                                newPath |> Common.Path.toString)

            Some newPath
        else 
            None
