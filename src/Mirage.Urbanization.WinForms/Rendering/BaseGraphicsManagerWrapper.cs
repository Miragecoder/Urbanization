using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.WinForms.Rendering
{
    public abstract class BaseGraphicsManagerWrapper : IGraphicsManagerWrapper
    {
        private readonly Action _renderAction;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected BaseGraphicsManagerWrapper(Action renderAction)
        {
            _renderAction = renderAction;
        }

        public abstract IGraphicsWrapper GetGraphicsWrapper();

        public abstract void InitializeRenderAction();
        public abstract void EndRenderAction();

        private Task _renderTask;

        public void StartRendering()
        {
            var stopwatch = new Stopwatch();
            int frameCount = 0;
            _renderTask = new Task(() =>
            {
                stopwatch.Start();
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    InitializeRenderAction();
                    _renderAction();
                    EndRenderAction();

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    frameCount++;
                    if (stopwatch.Elapsed.Seconds >= 1)
                    {
                        Console.WriteLine("Amount of FPS: " + frameCount);
                        frameCount = 0;

                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                }
            }, _cancellationTokenSource.Token);
            _renderTask.Start();
        }

        public virtual void Dispose()
        {
            Console.WriteLine("Killing renderer...");
            _cancellationTokenSource.Cancel();
            try
            {
                _renderTask.Wait();
            }
            catch (AggregateException aggEx)
            {
                if (!aggEx.IsCancelled())
                    throw;
            }
            Console.WriteLine("Renderer killed.");
        }
    }
}