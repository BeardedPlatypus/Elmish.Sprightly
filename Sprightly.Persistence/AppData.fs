namespace Sprightly.Persistence

open Sprightly

/// <summary>
/// <see cref="AppData"/> defines the logic to read and write
/// the appData to disk.
/// </summary>
module public AppData =
    [<RequireQualifiedAccess>]
    module BaseData = 
        
        type T = 
            { Version : Version.T }

        let current : T= 
            { Version = { Major = 0; Minor = 1; Patch = 0} }

        type DAR = 
            { Version : string }

        let toDAR (appData : T) : DAR =
            { Version = appData.Version |> Version.toString }

        let fromDAR (dar : DAR) : T option = 
            let version = dar.Version |> Version.fromString
            Option.map (fun v -> { Version = v }) version

    let private fileName = "sprightly.json"

    let private appFolder : Common.Path.T = 
        System.Environment.SpecialFolder.LocalApplicationData
        |> System.Environment.GetFolderPath
        |> Common.Path.fromString

    let private appDataFolder = 
        Common.Path.combine appFolder ("Sprightly" |> Common.Path.fromString)

    let private appDataFilePath = 
        Common.Path.combine appDataFolder (Common.Path.fromString fileName)

    /// <summary>
    /// Initialise a new AppData file on disk.
    /// </summary>
    let public initialise () : unit =
        if not (Common.Path.exists appDataFilePath) then
            (BaseData.current |> BaseData.toDAR)
            |> Json.serialize
            |> Json.writeJsonString appDataFilePath

    /// <summary>
    /// Retrieve the field associated with the specified <paramref name="key"/> as type <typeparamref name="T">.
    /// </summary>
    /// <param name="key">The key to retrieve the associated data with.</param>
    /// <returns>
    /// The field value associated with the specified <paramref name="key"/>.
    /// </returns>
    let public retrieveField<'T> (key: string) : Result<'T, exn> =
        Json.readJsonString appDataFilePath
        |> Json.queryKey<'T> key

    /// <summary>
    /// Retrieve the field associated with the specified <paramref name="key"/> as type <typeparamref name="T">.
    /// If no value is associated with <paramref name="key"/> then <paramref name="defaultValue"/> will be returned.
    /// </summary>
    /// <param name="defaultValue">The default value to return if no key is defined.</param>
    /// <param name="key">The key to retrieve the associated data with.</param>
    /// <returns>
    /// The field value associated with the specified <paramref name="key"/> if data exists otherwise <paramref name="defaultValue"/>.
    /// </returns>
    let public retrieveFieldWithDefault<'T> (defaultValue: 'T) (key: string) : 'T =
        match retrieveField<'T> key with 
        | Ok v    -> v 
        | Error _ -> defaultValue

    /// <summary>
    /// Update the value associated with <paramref name="key"/> to the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="key">The key to update.</param>
    /// <param name="data">The new data associated with the key.</param>
    let public updateField<'T> (key: string) (data: 'T) : unit =    
        let newAppData = 
            Json.readJsonString appDataFilePath
            |> Json.updateKey key data

        match newAppData with 
        | Result.Ok newData -> Json.writeJsonString appDataFilePath newData
        | _                 -> ()
