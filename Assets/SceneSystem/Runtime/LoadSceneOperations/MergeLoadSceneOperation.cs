using System;
using System.Linq;

namespace AnnulusGames.SceneSystem.LoadSceneOperations
{
    public sealed class MergeLoadSceneOperation : LoadSceneOperationBase
    {
        public MergeLoadSceneOperation(params LoadSceneOperationBase[] operations)
        {
            this.operations = operations;
        }

        private LoadSceneOperationBase[] operations;
        private bool hasExecuted;

        public override LoadSceneOperationHandle Execute()
        {
            if (hasExecuted) throw new InvalidOperationException();
            hasExecuted = true;

            foreach (var operation in operations)
            {
                operation.onCompleted += () =>
                {
                    if (this.IsDone) this.onCompleted?.Invoke();
                };
                if (!operation.HasExecuted) operation.Execute();
            }

            return new LoadSceneOperationHandle(this);
        }

        public override void AllowSceneActivation(bool allowSceneActivation)
        {
            foreach (var operation in operations) operation.AllowSceneActivation(allowSceneActivation);
        }

        public override bool IsDone => operations.All(x => x.IsDone);
        public override bool HasExecuted => hasExecuted;
        public override float Progress => operations.Average(x => x.Progress);
        public override event Action onCompleted;
    }
}