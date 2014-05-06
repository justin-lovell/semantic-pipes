using System;
using System.Runtime.Serialization;

namespace SemanticPipes
{
    [System.Serializable]
    public class InvalidRegistryConfigurationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidRegistryConfigurationException()
        {
        }

        public InvalidRegistryConfigurationException(string message) : base(message)
        {
        }

        public InvalidRegistryConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidRegistryConfigurationException(
            System.Runtime.Serialization.SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}