namespace Sprightly.Common

/// <summary>
/// A Path module that adds some convenience over the <see cref="System.IO.Path"/>
/// class.
/// </summary>
module public Path = 
    /// <summary>
    /// The base path object.
    /// </summary>
    type T = T of string

    /// <summary>
    /// Construct a new <see cref="Path.T"/> from the provided string.
    /// </summary>
    /// <param name="s">The string from which to create a <see cref="Path.T"/>.</param>
    /// <returns>
    /// A new <see cref="Path.T"/>.
    /// </returns>
    let fromString (s: string) : T = T s

    let toString (path: T) : string = match path with | T s -> s

    /// <summary>
    /// Combine <paramref name="parent"/> and <paramref name="child"/> into a 
    /// single <see cref="Path.T"/>.
    /// </summary>
    /// <param name="parent">The parent path.</param>
    /// <param name="child">The relative child path. </param>
    /// <returns>
    /// The combined <see cref="Path.T"/>.
    /// </returns>
    let combine (parent: T) (child: T) : T = 
        System.IO.Path.Combine [| (parent |> toString) ; (child |> toString) |] |> T

    /// <summary>
    /// Combine <paramref name="parent"/> and <paramref name="child"/> into a 
    /// single <see cref="Path.T"/>.
    /// </summary>
    /// <param name="parent">The parent path.</param>
    /// <param name="child">The relative child path. </param>
    /// <returns>
    /// The combined <see cref="Path.T"/>.
    /// </returns>
    let (/) (parent: T) (child: T) : T = combine parent child

    /// <summary>
    /// Get the name of provided <paramref name="path"/> as a string.
    /// </summary>
    /// <param name="path">The path from which to obtain the name. </param>
    /// <returns>
    /// The name of the directory or file to which <paramref name="path"/> points.
    /// </returns>
    let name (path: T) : string = 
        System.IO.Path.GetFileName (toString path)

    /// <summary>
    /// Get the name without extension of provided <paramref name="path"/> as a string.
    /// </summary>
    /// <param name="path">The path from which to obtain the name. </param>
    /// <returns>
    /// The name without extension of the directory or file to which <paramref name="path"/> points.
    /// </returns>
    let nameWithoutExtension (path: T) : string = 
        System.IO.Path.GetFileNameWithoutExtension (toString path)

    /// <summary>
    /// Get the extension of the provided <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path from which to obtain the extension. </param>
    /// <returns>
    /// The extension of the <paramref name="path"/>.
    /// </returns>
    let extension (path: T) : string = 
        System.IO.Path.GetExtension (toString path)

    /// <summary>
    /// Get the parent directory of the provided <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path from which to obtain the name. </param>
    /// <returns>
    /// The parent directory of the directory or file to which <paramref name="path"/> points.
    /// </returns>
    let parentDirectory (path: T) : T = 
        ( System.IO.Path.GetDirectoryName ( path |> toString )) |> fromString

    /// <summary>
    /// Get whether <paramref name="path"/> is valid or not.
    /// </summary>
    /// <param name="path">The path to verify.</param>
    /// <returns>
    /// True if <paramref name="path"/> is valid; false otherwise.
    /// </returns>
    let isValid (path: T) : bool =
        let pathStr = toString path

        try 
            System.IO.Path.GetFullPath pathStr |> ignore
            true
        with 
        | _ -> false

    /// <summary>
    /// Get whether <paramref name="path"/> is root or not.
    /// </summary>
    /// <param name="path">The path to verify.</param>
    /// <returns>
    /// True if <paramref name="path"/> is rooted; false otherwise.
    /// </returns>
    let isRooted (path: T) : bool = 
        System.IO.Path.IsPathRooted (path |> toString)

    /// <summary>
    /// <see cref="OpenMode"/> defines whether a file should be opened to read
    /// or to write.
    /// </summary>
    type OpenMode = 
        | Read
        | Write

    /// <summary>
    /// Open the file at the specified <paramref name="path"/> with the given 
    /// <paramref name="mode"/>.
    /// </summary>
    let openFile (path: T) (mode: OpenMode) : System.IO.FileStream = 
        match mode with 
        | Read  -> System.IO.File.OpenRead (path |> toString)
        | Write -> System.IO.File.OpenWrite (path |> toString)

    /// <summary>
    /// Check whether the specified path exists as eihter a file or a directory.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>
    /// True if the specified path is either a file or directory; false otherwise.
    /// </returns>
    let exists (path: T) : bool =
        let pString = path |> toString

        System.IO.File.Exists pString || 
        System.IO.Directory.Exists pString

    /// <summary>
    /// Generate a unique name given the specified <paramref name="name"/>
    /// within the specified <paramref name="dir"/>.
    /// </summary>
    /// <param name="dir">The directory folder in which to generate the name.</param>
    /// <param name="name">The initial name.</param>
    /// <returns>
    /// <paramref name="name"/> if <paramref name="name"/> does not exist
    /// within <paramref name="dir"/>, otherwise a name with an appended 
    /// index.
    /// </returns>
    let generateUniqueName (dir: T) (name: string) : string =
        let path = combine dir (fromString name)

        if exists path then 
            let namePath = fromString name

            let nameWithoutExtension = nameWithoutExtension namePath
            let extension = extension namePath

            let rec generate (i: int): string = 
                let newName = nameWithoutExtension + "_" + i.ToString() + extension
                let newPath = combine dir (newName |> fromString)

                if exists newPath then 
                    generate (i + 1)
                else 
                    newName

            generate 1
        else 
            name 

