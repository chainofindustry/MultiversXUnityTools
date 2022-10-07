using UnityEngine;
using UnityEngine.UI;

namespace ElrondUnityExamples
{
    public class HomeScreen : GenericUIScreen
    {
        public Text version;

        private void Start()
        {
            version.text = $"v.{Application.version}";
        }

        //linked to the login options button in editor
        public void LoginOptions()
        {
            DemoScript.Instance.LoadScreen(Screens.Login);
        }
    }
}
