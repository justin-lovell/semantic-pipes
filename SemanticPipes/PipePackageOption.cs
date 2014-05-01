using System;

namespace SemanticPipes
{
    public class PipePackageOption
    {
        private readonly Func<object> _processCallbackFunc;

        public PipePackageOption(Type outputType, Func<object> processCallbackFunc)
        {
            this._processCallbackFunc = processCallbackFunc;
            OutputType = outputType;
        }

        public Type OutputType { get; private set; }

        public Func<object> ProcessInput(object input)
        {
            throw new NotImplementedException();
        }
    }
}