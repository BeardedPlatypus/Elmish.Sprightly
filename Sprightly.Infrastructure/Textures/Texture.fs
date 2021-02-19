namespace Sprightly.Infrastructure.Textures

open Sprightly

/// <summary>
/// The <see cref="Infrastructure.Textures.texture"/> module contains all
/// logic related to loading and unloading of textures within the Sprightly
/// Application.
/// </summary>
module Texture =
    /// <summary>
    /// Check whether a texture with the specified <paramref name="id"/> exists
    /// within the current Sprightly application.
    /// </summary>
    /// <param name="id">The id to verify.</param>
    /// <returns>
    /// True if a texture with the specified <paramref name="id"/> exists within
    /// the current Sprightly Application.
    /// </returns>
    let hasTexture (id: Domain.Textures.Texture.InternalStoreId) : bool =
        let vp = Common.KoboldLayer.ViewportFactory.Create ()
        let idVal = match id with | Domain.Textures.Texture.InternalStoreId.Id v -> v

        vp.HasTexture (idVal.ToString())

    /// <summary>
    /// Load the texture at the specified <paramref name="path"/> into this
    /// Sprightly Application, and associate it with the specified 
    /// <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id that will be associated with the new texture.</param>
    /// <param name="path">The path to the texture file to be loaded.</param>
    let loadTexture (id: Domain.Textures.Texture.InternalStoreId) (path: Common.Path.T) : unit =
        let vp = Common.KoboldLayer.ViewportFactory.Create ()
        let idVal = Domain.Textures.Texture.toInternalStoreIdString id

        vp.LoadTexture(idVal.ToString(), path |> Common.Path.toString)

    /// <summary>
    /// Unload the texture associated with the specified <paramref name="id"/>
    /// from this Sprightly Application.
    /// </summary>
    /// <param name="id">The id of the texture that will be removed.</param>
    let unloadTexture (id: Domain.Textures.Texture.InternalStoreId) : unit = 
        let vp = Common.KoboldLayer.ViewportFactory.Create ()
        let idVal = Domain.Textures.Texture.toInternalStoreIdString id

        vp.UnloadTexture(idVal.ToString())
