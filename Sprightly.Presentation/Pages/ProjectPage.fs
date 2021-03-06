﻿namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly
open Sprightly.Presentation

module public ProjectPage =
    [<RequireQualifiedAccess>]
    type public SelectedId =
        | Texture of Domain.Textures.Texture.InternalStoreId
        | Sprite of Domain.Textures.Texture.InternalStoreId * Domain.Sprite.Id

    type public Model =
        { SolutionPath : Common.Path.T
          TextureStore : Domain.Textures.Texture.Store
          Selected : SelectedId option
        }

    type public TextureDetailMsg =
        | UpdateName of Domain.Textures.Texture.Name
        | UpdateId of Domain.Textures.Texture.Id

    type public Msg = 
        | HasInitialised
        | ChangeSelected of SelectedId option
        | TextureDetailMsg of id: Domain.Textures.Texture.InternalStoreId * msg: TextureDetailMsg
        | RequestOpenTextureFilePicker
        | RequestAddTexture of Common.Path.T
        | RequestRemoveSelected
        | UpdateStore of store: Domain.Textures.Texture.Store * resetSelected: bool
        | NoOp

    /// <summary>
    /// The data required to add a texture to this project.
    /// </summary>
    type public AddTextureDescription = 
        { TexturePath: Common.Path.T
          Store: Domain.Textures.Texture.Store
          SolutionPath: Common.Path.T
        }

    type public CmdMsg =
        | LoadProject of Common.Path.T
        | OpenTextureFilePicker 
        | AddTexture of AddTextureDescription
        | RemoveTexture of slnPath: Common.Path.T * 
                           id: Domain.Textures.Texture.InternalStoreId * 
                           store: Domain.Textures.Texture.Store

    let private openProjectFilePickerCmd () =
        let config = Components.Dialogs.FileDialogConfiguration(addExtension = true,
                                                                checkIfFileExists = true,
                                                                dereferenceLinks = true,
                                                                filter = "Texture files (*.png)|*.png|All files (*.*)|*.*",
                                                                filterIndex = 1, 
                                                                multiSelect = false,
                                                                restoreDirectory = false, 
                                                                title = "Add a new texture")
        Components.Dialogs.FileDialog.showDialogCmd
            RequestAddTexture
            (fun _ -> NoOp)
            (fun _ -> NoOp)
            Components.Dialogs.FileDialog.DialogType.Open
            config

    let public toCmd (toParentCmd : Msg -> 'ParentMsg ) 
                     (loadProjectCmd : Common.Path.T -> Cmd<'ParentMsg>)
                     (addTextureCmd : AddTextureDescription -> Cmd<'ParentMsg>)
                     (removeTextureCmd: (Common.Path.T -> Domain.Textures.Texture.InternalStoreId -> Domain.Textures.Texture.Store -> Cmd<'ParentMsg>))
                     (cmdMsg: CmdMsg) : Cmd<'ParentMsg> =
        match cmdMsg with 
        | LoadProject path ->
            loadProjectCmd path
        | OpenTextureFilePicker ->
            openProjectFilePickerCmd () |> Cmd.map toParentCmd
        | AddTexture descr ->
            addTextureCmd descr
        | RemoveTexture (slnPath, id, store) ->
            removeTextureCmd slnPath id store

    let public init (slnPath: Common.Path.T) : Model * CmdMsg list = 
        { SolutionPath = slnPath
          TextureStore = []
          Selected = None
        }, []

    let private updateTextureDetail (id: Domain.Textures.Texture.InternalStoreId) 
                                    (msg: TextureDetailMsg) 
                                    (model: Model) : Model * 'CmdMsg list = 
        let updateFunc =
            match msg with 
            | UpdateName name ->
                fun (t: Domain.Textures.Texture.T) -> 
                    { t with Data = { t.Data with Name = name }}
            | UpdateId id -> 
                fun (t: Domain.Textures.Texture.T) -> {t with Data = { t.Data with Id = id}}

        let newStore = Domain.Textures.Texture.updateTextureInStore model.TextureStore id updateFunc
        { model with TextureStore =  newStore }, []

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | HasInitialised ->
            model, [ LoadProject model.SolutionPath ]
        | ChangeSelected newSelected ->
            { model with Selected = newSelected}, []
        | TextureDetailMsg (id, textureDetailMsg) ->
            updateTextureDetail id textureDetailMsg model
        | RequestOpenTextureFilePicker ->
            model, [ OpenTextureFilePicker ]
        | RequestAddTexture path -> 
            model, [ AddTexture { TexturePath = path
                                  Store = model.TextureStore
                                  SolutionPath = model.SolutionPath
                                } ]
        | RequestRemoveSelected ->
            match model.Selected with 
            | Some(SelectedId.Texture id) ->
                model, [  RemoveTexture (model.SolutionPath, id, model.TextureStore) ]
            | _ ->
                model, []
        | UpdateStore (store, resetSelected) ->
            { model with Selected = if resetSelected then None else model.Selected
                         TextureStore = store
            }, []
        | NoOp ->
            model, []

    type private SpriteBindingModel = 
        { ParentTexture : Domain.Textures.Texture.InternalStoreId 
          Sprite : Domain.Sprite.T
        }

    let private toSpriteBindingModel (parentTexture : Domain.Textures.Texture.T) 
                                     (sprite : Domain.Sprite.T) =
        { ParentTexture = parentTexture.Id
          Sprite = sprite
        }

    let private toSelectedId (inp: obj) : SelectedId option =
        match inp with 
        | :? SelectedId as id -> Some id
        | _ -> None

    let private toDetailType (selected: SelectedId option) : Presentation.Common.DetailType =
        match selected with 
        | Some(SelectedId.Texture _)  -> 
            Presentation.Common.DetailType.Texture
        | Some(SelectedId.Sprite _) -> 
            Presentation.Common.DetailType.Sprite
        | None -> 
            Presentation.Common.DetailType.None

    let private toTextureDetailModel (selected: SelectedId option) : Domain.Textures.Texture.InternalStoreId option =
        match selected with
        | Some(SelectedId.Texture id)  -> 
            Some id
        | _ -> 
            None

    let private toRenderStrategyType (selected: SelectedId option) (store: Domain.Textures.Texture.Store) : Common.RenderStrategyType =
        match selected with 
        | Some(SelectedId.Texture texId) ->
            let key = Domain.Textures.Texture.toInternalStoreIdString texId
            Common.RenderStrategyType.Texture (Common.TextureRenderStrategyInfo(key))
        | _ -> 
            Common.RenderStrategyType.NoSelection

    let public bindings () = 
        [ "Textures" |> Binding.subModelSeq(
            (fun (m: Model) -> m.TextureStore),
            (fun (_, m) -> m),
            (fun (e: Domain.Textures.Texture.T) -> e.Id),
            (fun (id: Domain.Textures.Texture.InternalStoreId, msg) -> NoOp),
            (fun () -> [ "Name" |> Binding.oneWay (fun m -> match m.Data.Name with | Domain.Textures.Texture.Name n -> n) 
                         "Icon" |> Binding.oneWay (fun _ -> "ImageOutline")
                         "SelectedId" |> Binding.oneWay (fun m -> SelectedId.Texture m.Id)
                         "Children" |> Binding.subModelSeq(
                           (fun (m: Domain.Textures.Texture.T) -> m.Data.Sprites),
                           (fun (p, m) -> toSpriteBindingModel p m),
                           (fun (e: SpriteBindingModel) -> e.Sprite.Id),
                           (fun (id: Domain.Sprite.Id, msg) -> NoOp),
                           (fun () -> [ "Name" |> Binding.oneWay (fun m -> m.Sprite.Data.Name )
                                        "Icon" |> Binding.oneWay (fun _ -> "ScanHelper")
                                        "SelectedId" |> Binding.oneWay (fun m -> SelectedId.Sprite (m.ParentTexture, m.Sprite.Id))
                                        "Children" |> Binding.oneWay (fun _ -> [])
                                      ]))
                       ]))
          "HasTextures" |> Binding.oneWay(fun (m: Model) -> not (m.TextureStore |> List.isEmpty))
          "DetailType" |> Binding.oneWay(fun (m: Model) -> toDetailType m.Selected )
          "TextureDetail" |> Binding.subModelOpt(
            (fun (m: Model) ->  toTextureDetailModel m.Selected),
            (fun (p: Model, m: Domain.Textures.Texture.InternalStoreId) -> Domain.Textures.Texture.getTextureFromStore p.TextureStore m),
            TextureDetailMsg, 
            (fun () -> [ "Name" |> Binding.twoWay(
                            (fun (m: Domain.Textures.Texture.T) -> match m.Data.Name with | Domain.Textures.Texture.Name v -> v ),
                            (fun (v: string) (m: Domain.Textures.Texture.T) -> (m.Id, UpdateName (Domain.Textures.Texture.Name v))))
                         "IdString" |> Binding.twoWay(
                            (fun (m: Domain.Textures.Texture.T) -> m.Data.Id.Str),
                            (fun (v: string) (m: Domain.Textures.Texture.T) -> (m.Id, UpdateId { m.Data.Id with Str = v })))
                         "IdIndex" |> Binding.twoWay(
                            (fun (m: Domain.Textures.Texture.T) -> m.Data.Id.Index),
                            (fun (v: uint) (m: Domain.Textures.Texture.T) -> (m.Id, UpdateId { m.Data.Id with Index = v })))
                         "Dimensions" |> Binding.oneWay(fun (m: Domain.Textures.Texture.T) -> match m.Data.MetaData.Width, m.Data.MetaData.Height with | Domain.Textures.MetaData.Pixel w, Domain.Textures.MetaData.Pixel h -> $"{w} px x {h} px")
                         "DiskSize" |> Binding.oneWay(fun (m: Domain.Textures.Texture.T) -> match m.Data.MetaData.DiskSize with | Domain.Textures.MetaData.Size s -> $"{s} Kb")
                       ]))
          "RenderStrategyType" |> Binding.oneWay(fun (m: Model) -> toRenderStrategyType m.Selected m.TextureStore)

          "OnSelectedItemChangedCommand" |> Binding.cmdParam(fun (selectedId) (m: Model) -> ChangeSelected (toSelectedId selectedId))
          "RequestOpenTextureFilePickerCommand" |> Binding.cmd(fun _ -> RequestOpenTextureFilePicker)
          "RequestRemoveSelectedCommand" |> Binding.cmdIf((fun _ -> RequestRemoveSelected), (fun (m: Model) -> m.Selected.IsSome))
          "OnHasInitialisedCommand" |> Binding.cmd(fun _ -> HasInitialised)
        ]
