namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly

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
        | ChangeSelected of SelectedId option
        | TextureDetailMsg of id: Domain.Textures.Texture.InternalStoreId * msg: TextureDetailMsg
        | NoOp

    let public init (slnPath: Common.Path.T) : Model = 
        { SolutionPath = slnPath
          TextureStore = 
            [ { Id = Domain.Textures.Texture.InternalStoreId.Id ((uint) 1)
                Data = { Id = { Str = "tex"; Index = (uint) 1 }
                         Name = Domain.Textures.Texture.Name "Texture 1" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 12.0
                                    }
                         Sprites = [ { Id = Domain.Sprite.Id ("sprite", (uint) 0 )
                                       Data = { Name = "Sprite 0" }
                                     } 
                                     { Id = Domain.Sprite.Id ("sprite", (uint) 1)
                                       Data = { Name = "Sprite 1" }
                                     }
                                     { Id = Domain.Sprite.Id ("sprite", (uint) 1)
                                       Data = { Name = "Sprite 1" }
                                     }
                                   ]
                       }
              }
              { Id = Domain.Textures.Texture.InternalStoreId.Id ((uint) 2)
                Data = { Id = { Str = "tex"; Index = (uint) 2 }
                         Name = Domain.Textures.Texture.Name "Texture 2" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 13.0
                                    }
                         Sprites = []
                       }
              }
              { Id = Domain.Textures.Texture.InternalStoreId.Id ((uint) 3)
                Data = { Id = { Str = "tex"; Index = (uint) 3 }
                         Name = Domain.Textures.Texture.Name "Texture 3" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 14.0
                                    }
                         Sprites = []
                       }
              }
            ]
          Selected = None
        }

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

    let public update (msg: Msg) (model: Model) : Model * 'CmdMsg list =
        match msg with 
        | ChangeSelected newSelected ->
            { model with Selected = newSelected}, []
        | TextureDetailMsg (id, textureDetailMsg) ->
            updateTextureDetail id textureDetailMsg model
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

          "OnSelectedItemChanged" |> Binding.cmdParam(fun (selectedId) (m: Model) -> ChangeSelected (toSelectedId selectedId))
        ]
