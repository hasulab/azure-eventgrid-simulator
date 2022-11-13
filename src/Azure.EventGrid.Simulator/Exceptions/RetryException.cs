using System.Runtime.Serialization;

namespace Azure.EventGrid.Simulator.Exceptions
{
    [Serializable]
    public class RetryException : Exception
    {
        public RetryException()
        {
        }

        public RetryException(string message) : base(message)
        {
        }

        public RetryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RetryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
