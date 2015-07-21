using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Simulation
{
    public class NeverEndingTask
    {
        private readonly CancellationToken _token;
        private readonly Task _task;
        public NeverEndingTask(string description, Action taskAction, CancellationToken token)
        {
            if (taskAction == null) throw new ArgumentNullException(nameof(taskAction));
            _token = token;
            _task = CreateTask(description.ToLower(), taskAction, token);
        }

        private static Task CreateTask(string description, Action taskAction, CancellationToken token)
        {
            return new Task(() =>
            {
                var stopWatch = new Stopwatch();
                while (true)
                {
                    stopWatch.Restart();

                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {description}...");
                    taskAction();
                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {description} completed in {stopWatch.Elapsed}.");
                    Thread.Sleep(2000);
                }
                // ReSharper disable once FunctionNeverReturns
            }, token);
        }

        public void Start()
        {
            _task.Start();
        }

        public void Wait()
        {
            try
            {
                _task.Wait(_token);
            }
            catch (OperationCanceledException ex)
            {
                Mirage.Urbanization.Logger.Instance.WriteLine(ex);
            }
        }
    }
}