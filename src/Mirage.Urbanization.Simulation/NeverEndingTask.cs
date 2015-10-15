using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Simulation
{
    public class NeverEndingTask
    {
        private readonly string _description;
        private readonly Func<Task> _taskAction;
        private readonly CancellationToken _token;
        private readonly int _pause;
        private Task _task;
        public NeverEndingTask(string description, Func<Task> taskAction, CancellationToken token, int pause = 2000)
        {
            if (taskAction == null) throw new ArgumentNullException(nameof(taskAction));
            _description = description;
            _taskAction = taskAction;
            _token = token;
            _pause = pause;
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
                    await _taskAction();
                    Mirage.Urbanization.Logger.Instance.WriteLine($"Executing {_description} completed in {stopWatch.Elapsed}.");
                    await Task.Delay(_pause, _token);
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