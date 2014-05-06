using System;
using System.Runtime.Serialization;

namespace SemanticPipes
{
    [Serializable]
    public class CannotResolveSemanticException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CannotResolveSemanticException()
        {
        }

        public CannotResolveSemanticException(string message) : base(message)
        {
        }

        public CannotResolveSemanticException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CannotResolveSemanticException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}