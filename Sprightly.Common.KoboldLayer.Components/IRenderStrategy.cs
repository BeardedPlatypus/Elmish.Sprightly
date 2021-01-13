namespace Sprightly.Common.KoboldLayer.Components
{
    /// <summary>
    /// <see cref="IRenderStrategy"/> defines the interface with which
    /// a single render pass can be rendered.
    /// </summary>
    public interface IRenderStrategy
    {
        /// <summary>
        /// Renders a frame.
        /// </summary>
        /// <param name="renderer">The renderer used to render the frame.</param>
        void RenderFrame(IRenderer renderer);
    }
}