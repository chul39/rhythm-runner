using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_ListManager : MonoBehaviour {

    [SerializeField] private GameObject trackBtn0, trackBtn1, trackBtn2, trackBtn3, trackBtn4, trackBtn5, trackBtn6, trackBtn7, trackBtn8, backBtn, nextBtn;
    [SerializeField] private Text pageNumber, TrackName, TrackPath, TrackFileType, TrackLength, MatchedBpm, GameBpm;
    private float bpm, gBpm;
    private AudioSource audioSrc;
    private GameObject[] btnArray;
    private List<TrackInfo> trackList;
    private bool trackSelected, textFilled;
    private int btnSelectedNum, pageNum;
    private Menu_LoadAudio m_la;
    private ListSave ls;

    private void Start() {
        audioSrc = GetComponent<AudioSource>();
        try{
            audioSrc.clip.UnloadAudioData();
        }catch{
            Debug.Log("Failed to unload audio");
		}
        ls = GameObject.Find("ListSave").GetComponent<ListSave>();
        trackList = ls.GetTrackList();
        m_la = GetComponent<Menu_LoadAudio>();
        btnArray = new GameObject[] {
            trackBtn0,
            trackBtn1,
            trackBtn2,
            trackBtn3,
            trackBtn4,
            trackBtn5,
            trackBtn6,
            trackBtn7,
            trackBtn8,
            backBtn,
            nextBtn
        };
        pageNum = 1;
        trackSelected = false;
        RefreshList();
    }
	
    private void Update() {
        try {
            if (trackSelected) {
                try{
                    TrackLength.text = TrackLengthFormat (audioSrc.clip.length);
                    if (m_la.GetBpm() == -1) {
                        m_la.SetBpm();
                    } else {
                        bpm = m_la.GetBpm();
                        MatchedBpm.text = "" + bpm;
                        gBpm = bpm;
                        if (bpm >= 200) gBpm /= 2;
                        GameBpm.text = "" + gBpm;
                    }
                } catch {
                    Debug.Log("Cannot get track info");
                }
            }
            if (!audioSrc.isPlaying && audioSrc.clip.loadState == AudioDataLoadState.Loaded) audioSrc.Play();
        } catch {
            // DO NOTHING
        }
    }

    public void RefreshList() {
        int refNum = (pageNum - 1) * 9;
        TrackInfo temp;
        btnArray[9].GetComponent<Button>().interactable = !(pageNum == 1);
        for (int i = 0; i < 9; i++) {
            if (refNum + i < trackList.Count) {
                temp = trackList [refNum + i];
                btnArray[i].GetComponentInChildren<Text>().text = temp.GetName();
                btnArray[i].GetComponent<Button>().interactable = true;
                if (i == 8 && !btnArray [10].GetComponent<Button>().interactable) btnArray[10].GetComponent<Button>().interactable = true;
            } else {
                btnArray[i].GetComponentInChildren<Text>().text = "-";
                btnArray[i].GetComponent<Button>().interactable = false;
                if (btnArray[10].GetComponent<Button>().interactable) btnArray[10].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ImportTrack() {
        TrackInfo temp = m_la.GetBrowseFile();
        if (temp.GetPath() != null) trackList.Add(temp);
        RefreshList();
    }

    public void DeleteTrack() {
        if (btnSelectedNum != -1) {
            if (audioSrc.isPlaying) {
                try {
                    audioSrc.clip.UnloadAudioData();
                } catch {
                    Debug.Log("Failed to delete track");
                }
            }
            trackList.RemoveAt (((pageNum - 1) * 9) + btnSelectedNum);
            TrackName.text = "";
            TrackPath.text = "";
            TrackFileType.text = "";
            TrackLength.text = "";
            MatchedBpm.text = "";
            GameBpm.text = "";
            m_la.resetBpm();
            trackSelected = false;
            RefreshList();
        }
    }

    public void GoToNextPage() {
        pageNum++;
        pageNumber.text = "" + pageNum;
        RefreshList();
    }

    public void GoToPrevPage() {
        if (pageNum > 1) {
            pageNum--;
            pageNumber.text = "" + pageNum;
            RefreshList();
        }
    }

    public void GetTrackInfo(int btnNum) {
        try {
            audioSrc.clip.UnloadAudioData();
        } catch {
            Debug.Log("Failed to unload audio data");
        }
        m_la.resetBpm();
        TrackInfo temp;
        btnSelectedNum = btnNum;
        temp = trackList[((pageNum - 1) * 9) + btnNum];
        m_la.PlayTrack(temp.GetPath ());
        audioSrc.Play();
        TrackName.text = temp.GetName();
        TrackPath.text = temp.GetPath();
        TrackFileType.text = temp.GetFileType();
        trackSelected = true;
    }

    public void ToNextScene() {
        GameObject.Find("Payload").GetComponent<Payload>().SetPayload(trackList[((pageNum - 1) * 9) + btnSelectedNum]);
        SceneManager.LoadScene("InGame_Main");
    }

    public string TrackLengthFormat(float f) {
        string s = string.Format("{0}:{1:00}", (int)f/60, (int)f%60);
        return s;
    }

}
