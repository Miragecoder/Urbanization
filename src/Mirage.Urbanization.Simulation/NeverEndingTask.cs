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
                while (true)
                {
                    var stopWatch = new Stopwatch();

                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {description}...");
                    taskAction();
                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {description} completed in {stopWatch.Elapsed}.");
                    Task.Delay(2000, token).Wait(token);
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