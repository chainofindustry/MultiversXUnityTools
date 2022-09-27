namespace ElrondUnityExamples
{
    public class HomeScreen : GenericUIScreen
    {
        //linked to the login options button in editor
        public void LoginOptions()
        {
            DemoScript.Instance.LoadScreen(Screens.Login);
        }
    }
}
