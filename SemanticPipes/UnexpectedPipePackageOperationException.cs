using System;
using System.Runtime.Serialization;

namespace SemanticPipes
{
    [Serializable]
    public class UnexpectedPipePackageOperationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UnexpectedPipePackageOperationException()
        {
        }

        public UnexpectedPipePackageOperationException(string message) : base(message)
        {
        }

        public UnexpectedPipePackageOperationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnexpectedPipePackageOperationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}