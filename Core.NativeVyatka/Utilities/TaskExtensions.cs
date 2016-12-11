using System.Threading.Tasks;

namespace NativeVyatkaCore.Utilities
{
    public static class TaskExtensions
    {
        public static readonly Task CompletedTask = Task.FromResult(false);

        public static Task<bool> CompletedResultTask(bool result)
        {
            var completionSource = new TaskCompletionSource<bool>();
            completionSource.SetResult(result);
            return completionSource.Task;
        }
    }
}
