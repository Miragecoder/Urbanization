using System;

namespace Mirage.Urbanization.WinForms.Rendering
{
    public interface IGraphicsManagerWrapper : IDisposable
    {
        IGraphicsWrapper GetGraphicsWrapper();
        void StartRendering();
    }
}