namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly

module public ProjectPage =
    [<RequireQualifiedAccess>]
    type public SelectedId =
        | Texture of Domain.Textures.Texture.Id
        | Sprite of Domain.Textures.Texture.Id * Domain.Sprite.Id

    type public Model =
        { SolutionPath : Common.Path.T
          TextureStore : Domain.Textures.Texture.Store
          Selected : SelectedId option
        }

    type public Msg = 
        | ChangeSelected of SelectedId option
        | NoOp

    let public init (slnPath: Common.Path.T) : Model = 
        { SolutionPath = slnPath
          TextureStore = 
            [ { Id = Domain.Textures.Texture.Id ("tex", (uint) 1) 
                Data = { Name = Domain.Textures.Texture.Name "Texture 1" 
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
              { Id = Domain.Textures.Texture.Id ("tex", (uint) 2) 
                Data = { Name = Domain.Textures.Texture.Name "Texture 2" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 13.0
                                    }
                         Sprites = []
                       }
              }
              { Id = Domain.Textures.Texture.Id ("tex", (uint) 3) 
                Data = { Name = Domain.Textures.Texture.Name "Texture 3" 
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

    let public update (msg: Msg) (model) : Model * 'CmdMsg list =
        match msg with 
        | ChangeSelected newSelected ->
            { model with Selected = newSelected}, []
        | NoOp ->
            model, []

    type private SpriteBindingModel = 
        { ParentTexture : Domain.Textures.Texture.Id 
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

    let public bindings () = 
        [ "Textures" |> Binding.subModelSeq(
            (fun (m: Model) -> m.TextureStore),
            (fun (_, m) -> m),
            (fun (e: Domain.Textures.Texture.T) -> e.Id),
            (fun (id: Domain.Textures.Texture.Id, msg) -> NoOp),
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
          "OnSelectedItemChanged" |> Binding.cmdParam(fun (selectedId) (m: Model) -> ChangeSelected (toSelectedId selectedId))
        ]
