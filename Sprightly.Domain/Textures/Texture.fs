namespace Sprightly.Domain.Textures

open Sprightly.Common
open Sprightly.Domain

/// <summary>
/// <see cref="Texture"/> defines the <see cref="Texture.T"/> and related
/// functions.
/// </summary>
module public Texture = 
    type public InternalStoreId = 
        | Id of uint

    /// <summary>
    /// <see cref="Id"/> defines a texture id.
    /// </summary>
    type public Id = 
        { Str : string 
          Index : uint
        }

    /// <summary>
    /// Convert the specified <paramref name="id"/> to its corresponding key
    /// string value.
    /// </summary>
    /// <param name="id">The <see cref='Id"/> to convert.</param>
    /// <returns>
    /// The corresponding key string.
    /// </returns>
    let public toKeyString (id: Id): string = $"{id.Str}#{id.Index}"

    /// <summary>
    /// <see cref="Name"/> defines a texture name.
    /// </summary>
    type public Name = | Name of string

    /// <summary>
    /// The data of a single texture.
    /// </summary>
    type public Data = {
        Name: Name
        Path: Path.T
        MetaData: MetaData.T
        Sprites: Sprite.T list
        Id: Id
    }

    /// <summary>
    /// <see cref="T"/> defines the texture type.
    /// </summary>
    type public T = {
        Id: InternalStoreId
        Data: Data
    }

    /// <summary>
    /// Construct a new <see cref="T"/>
    /// </summary>
    /// <param name="id">The id of the texture. </param>
    /// <param name="name">The human-readable name of this texture. </param>
    /// <returns>
    /// A new texture.
    /// </returns>
    let construct (internalStoreId: InternalStoreId)
                  (id: Id) 
                  (name: string) 
                  (path: Path.T) 
                  (metaData: MetaData.T) : T =
        { Id = internalStoreId
          Data = { Id = id
                   Name = Name name
                   Path = path
                   MetaData = metaData
                   Sprites = []
                 }
        }

    /// <summary>
    /// <see cref="Store"/> defines a store of textures.
    /// </summary>
    type public Store = T list

    /// <summary>
    /// Create a new empty <see cref="Store"/>.
    /// </summary>
    /// <returns>
    /// A new empty <see cref="Store"/>.
    /// </returns>
    let public emptyStore () : Store = []

    /// <summary>
    /// Add the specified <paramref name="textures"/> to the provided 
    /// <paramref name="store"/> and return the new <see cref="Store"/>.
    /// </summary>
    /// <param name="store">The store to which the textures should be added.</param>
    /// <param name="textures">The textures to add</param>
    /// <returns>
    /// The new <see cref="Store"/> after adding the provided 
    /// <paramref name="textures"/> to the provided <paramref name="store"/>.
    /// </returns>
    let public addTexturesToStore (store: Store) (textures: T list) : Store =
        textures @ store 
        |> List.sortBy (fun (t: T) -> match t.Id with InternalStoreId.Id v -> v)

    /// <summary>
    /// Add the specified <paramref name="texture"/> to the provided 
    /// <paramref name="store"/> and return the new <see cref="Store"/>.
    /// </summary>
    /// <param name="store">The store to which the texture should be added.</param>
    /// <param name="texture">The texture to add</param>
    /// <returns>
    /// The new <see cref="Store"/> after adding the provided 
    /// <paramref name="texture"/> to the provided <paramref name="store"/>.
    /// </returns>
    let public addTextureToStore (store: Store) (texture: T) : Store =
        addTexturesToStore store [ texture ]

    /// <summary>
    /// Get an id unique within the provided <paramref name="store"/> with the
    /// <paramref name="idString"/> as id string.
    /// </summary>
    /// <param name="store">The store in which the unique id should be generated.</param>
    /// <param name="idString">The base id string.</param>
    /// <returns>
    /// An <see cref="Id"/> with the given idString and a unique index.
    /// </returns>
    let public getUniqueId (store: Store) (idString: string) : Id = 
        let usedIndices: uint Set = List.filter (fun (e: T) -> e.Data.Id.Str = idString) store
                                    |> List.map (fun (e: T) -> e.Data.Id.Index)
                                    |> Set.ofList

        let rec generateId (id: uint) =
            if Set.contains id usedIndices then 
                generateId (id + uint 1)
            else 
                id

        { Str = idString; Index = generateId <| uint 0 }

    let public getTextureFromStore (store: Store) (id: InternalStoreId) : T =
        List.find (fun e -> e.Id = id) store

    let public updateTextureInStore (store: Store) (id: InternalStoreId) (fUpdate : T -> T) : Store =
        List.map (fun (e: T) -> if e.Id = id then fUpdate e else e ) store

    let public GetUniqueStoreInternalId (store: Store) : InternalStoreId =
        if store |> List.isEmpty then 
            InternalStoreId.Id ((uint) 0)
        else
            List.map (fun e -> match e.Id with InternalStoreId.Id v -> v) store
            |> (fun l -> (List.max l) + (uint) 1)
            |> InternalStoreId.Id
