namespace Sprightly.Persistence

open Sprightly

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

    let public initialise () : unit =
        if not (Common.Path.exists appDataFilePath) then
            (BaseData.current |> BaseData.toDAR)
            |> Json.serialize
            |> Json.writeJsonString appDataFilePath

    let public retrieveField<'T> (key: string) : Result<'T, exn> =
        Json.readJsonString appDataFilePath
        |> Json.queryKey<'T> key

    let public retrieveFieldWithDefault<'T> (defaultValue: 'T) (key: string) : 'T =
        match retrieveField<'T> key with 
        | Ok v    -> v 
        | Error _ -> defaultValue
