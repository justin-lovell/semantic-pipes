using System;
using System.Threading.Tasks;

namespace SemanticPipes
{
    public delegate Task<object> PipeCallback(object input, ISemanticBroker broker);

    public class PipeOutputPackage
    {
        private const int IncrementChainedWeight = 256;
        private readonly PipeCallback _processCallbackFunc;

        private PipeOutputPackage(int weight, Type inputType, Type outputType, PipeCallback processCallbackFunc)
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

        public async Task<object> ProcessInput(object input, ISemanticBroker semanticBroker)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            GuardAgainstUnexpectedInputType(input);

            var processingTask = _processCallbackFunc(input, semanticBroker);
            GuardAgainstNullResultFromCallback(processingTask);

            object output = await processingTask.ConfigureAwait(false);

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

            PipeCallback processCallbackFunc = async (input, broker) =>
            {
                object intermediate = await startPackage.ProcessInput(input, broker).ConfigureAwait(false);
                return await endPackage.ProcessInput(intermediate, broker).ConfigureAwait(false);
            };

            return new PipeOutputPackage(weight, sourceType, destinationType, processCallbackFunc);
        }

        public static PipeOutputPackage Infer(PipeOutputPackage basedOffPackage, Type inputType, Type outputType,
            PipeCallback processCallbackFunc)
        {
            int weight = basedOffPackage.NextChainingWeight();
            return new PipeOutputPackage(weight, inputType, outputType, processCallbackFunc);
        }

        public static PipeOutputPackage Direct(Type inputType, Type outputType, PipeCallback processCallbackFunc)
        {
            const int weight = 1;
            return new PipeOutputPackage(weight, inputType, outputType, processCallbackFunc);
        }
    }
}