using Sprightly.Common.KoboldLayer.Components;

namespace Sprightly.Presentation.Views.Pages.ProjectPage.RenderStrategies
{
    /// <summary>
    /// <see cref="NothingSelectedRenderStrategy"/> implements the <see cref="IRenderStrategy"/>
    /// for the state in which no texture or sprite is selected.
    /// </summary>
    /// <seealso cref="IRenderStrategy" />
    public sealed class NothingSelectedRenderStrategy : IRenderStrategy
    {
        public void RenderFrame(IRenderer renderer)
        {
            // This is currently loaded in the ViewportHost and should be moved to a better location.
            renderer.RenderTexture("unloaded");
        }
    }
}