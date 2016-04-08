using System;
using System.Runtime.Serialization;

namespace PetInfoGrabber
{
    [Serializable]
    internal class BreedNotFoundException : Exception
    {
        public BreedNotFoundException()
        {
        }

        public BreedNotFoundException(string message) : base(message)
        {
        }

        public BreedNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BreedNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}