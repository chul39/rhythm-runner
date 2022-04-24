using UnityEngine;

public class Menu_MenuManager : MonoBehaviour {

    [SerializeField] private GameObject TrackListPanel;
    [SerializeField] private GameObject TrackInfo;
    [SerializeField] private GameObject SettingPanel;

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenSetting() {
        TrackListPanel.SetActive(false);
        TrackInfo.SetActive(false);
        SettingPanel.SetActive(true);
	}

    public void CloseSetting() {
        TrackListPanel.SetActive(true);
        TrackInfo.SetActive(true);
        SettingPanel.SetActive(false);
	}

}
