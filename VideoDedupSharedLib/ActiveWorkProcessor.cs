namespace VideoDedupSharedLib
{
    public class ActiveWorkProcessor<T>(
        Action<T, CancellationToken> processFunction)
        : IDisposable
    {
        private readonly Action<T, CancellationToken> processFunc =
            processFunction
            ?? throw new ArgumentNullException(nameof(processFunction));
        private readonly Queue<T> queue = new();
        private readonly object queueLock = new();

        private Task? processingTask;
        private CancellationTokenSource cancelSource = new();
        private volatile bool isProcessing;
        private bool disposedValue;

        public ActiveWorkProcessor(Action<T> processFunction) :
            this((item, token) => processFunction(item))
        { }

        public int Count
        {
            get
            {
                lock (queueLock)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (queueLock)
            {
                queue.Enqueue(item);

                if (isProcessing)
                {
                    return;
                }

                isProcessing = true;
                processingTask?.Wait();
                processingTask?.Dispose();
                processingTask = Task.Run(ProcessQueue);
            }
        }

        private void ProcessQueue()
        {
            while (true)
            {
                if (cancelSource.Token.IsCancellationRequested)
                {
                    return;
                }

                T? item;

                lock (queueLock)
                {
                    isProcessing = queue.TryDequeue(out item);
                    if (!isProcessing)
                    {
                        return;
                    }
                }

                processFunc(item!, cancelSource.Token);
            }
        }

        public void Clear()
        {
            cancelSource.Cancel();
            processingTask?.Wait();
            cancelSource.Dispose();

            lock (queueLock)
            {
                cancelSource = new CancellationTokenSource();
                processingTask?.Dispose();
                queue.Clear();
                isProcessing = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancelSource.Cancel();
                    processingTask?.Wait();
                    cancelSource.Dispose();
                    processingTask?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
