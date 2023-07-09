#if SCENESYSTEM_SUPPORT_UNITASK
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AnnulusGames.SceneSystem
{
    public static class UniTaskLoadSceneOperationHandleExtensions
    {
        public static async UniTask ToUniTask(this LoadSceneOperationHandle self, CancellationToken cancellationToken = default)
        {
            while (!self.IsDone)
            {
                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
#endif
