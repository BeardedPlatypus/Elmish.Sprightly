namespace Sprightly.Application

open Sprightly.Common
open Sprightly.Domain

/// <summary>
/// The <see cref="Application.Texture"/> module contains all business
/// logic related to interacting with textures.
/// </summary>
module public Texture =
    /// <summary>
    /// <see cref="TextureDescription"/> describes all information
    /// related to loading a texture.
    /// </summary>
    type public TextureDescription = 
        { Name : string 
          Id : Textures.Texture.Id
          Path : Path.T
        }

    /// <summary>
    /// The function to retrieve the <see cref="MetaData.T"/> of a texture from a given
    /// <see cref="Path.T"/>.
    /// <summary>
    type public RetrieveTextureMetaDataFunc = Path.T -> Textures.MetaData.T option

    /// <summary>
    /// The function to load the specified texture into memory.
    /// </summary>
    type public LoadTextureFunc = Textures.Texture.T -> unit

    /// <summary>
    /// The function to copy the texture defined in the <see cref="TextureCopyData"/>
    /// and return the new location.
    /// </summary>
    type public CopyTextureIntoSolutionFunc = Path.T -> Path.T option

    /// <summary>
    /// The function to save the store to disk.
    /// </summary>
    type public SaveStoreFunc = Textures.Texture.Store -> unit

    /// <summary>
    /// Adds the texture at the specified <paramref name="texturePath"/> to the
    /// specified the provided <paramref name="store"/> and return the id of 
    /// the new texture and the new store.
    /// </summary>
    /// <param name="fCopyTextureIntoSolution">
    /// The function to copy the provided texture path into the solution, and 
    /// return the new texture path.
    /// </param>
    /// <param name="fRetrieveMetaData>
    /// The function to retrieve the <see cref="Textures.MetaData.T"/> of a
    /// texture.
    /// </param>
    /// <param name="fLoadTexture">
    /// The function to load the <see cref="Textures.Texture.T"/> into memory.
    /// </param>
    /// <param name="texturePath">
    /// The path outside of the sln directory from which the file should be copied.
    /// </param>
    /// <param name="store">
    /// The current texture store of this Sprightly application.
    /// </param>
    /// <returns>
    /// The id of the newly created texture and the new store.
    /// <returns>
    let public addNewTextureToStore (fCopyTextureIntoSolution: CopyTextureIntoSolutionFunc)
                                    (fRetrieveMetaData: RetrieveTextureMetaDataFunc)
                                    (fLoadTexture: LoadTextureFunc)
                                    (fSaveStore: SaveStoreFunc)
                                    (texturePath: Path.T)
                                    (store: Textures.Texture.Store) : 
                                    (Textures.Texture.InternalStoreId * Textures.Texture.Store) option =
        let slnTexturePath = fCopyTextureIntoSolution texturePath
        if slnTexturePath.IsSome then
            let metaData = fRetrieveMetaData slnTexturePath.Value
            
            if metaData.IsSome then 
                let name = Path.nameWithoutExtension slnTexturePath.Value
                let id = Textures.Texture.getUniqueId store name
                let storeId = Textures.Texture.GetUniqueStoreInternalId store
                let newTexture = 
                    Textures.Texture.construct storeId id name slnTexturePath.Value metaData.Value
            
                fLoadTexture newTexture
            
                let newStore = Textures.Texture.addTextureToStore store newTexture

                fSaveStore newStore
                Some (storeId, newStore)
            else 
                None
        else 
            None 

    /// <summary>
    /// The function to remove a texture from the solution on the disk.
    /// </summary>
    type public RemoveTextureFromSolutionFunc = Path.T -> unit

    /// <summary>
    /// The function to unload the texture from the underlying render memory.
    /// </summary>
    type public UnloadTextureFunc = Textures.Texture.T -> unit

    /// <summary>
    /// Removes the texture associated with the <paramref name="id"/> from the
    /// <paramref name="store"/> and return the new store.
    /// </summary>
    /// <param name="fRemoveTextureFromSolution">
    /// The function to remove the texture from the solution on disk.
    /// </param>
    /// <param name="fUnloadTexture">
    /// The function to unload the texture from memory.
    /// </param>
    /// <param name="store">
    /// The current texture store of this Sprightly application.
    /// </param>
    /// <param name="id">
    /// The id of the texture to remove from the <paramref name="store"/>.
    /// <param>
    /// <returns>
    /// The new store without the texture associated with <paramref name="id"/>.
    /// </returns>
    let public removeTextureFromStore (fRemoveTextureFromSolution: RemoveTextureFromSolutionFunc)
                                      (fUnloadTexture: UnloadTextureFunc)
                                      (fSaveStore: SaveStoreFunc)
                                      (id: Textures.Texture.InternalStoreId)
                                      (store: Textures.Texture.Store) : Textures.Texture.Store =
        let tex: Textures.Texture.T = Textures.Texture.getTextureFromStore store id
        
        fRemoveTextureFromSolution tex.Data.Path
        fUnloadTexture tex

        let newStore = Textures.Texture.removeTextureFromStore store id
        fSaveStore newStore 

        newStore
