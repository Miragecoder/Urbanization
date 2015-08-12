using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Simulation
{
    public class NeverEndingTask
    {
        private readonly string _description;
        private readonly Action _taskAction;
        private readonly CancellationToken _token;
        private Task _task;
        public NeverEndingTask(string description, Action taskAction, CancellationToken token)
        {
            if (taskAction == null) throw new ArgumentNullException(nameof(taskAction));
            _description = description;
            _taskAction = taskAction;
            _token = token;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(async () =>
            {
                var stopWatch = new Stopwatch();
                while (true)
                {
                    stopWatch.Restart();

                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {_description}...");
                    _taskAction();
                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {_description} completed in {stopWatch.Elapsed}.");
                    await Task.Delay(2000, _token);
                }
                // ReSharper disable once FunctionNeverReturns
            }, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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