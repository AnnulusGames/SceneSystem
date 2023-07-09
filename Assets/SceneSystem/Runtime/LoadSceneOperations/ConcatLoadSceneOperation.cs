using System;
using System.Linq;

namespace AnnulusGames.SceneSystem.LoadSceneOperations
{
    public sealed class ConcatLoadSceneOperation : LoadSceneOperationBase
    {
        public ConcatLoadSceneOperation(params LoadSceneOperationBase[] operations)
        {
            this.operations = operations;
        }

        private LoadSceneOperationBase[] operations;
        private bool hasExecuted;
        private bool allowSceneActivation = true;

        public override LoadSceneOperationHandle Execute()
        {
            if (hasExecuted) throw new InvalidOperationException();
            hasExecuted = true;

            for (int i = 0; i < operations.Length; i++)
            {
                int index = i;
                if (index == operations.Length - 1)
                {
                    operations[index].onCompleted += () =>
                    {
                        this.onCompleted?.Invoke();
                    };
                }
                else
                {
                    operations[index].onCompleted += () =>
                    {
                        var next = operations[index + 1];
                        if (!next.HasExecuted) next.Execute();
                        if (allowSceneActivation) next.AllowSceneActivation(true);
                    };
                }
            }
            operations[0].Execute();
            OnAllowSceneActivationChanged();

            return new LoadSceneOperationHandle(this);
        }

        public override void AllowSceneActivation(bool allowSceneActivation)
        {
            this.allowSceneActivation = allowSceneActivation;
            if (hasExecuted) OnAllowSceneActivationChanged();
        }

        private void OnAllowSceneActivationChanged()
        {
            if (allowSceneActivation)
            {
                operations[0].AllowSceneActivation(true);
            }
            else
            {
                foreach (var operation in operations)
                {
                    operation.AllowSceneActivation(false);
                    if (!operation.HasExecuted) operation.Execute();
                }
            }
        }

        public override bool IsDone => operations.Last().IsDone;
        public override bool HasExecuted => hasExecuted;
        public override float Progress => operations.Sum(x => x.Progress) / operations.Length;
        public override event Action onCompleted;
    }
}