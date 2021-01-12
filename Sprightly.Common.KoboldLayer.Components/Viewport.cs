using System;
using kobold_layer.clr;

namespace Sprightly.Common.KoboldLayer.Components
{
    /// <summary>
    /// <see cref="Viewport"/> implements the interface defined by the
    /// <see cref="IViewport"/> utilising kobold.layer's view.
    /// </summary>
    /// <seealso cref="IViewport" />
    public class Viewport : IViewport
    {
        private readonly view _view = new view();

        public void Initialise(IntPtr pWindow)
        {
            unsafe
            {
                _view.initialise(pWindow.ToPointer());
            }
        }

        public void Update()
        {
            _view.update();
        }

        public void BeginRender()
        {
            _view.initialise_frame();
        }

        public void RenderTexture(string textureLabel)
        {
            _view.render_texture(textureLabel);
        }

        public void FinaliseRender()
        {
            _view.finalise_frame();
        }

        public bool HasTexture(string id)
        {
            return _view.has_texture(id);
        }

        public void LoadTexture(string id, string path)
        {
            _view.load_texture(id, path);
        }
    }
}