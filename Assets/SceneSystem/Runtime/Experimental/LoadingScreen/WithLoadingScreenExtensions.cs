namespace AnnulusGames.SceneSystem.Experimental
{
    public static class WithLoadingScreenExtensions
    {
        public static LoadSceneOperationHandle WithLoadingScreen(this LoadSceneOperationHandle self, ILoadingScreen loadingScreen)
        {
            self.AllowSceneActivation(false);
            loadingScreen.Show(self, () =>
            {
                self.AllowSceneActivation(true);
            });
            return self;
        }
    }
}