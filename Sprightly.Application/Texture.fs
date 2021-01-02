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
    /// The function to retrieve all information of a texture from a given
    /// <see cref="TextureDescription"/>.
    /// <summary>
    type public RetrieveTextureDataFunc = TextureDescription -> Textures.Texture.T option

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
                                    (Textures.Texture.Id * Textures.Texture.Store) option =
        let slnTexturePath = fCopyTextureIntoSolution texturePath
        if slnTexturePath.IsSome then
            let metaData = fRetrieveMetaData slnTexturePath.Value
            
            if metaData.IsSome then 
                let name = Path.nameWithoutExtension slnTexturePath.Value
                let id = Textures.Texture.getUniqueId store name
                let newTexture = 
                    Textures.Texture.construct id name slnTexturePath.Value metaData.Value
            
                fLoadTexture newTexture
            
                let newStore = Textures.Texture.addTextureToStore store newTexture

                fSaveStore newStore
                Some (id, newStore)
            else 
                None
        else 
            None 
