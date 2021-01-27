namespace Sprightly.Domain

/// <summary>
/// <see cref="Rectangle"/> defines the types and functions related to
/// managing rectangles used within the rendering logic of Sprightly.
/// </summary>
module Rectangle =
    /// <summary>
    /// <see cref="Rectangle.T"/> defines a single rectangle consisting
    /// of a width and height and a lower left corner defined by (x, y)
    /// </summary>
    [<RequireQualifiedAccess>]
    type public T =
        { x: uint 
          y: uint
          width: uint
          height: uint
        }
