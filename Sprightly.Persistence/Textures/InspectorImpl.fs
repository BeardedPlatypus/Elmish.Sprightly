namespace Sprightly.Persistence.Textures

open Sprightly

/// <summary>
/// <see cref="InspectorImpl"/> implements the <see cref="Domain.Textures.Inspector"/> within 
/// the WPF implementation.
/// </summary>
[<Sealed>]
type public InspectorImpl() = 
    interface Domain.Textures.Inspector with 
        member this.ReadMetaData (path: Common.Path.T) : Domain.Textures.MetaData.T option =
            try
                let fileInfo = System.IO.FileInfo(path |> Common.Path.toString)
                use image : SixLabors.ImageSharp.Image = 
                    SixLabors.ImageSharp.Image.Load(fileInfo.FullName)

                Some { Width = Domain.Textures.MetaData.Pixel image.Width
                       Height = Domain.Textures.MetaData.Pixel image.Height
                       DiskSize = Domain.Textures.MetaData.Size (System.Math.Round((((float) fileInfo.Length) / 1024.0), 2))
                     }
            with 
            | _ -> None
