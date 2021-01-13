namespace Sprightly.Common.KoboldLayer.Components
{
    /// <summary>
    /// <see cref="IRenderer"/> defines the methods to render different components
    /// within the corresponding viewport.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Renders the texture with the specified <paramref name="textureLabel"/>.
        /// </summary>
        /// <param name="textureLabel">The texture label.</param>
        void RenderTexture(string textureLabel);
    }
}