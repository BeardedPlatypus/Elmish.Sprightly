namespace Sprightly.Domain.Textures

open Sprightly

/// <summary>
/// <see crfe="Inspector"/> defines the interface of an Inspector.
/// </summary>
type public Inspector =
    /// <summary>
    /// Read the metadata of the file at the specified path.
    /// </summary>
    /// <returns>
    /// Some <see cref="MetaData"/> if the file can be read correctly;
    /// otherwise None.
    /// </returns>
    abstract ReadMetaData : Common.Path.T -> MetaData.T option

