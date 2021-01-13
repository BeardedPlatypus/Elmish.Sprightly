using Sprightly.Common.KoboldLayer.Components;

namespace Sprightly.Presentation.Views.Pages.ProjectPage.RenderStrategies
{
    public class TextureRenderStrategy : IRenderStrategy
    {
        private readonly string textureId;

        public TextureRenderStrategy(string textureId)
        {
            this.textureId = textureId;
        }

        public void RenderFrame(IRenderer renderer)
        {
            renderer.RenderTexture(textureId);
        }
    }
}