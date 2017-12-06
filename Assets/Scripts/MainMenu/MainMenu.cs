using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private RectTransform MatchFinding;
        [SerializeField]
        private Button playBtn;

        private void Awake()
        {
            GameObject gObj = Instantiate(new GameObject());
            gObj.name = "Network";
            gObj.AddComponent<NetManager>();
            gObj.AddComponent<CreatureLoader>();
            playBtn.interactable = false;
            NetManager.Instance.regCallback += OnRegistration;
            NetManager.Instance.authCallback += OnAuthentication;
            NetManager.Instance.matchCreatedCallback += OnMatchCreated;
            new ProfileManager();
            ProfileManager.Instance.Load();
        }

        //AsyncOperation sceneLoad;
        public void PlayBtn()
        {
            //sceneLoad = SceneManager.LoadSceneAsync("Match", LoadSceneMode.Single);
            //sceneLoad.allowSceneActivation = false;
            DialogManager.Instance.OpenDialog(MatchFinding);
            NetManager.Instance.FindMatch();
        }

        public void CancelMatchmakeBtn()
        {
            DialogManager.Instance.CloseDialog(MatchFinding);
        }

        #region Networking
        private void OnConnect(object sender, EventArgs e)
        {

        }

        private void OnDisconnect(object sender, EventArgs e)
        {

        }

        private void OnRegistration(object sender, EventArgs e)
        {

        }

        private void OnAuthentication(object sender, EventArgs e)
        {
            playBtn.interactable = true;
        }

        private void OnMatchCreated(object sender, EventArgs e)
        {
            DialogManager.Instance.CloseDialog();
            SceneManager.LoadScene("Match", LoadSceneMode.Single);
            //sceneLoad.allowSceneActivation = true;
        }
        #endregion
    }
}