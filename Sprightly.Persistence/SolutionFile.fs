namespace Sprightly.Persistence

open Sprightly.Common
open Sprightly.Common.Path

///<summary>
/// <see cref="SolutionFile"/> defines the types and functions related
/// to accessing a solution file on disk.
/// </summary>
module SolutionFile =
    /// <summary>
    /// <see cref="currentFileVersion"> defines the current file version.
    /// </summary>
    let private currentFileVersion : Version.T =
        { Major = 0
          Minor = 1
          Patch = 0
        }

    /// <summary>
    /// <see cref="DataAccessRecord"/> describes the Data Access version of 
    /// the solution file.
    /// </summary>
    type public DataAccessRecord = 
        { FileVersion: string 
          Textures: Texture.DataAccessRecord list
        }

    /// <summary>
    /// Write the specified <paramref name="solutionFile"/> to the specified path.
    /// </summary>
    /// <param name="solutionFile">The solution file to write to disk.</param>
    /// <param name="path">The path to write to. </param>
    let write (solutionFile: DataAccessRecord ) (path: Path.T) : unit = 
        Json.serialize solutionFile 
        |> ( Json.writeJsonString path )


    /// <summary>
    /// Read the <see cref="DataAccessRecord"/> at the specified <paramref name="path"/>
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <returns>
    /// The <see cref="DataAccessRecord"/> at the specified <paramref name="path"/>
    /// if it was read succesfully, else <see cref="Option.None"/>.
    /// </returns>
    let read (path: Path.T) : DataAccessRecord option =
        let result = Json.readJsonString path 
                     |> Json.deserialize<DataAccessRecord>
        
        match result with 
        | Result.Ok v -> Some v
        | _           -> None

    /// <summary>
    /// Create an empty document.
    /// </summary>
    // TODO: refactor this.
    let private emptyRecord : DataAccessRecord = 
        { FileVersion = ( currentFileVersion |> Version.toString )
          Textures = []
        }

    let public writeEmpty (path: Path.T) : unit = 
        write emptyRecord path

    /// <summary>
    /// <see cref="T"/> defines a single solution file.
    /// </summary>
    type public Description = 
        { FileName: string
          DirectoryPath: Path.T
        }

    /// <summary>
    /// Initialise a new <see cref="Description"/>.
    /// </summary>
    /// <param name="fileName">The solution file name.</param>
    /// <param name="directoryPath">The path to the parent directory.</param>
    /// <returns>
    /// A new <see cref="Description"/>
    /// </returns>
    let public description (fileName: string) (directoryPath: Path.T) : Description =
        { FileName = fileName
          DirectoryPath = directoryPath
        }

    /// <summary>
    /// Transform the provided <paramref name="description"/> to a 
    /// <see cref="Path.T"/>.
    /// </summary>
    /// <param name="description">The description to transform.</param>
    /// <returns>
    /// The path corresponding with the provided <paramref name="description"/>.
    /// </returns>
    let public descriptionToPath (description: Description): Path.T =
        description.DirectoryPath / (description.FileName |> Path.fromString)

    /// <summary>
    /// Transform the provided <paramref name="path"/> to a <see cref="Description"/>.
    /// </summary>
    /// <param name="path">The path to transform.</param>
    /// <returns>
    /// The description corresponding with the provided <paramref name="path"/>.
    /// </returns>
    let public pathToDescription (path: Path.T) =
        { FileName = Path.name path
          DirectoryPath = Path.parentDirectory path
        }

    /// <summary>
    /// Update the solution at <paramref name="slnPath"/> with the given 
    /// <paramref name="updateSln"/> function.
    /// </summary>
    /// <param name="updateSln">Function to update the solution with.</param>
    /// <param name="slnPath">Path of the solution file.</param>
    /// <remarks>
    /// If no (correct) file exists at <paramref name="slnPath"/> then nothing 
    /// will happen.
    /// </remarks>
    let public updateOnDisk (updateSln: DataAccessRecord -> DataAccessRecord) (slnPath: Path.T) : unit =
        let sln = read slnPath

        if sln.IsSome then 
            write (updateSln sln.Value) slnPath

    /// <summary>
    /// Update the <see cref="Store"/> in the solution file at <paramref name="slnPath"/>
    /// to the provided <paramref name="store"/>.
    /// </summary>
    /// <param name="store">The new <see cref="Store"/>.</param>
    /// <param name "slnPath">The path to the solution file.</param>
    /// <remarks>
    /// If no (correct) file exists at <paramref name="slnPath"/> then nothing 
    /// will happen.
    /// </remarks>
    let public updateTexturesOnDisk (textures: Texture.DataAccessRecord list) (slnPath: Path.T) : unit =
        let updateSolution (originalSolution: DataAccessRecord) : DataAccessRecord =
            { originalSolution with Textures = textures }

        updateOnDisk updateSolution slnPath
