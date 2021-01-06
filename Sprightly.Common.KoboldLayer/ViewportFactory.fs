namespace Sprightly.Common.KoboldLayer

open Sprightly.Common.KoboldLayer.Components

/// <summary>
/// <see cref="ViewportFactory"/> module provides the methods to construct a
/// <see cref="IViewport"/>.
/// </summary>
module public ViewportFactory = 
    // Currently this is a singleton that creates a single Viewport.
    let private viewport : IViewport =
        Viewport() :> IViewport

    /// <summary>
    /// Create a reference to the singleton <see cref="IViewport"/>.
    /// </summary>
    let public Create () : IViewport = viewport

