using System;

namespace SemanticPipes
{
    public class PipeOutputPackage
    {
        private const int IncrementChainedWeight = 256;
        private readonly Func<object, ISemanticBroker, object> _processCallbackFunc;

        private PipeOutputPackage(int weight, Type inputType, Type outputType, Func<object, ISemanticBroker, object> processCallbackFunc)
        {
            if (inputType == null)
            {
                throw new ArgumentNullException("inputType");
            }
            if (outputType == null)
            {
                throw new ArgumentNullException("outputType");
            }
            if (processCallbackFunc == null)
            {
                throw new ArgumentNullException("processCallbackFunc");
            }

            _processCallbackFunc = processCallbackFunc;
            InputType = inputType;
            OutputType = outputType;
            Weight = weight;
        }

        public Type OutputType { get; private set; }
        public Type InputType { get; private set; }
        public int Weight { get; private set; }

        public object ProcessInput(object input, ISemanticBroker semanticBroker)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            GuardAgainstUnexpectedInputType(input);

            object output = _processCallbackFunc(input, semanticBroker);

            GuardAgainstNullResultFromCallback(output);
            GuardAgainstUnexpectedReturnTypeFromCallback(output);

            return output;
        }

        private void GuardAgainstUnexpectedInputType(object input)
        {
            Type actualInputType = input.GetType();
            if (InputType.IsAssignableFrom(actualInputType))
            {
                return;
            }

            string message =
                string.Format(
                    "Expected to only process objects of type '{0}'. Instead, we got an object of type '{1}'.",
                    InputType, actualInputType);
            throw new ArgumentException(message, "input");
        }

        private void GuardAgainstUnexpectedReturnTypeFromCallback(object output)
        {
            Type actualOutputType = output.GetType();
            if (OutputType.IsAssignableFrom(actualOutputType))
            {
                return;
            }

            string message =
                string.Format("Expected an output of type '{0}'. Instead, we got an object of type of '{1}'.",
                    OutputType, actualOutputType);
            throw new UnexpectedPipePackageOperationException(message);
        }

        private static void GuardAgainstNullResultFromCallback(object output)
        {
            if (output != null)
            {
                return;
            }

            const string message = "The output of the process callback returned null. We expected a non-null object.";
            throw new UnexpectedPipePackageOperationException(message);
        }

        private int NextChainingWeight()
        {
            return Weight + IncrementChainedWeight;
        }

        public bool IsFromUserRegistration()
        {
            return Weight < IncrementChainedWeight;
        }

        public static PipeOutputPackage Bridge(PipeOutputPackage startPackage, PipeOutputPackage endPackage)
        {
            if (startPackage.OutputType != endPackage.InputType)
            {
                throw new NotSupportedException();
            }

            Type sourceType = startPackage.InputType;
            Type destinationType = endPackage.OutputType;
            int weight = startPackage.Weight + endPackage.Weight;

            Func<object, ISemanticBroker, object> processCallbackFunc = (input, broker) =>
            {
                object intermediate = startPackage.ProcessInput(input, broker);
                return endPackage.ProcessInput(intermediate, broker);
            };

            return new PipeOutputPackage(weight, sourceType, destinationType, processCallbackFunc);
        }

        public static PipeOutputPackage Infer(PipeOutputPackage basedOffPackage, Type inputType, Type outputType,
            Func<object, ISemanticBroker, object> processCallbackFunc)
        {
            int weight = basedOffPackage.NextChainingWeight();
            return new PipeOutputPackage(weight, inputType, outputType, processCallbackFunc);
        }

        public static PipeOutputPackage Direct(Type inputType, Type outputType, Func<object, ISemanticBroker, object> processCallbackFunc)
        {
            const int weight = 1;
            return new PipeOutputPackage(weight, inputType, outputType, processCallbackFunc);
        }
    }
}