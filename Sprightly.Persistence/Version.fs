namespace Sprightly.Persistence

/// <summary>
/// <see cref="Version"/> defines the methods working on the
/// version type.
/// </summary>
module public Version =
    /// <summary>
    /// <see cref="Version"/> defines a version as a 
    /// Major.Minor.Patch
    /// </summary>
    type T =
        { Major: int
          Minor: int
          Patch: int
        }

    /// <summary>
    /// Create a <see cref="string"/> from the specified <paramref name="version"/>.
    /// </summary>
    /// <param name="version>The version to transform. </param>
    /// <returns>
    /// A <see cref="string"/> corresponding with the provided <paramref name="version"/>.
    /// </returns>
    let public toString (version: T ) : string =
        [ version.Major; version.Minor; version.Patch ] 
        |> List.map (fun x -> x.ToString())
        |> List.fold (fun (acc: string) (x: string) -> acc + "." + x) ""
     

    /// <summary>
    /// Create a <see cref="Version"/> from the specified <paramref name="str"/> 
    /// if possible.
    /// </summary>
    /// <param name="str">The string to convert to a version</param>
    /// <returns>
    /// A <see cref="Version"/> corresponding with the provided <paramref name="str"/>.
    /// </returns>
    let public fromString (str: string) : T Option =
        let splitString = str.Split('.') |> ( Array.map System.Int32.TryParse ) 

        match splitString with
        | [| (true, major); (true, minor); (true, patch) |] ->
            Some { Major = major; Minor = minor; Patch = patch }
        | _ -> 
            None
