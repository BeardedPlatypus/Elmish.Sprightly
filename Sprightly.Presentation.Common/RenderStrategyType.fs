namespace Sprightly.Presentation.Common

type public TextureRenderStrategyInfo(textureId: string) =
    member public this.TextureId = textureId

[<RequireQualifiedAccess>]
type public RenderStrategyType =
    | NoSelection
    | Texture of TextureRenderStrategyInfo

