using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Grafika.Handler
{
    public class CommandQueueHandler
    {
        private Queue<ICommand> _commandQueue = new Queue<ICommand>();
        private bool _isProcessing = false;
        private object _lockObject = new object();
        private Thread _processingThread;

        public void EnqueueCommand(ICommand command)
        {
            lock (_lockObject)
            {
                _commandQueue.Enqueue(command);

                if (!_isProcessing)
                {
                    _processingThread = new Thread(ProcessCommands);
                    _processingThread.Start();
                }
            }
        }

        private void ProcessCommands()
        {
            while (true)
            {
                ICommand nextCommand = null;

                lock (_lockObject)
                {
                    if (_commandQueue.Count > 0)
                    {
                        nextCommand = _commandQueue.Dequeue();
                        _isProcessing = true;
                    }
                    else
                    {
                        _isProcessing = false;
                        break;
                    }
                }

                if (nextCommand != null && nextCommand.CanExecute(null))
                {
                    nextCommand.Execute(null);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}
