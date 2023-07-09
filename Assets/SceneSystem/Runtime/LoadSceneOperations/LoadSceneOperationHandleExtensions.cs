using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace AnnulusGames.SceneSystem
{
    public static class LoadSceneOperationHandleExtensions
    {
        public static IEnumerator ToYieldInteraction(this LoadSceneOperationHandle self)
        {
            while (!self.IsDone)
            {
                yield return null;
            }
        }

        public static async Task ToTask(this LoadSceneOperationHandle self, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (self.IsDone)
            {
                await Task.CompletedTask;
                return;
            }

            while (!self.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
    }
}