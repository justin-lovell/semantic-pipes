using System.Threading.Tasks;

namespace SemanticPipes
{
    internal static class TaskEx
    {
        public static Task<TResult> IntoTaskResult<TResult>(this TResult result)
        {
            var taskSource = new TaskCompletionSource<TResult>();
            taskSource.SetResult(result);
            return taskSource.Task;
        }
    }
}