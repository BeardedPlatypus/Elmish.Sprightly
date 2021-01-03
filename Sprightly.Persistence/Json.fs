namespace Sprightly.Persistence

open Sprightly.Common

/// <summary>
/// <see cref="Json"/> wraps the methods to serialize, deserialize, read and write
/// json records.
/// </summary>
module public Json =
    open Newtonsoft.Json

    /// <summary>
    /// <see cref="JsonString"/> defines a json string to use in this module.
    /// </summary>
    type public JsonString = string

    /// <summary>
    /// Serialize the provided <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>
    /// A serialized <see cref="JsonString"/> describing the provided
    /// <paramref name="obj"/>.
    /// </returns>
    let public serialize (obj: 'T) : JsonString =
        JsonConvert.SerializeObject obj

    /// <summary>
    /// Deserialize the provided <paramref name="str"/> into a <typeref="T">.
    /// </summary>
    /// <param name="str">The string to deserialize.</param>
    /// <returns>
    /// The deserialized object of type <typeref="T"/>.
    /// </returns>
    let public deserialize<'T> (str: JsonString) =
        try 
            JsonConvert.DeserializeObject<'T> str 
            |> Result.Ok
        with 
            | ex -> Result.Error ex

    /// <summary>
    /// Write the specified <paramref name="content"/> to the specified
    /// <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to write to.</param>
    /// <param name="content">The json string to write.</param>
    let public writeJsonString (path: Path.T) (content: JsonString) : unit =
        let parentDirectory = Path.parentDirectory path
        System.IO.Directory.CreateDirectory ( parentDirectory |> Path.toString ) |> ignore

        use stream = new System.IO.StreamWriter (path |> Path.toString)
        stream.Write(content) |> ignore

    /// <summary>
    /// Read the content from the file at the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <returns>
    /// The content of the file at the specified <paramref name="path"/>.
    /// </returns>
    let public readJsonString (path: Path.T) : JsonString =
        use stream = new System.IO.StreamReader (path |> Path.toString)
        stream.ReadToEnd()
