namespace Sprightly.Infrastructure.Textures

open Sprightly

module Texture =
    let hasTexture (id: Domain.Textures.Texture.InternalStoreId) : bool =
        let vp = Common.KoboldLayer.ViewportFactory.Create ()
        let idVal = match id with | Domain.Textures.Texture.InternalStoreId.Id v -> v

        vp.HasTexture (idVal.ToString())

    let loadTexture (id: Domain.Textures.Texture.InternalStoreId) (path: Common.Path.T) : unit =
        let vp = Common.KoboldLayer.ViewportFactory.Create ()
        let idVal = Domain.Textures.Texture.toInternalStoreIdString id

        vp.LoadTexture(idVal.ToString(), path |> Common.Path.toString)


