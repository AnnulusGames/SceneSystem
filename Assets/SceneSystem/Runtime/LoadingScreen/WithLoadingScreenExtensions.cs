namespace AnnulusGames.SceneSystem
{
    public static class WithLoadingScreenExtensions
    {
        public static LoadSceneOperationHandle WithLoadingScreen(this LoadSceneOperationHandle self, LoadingScreen loadingScreen)
        {
            loadingScreen.Show(self);
            return self;
        }
    }
}