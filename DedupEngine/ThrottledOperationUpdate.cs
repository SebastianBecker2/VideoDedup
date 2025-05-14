namespace DedupEngine
{
    using static VideoDedupGrpc.OperationInfo.Types;

    internal class ThrottledOperationUpdate(
        Action<OperationType, int, int> eventCaller)
        : ThrottledEvent
    {
        public void Raise(OperationType type, int counter, int maxCount)
        {
            if (Execute())
            {
                eventCaller(type, counter, maxCount);
            }
        }
    }
}
