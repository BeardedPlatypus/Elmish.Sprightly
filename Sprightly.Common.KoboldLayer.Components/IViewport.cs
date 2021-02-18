using System;
using kobold_layer.clr;

namespace Sprightly.Common.KoboldLayer.Components
{
    /// <summary>
    /// <see cref="IViewport"/> defines the interface with which components
    /// can interact with the viewport.
    /// </summary>
    public interface IViewport : IRenderer
    {
        /// <summary>
        /// Initialises the specified p window.
        /// </summary>
        /// <param name="pWindow">The p window.</param>
        void Initialise(IntPtr pWindow);

        // TODO: refactor this
        void Update();

        /// <summary>
        /// Begins the render.
        /// </summary>
        void BeginRender();

        /// <summary>
        /// Finalises the render.
        /// </summary>
        void FinaliseRender();

        /// <summary>
        /// Determines whether a texture exists for the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the texture.</param>
        /// <returns>
        ///   <c>true</c> if a texture exists with the corresponding <paramref name="id"/>; otherwise, <c>false</c>.
        /// </returns>
        bool HasTexture(string id);

        /// <summary>
        /// Loads the texture at the specified <paramref name="path"/> and
        /// assign it to the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="path">The path.</param>
        void LoadTexture(string id, string path);

        /// <summary>
        /// Unloads the texture associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void UnloadTexture(string id);

        /// <summary>
        /// Get the world area as a <see cref="rectangle"/>.
        /// </summary>
        /// <returns>
        /// The world area as a <see cref="rectangle"/>.
        /// </returns>
        rectangle get_world_area();
        
        /// <summary>
        /// Set the world area to <paramref name="worldArea"/>.
        /// </summary>
        /// <param name="worldArea">The new world area.</param>
        void set_world_area(rectangle worldArea);

        /// <summary>
        /// Get the viewport area as a <see cref="rectangle"/>.
        /// </summary>
        /// <returns>
        /// The viewport area as a <see cref="rectangle"/>.
        /// </returns>
        rectangle get_viewport_area();

        /// <summary>
        /// Set the viewport area to <paramref name="viewportArea"/>.
        /// </summary>
        /// <param name="viewportArea">The new viewport area.</param>
        void set_viewport_area(rectangle viewportArea);
    }
}