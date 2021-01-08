namespace Sprightly.Presentation.Pages

open Elmish
open Elmish.WPF

open Sprightly

module public ProjectPage =
    type public Selected =
        | Texture of Domain.Textures.Texture.Id

    type public Model =
        { SolutionPath : Common.Path.T
          TextureStore : Domain.Textures.Texture.Store
          Selected : Selected
        }

    type public Msg = 
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
                       }
              }
              { Id = Domain.Textures.Texture.Id ("tex", (uint) 2) 
                Data = { Name = Domain.Textures.Texture.Name "Texture 2" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 13.0
                                    }
                       }
              }
              { Id = Domain.Textures.Texture.Id ("tex", (uint) 3) 
                Data = { Name = Domain.Textures.Texture.Name "Texture 3" 
                         Path = Common.Path.T "empty" 
                         MetaData = { Width = Domain.Textures.MetaData.Pixel 256
                                      Height = Domain.Textures.MetaData.Pixel 512
                                      DiskSize = Domain.Textures.MetaData.Size 14.0
                                    }
                       }
              }
            ]
          Selected = Texture (Domain.Textures.Texture.Id ("tex", (uint) 1))
        }

    let public update (_: Msg) (model) : Model * 'CmdMsg list =
        model, []

    let public bindings () = 
        [ "Textures" |> Binding.subModelSeq(
            (fun (m: Model) -> m.TextureStore),
            (fun (_, m) -> m),
            (fun (e: Domain.Textures.Texture.T) -> e.Id),
            (fun (id: Domain.Textures.Texture.Id, msg) -> NoOp),
            (fun () -> [ "Name" |> Binding.oneWay (fun m -> match m.Data.Name with | Domain.Textures.Texture.Name n -> n) ]))
        ]
