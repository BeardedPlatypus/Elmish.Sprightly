namespace Sprightly.Persistence

open Sprightly

module public AppData =
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
            Json.writeJsonString appDataFilePath ""

    let public retrieveField<'T> (key: string) : Result<'T, exn> =
        Json.readJsonString appDataFilePath
        |> Json.queryKey<'T> key

    let public retrieveFieldWithDefault<'T> (defaultValue: 'T) (key: string) : 'T =
        match retrieveField<'T> key with 
        | Ok v    -> v 
        | Error _ -> defaultValue
