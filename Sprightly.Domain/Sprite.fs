namespace Sprightly.Domain

/// <summary>
/// The <see cref="Sprite"/> module defines all types and functions related
/// to defining sprites within the Sprightly application.
///
/// A sprite is defined as a rectangle on a parent texture, defined with a
/// separate name and id.
/// </summary>
module public Sprite =
    /// <summary>
    /// <see cref="Id"/> defines the id of a sprite consisting of a string and
    /// unsigned integer.
    /// </summary>
    type public Id = | Id of string * uint

    /// <summary>
    /// <see cref="Data"/> defines the data of a single sprite.
    /// </summary>
    type public Data = 
        { Name : string
        }

    /// <summary>
    /// <see cref="Sprite.T"/> defines a single sprite entity consisting of an 
    /// <see cref="Sprite.Id"/> and <see cref="Sprite.Data"/>.
    /// </summary>
    type public T =
        { Id : Id 
          Data : Data
        }

