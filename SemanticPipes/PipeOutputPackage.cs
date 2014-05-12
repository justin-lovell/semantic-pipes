using System;

namespace SemanticPipes
{
    public class PipeOutputPackage
    {
        private readonly Func<object, object> _processCallbackFunc;

        public PipeOutputPackage(Type inputType, Type outputType, Func<object, object> processCallbackFunc)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");
            if (outputType == null) throw new ArgumentNullException("outputType");
            if (processCallbackFunc == null) throw new ArgumentNullException("processCallbackFunc");

            _processCallbackFunc = processCallbackFunc;
            InputType = inputType;
            OutputType = outputType;
        }

        public Type OutputType { get; private set; }
        public Type InputType { get; private set; }

        public object ProcessInput(object input)
        {
            if (input == null) throw new ArgumentNullException("input");

            GuardAgainstUnexpectedInputType(input);

            object output = _processCallbackFunc(input);

            GuardAgainstNullResultFromCallback(output);
            GuardAgainstUnexpectedReturnTypeFromCallback(output);

            return output;
        }

        private void GuardAgainstUnexpectedInputType(object input)
        {
            Type inputType = input.GetType();
            if (inputType == InputType) return;

            string message =
                string.Format(
                    "Expected to only process objects of type '{0}'. Instead, we got an object of type '{1}'.",
                    InputType, inputType);
            throw new ArgumentException(message, "input");
        }

        private void GuardAgainstUnexpectedReturnTypeFromCallback(object output)
        {
            Type outputType = output.GetType();
            if (OutputType.IsAssignableFrom(outputType)) return;

            string message =
                string.Format("Expected an output of type '{0}'. Instead, we got an object of type of '{1}'.",
                    OutputType, outputType);
            throw new UnexpectedPipePackageOperationException(message);
        }

        private static void GuardAgainstNullResultFromCallback(object output)
        {
            if (output != null) return;

            const string message = "The output of the process callback returned null. We expected a non-null object.";
            throw new UnexpectedPipePackageOperationException(message);
        }
    }
}