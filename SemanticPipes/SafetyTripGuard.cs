using System;

namespace SemanticPipes
{
    internal sealed class SafetyTripGuard
    {
        private Exception _killingException;

        public void DoAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            GuardAgainstKilledSwitch();
            ExecuteAction(action);
        }

        private void ExecuteAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _killingException = ex;
                throw;
            }
        }

        private void GuardAgainstKilledSwitch()
        {
            if (_killingException == null)
            {
                return;
            }

            const string msg = "The operation cannot be performed since the safety alarm was " +
                               "triggered by an excpetion previous encountered." +
                               " Please see the inner exception for more details.";
            throw new InvalidOperationException(msg, _killingException);
        }
    }
}