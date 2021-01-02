namespace Sprightly.Domain.Textures

/// <summary>
/// The <see cref="MetaData"/> module defines all logic related to meta data.
/// </summary>
module MetaData =
    /// <summary>
    /// <see cref="Pixel"/> defines a number of pixels of a texture.
    /// </summary>
    type public Pixel = Pixel of int

    /// <summary>
    /// <see cref="Size"/> defines the size of a texture in kilo bytes (KB)
    /// </summary>
    type public Size = Size of float

    /// <summary>
    /// <see cref="MetaData"/> defines the metadata information of a texture 
    /// file.
    /// </summary>
    type public T =
        { Width: Pixel
          Height: Pixel
          DiskSize: Size
        }


