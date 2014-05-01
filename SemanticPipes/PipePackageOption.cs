using System;

namespace SemanticPipes
{
    public class PipePackageOption
    {
        private readonly Func<object, object> _processCallbackFunc;

        public PipePackageOption(Type outputType, Func<object, object> processCallbackFunc)
        {
            if (outputType == null) throw new ArgumentNullException("outputType");
            if (processCallbackFunc == null) throw new ArgumentNullException("processCallbackFunc");

            _processCallbackFunc = processCallbackFunc;
            OutputType = outputType;
        }

        public Type OutputType { get; private set; }

        public object ProcessInput(object input)
        {
            if (input == null) throw new ArgumentNullException("input");

            object output = _processCallbackFunc(input);

            GuardAgainstNullResultFromCallback(output);
            GuardAgainstUnexpectedReturnTypeFromCallback(output);

            return output;
        }

        private void GuardAgainstUnexpectedReturnTypeFromCallback(object output)
        {
            Type outputType = output.GetType();
            if (outputType == OutputType) return;

            string message = string.Format("Callback returned '{0}'. We were expecting an object of type '{1}'.",
                outputType, OutputType);
            throw new NotSupportedException(message);
        }

        private static void GuardAgainstNullResultFromCallback(object output)
        {
            if (output != null) return;

            const string message = "The output of the process callback returned null. We expected a non-null object.";
            throw new NotSupportedException(message);
        }
    }
}