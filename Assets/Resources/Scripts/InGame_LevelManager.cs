using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class InGame_LevelManager : MonoBehaviour {

    public enum State{
        Awake,
        Init,
        Play,
        Pause,
        NearEnd,
        End
    }

    public State levelState, prevState;
    
    private int itemObtained, itemMissed, comboNum, spawnListIndex, currentVal;
    private float time, objectSpeed, percent, bpm, invokeDelay, multiplier, activeTime, maxTime;
    private double scorePercent;

    private InGame_LoadAudio i_la;
    private InGame_ObjectPool i_op;
    private InGame_PlayerControl i_pc;

    private TrackInfo gameTrack;
    private TrackPlayer tp;
    private AudioSource audioSrc;

    [SerializeField] private Slider sd;
    [SerializeField] private GameObject introPanel, gameOverPanel, resultPanel, pausePanel, startTrigger, endingTrigger;
    [SerializeField] private Text scoreText, combo, resultText;

    private void Start () {
        levelState = State.Awake;
        audioSrc = GetComponent<AudioSource>();
        tp = new TrackPlayer(audioSrc);
        gameTrack = GameObject.Find("Payload").GetComponent<Payload>().GetPayload();
        i_la = GameObject.Find ("LevelGenerator").GetComponent<InGame_LoadAudio>();
        i_op = GameObject.Find("LevelGenerator").GetComponent<InGame_ObjectPool>();
        i_pc = GameObject.Find ("RobotBoi").GetComponent<InGame_PlayerControl>();
        spawnListIndex = 0;
        objectSpeed = 0f;
        activeTime = 0f;
        time = 0f;
        GetTrack(gameTrack.GetPath());	
    }

    private void Update () {
        time += Time.deltaTime;
        switch (levelState) {
            case State.Awake:
                if (Input.anyKeyDown) {
                    StartCoroutine(WaitToIntialize());
                    introPanel.SetActive(false);
                    levelState = State.Init;
                }
                break;
            case State.Init:
                if (startTrigger.transform.position.x < 0f) levelState = State.Play;
                UpdatePause();
                break;
            case State.Play:
                PlayAudio();
                if (i_pc.playerState == InGame_PlayerControl.State.isDead) levelState = State.End;
                UpdatePause();
                break;
            case State.Pause:
                audioSrc.Pause();
                UpdatePause();
                break;
            case State.NearEnd:
                PlayAudio();
                UpdatePause();
                if (endingTrigger.transform.position.x <= 0f) {
                    FinalizeGameSession();
                    levelState = State.End;
                }
                break;
            case State.End:
                if (Input.anyKeyDown) backToMenu();
                break;
        }
    }

    private IEnumerator WaitToIntialize() {
        yield return new WaitForSeconds(3);
        InitializeSession();
    }

    private void UpdatePause() {
        if (levelState == State.Pause) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                levelState = prevState;
                pausePanel.SetActive(false);
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                prevState = levelState;
                levelState = State.Pause;
                pausePanel.SetActive(true);
            }
        }
    }

    public void Unpause() {
        levelState = prevState;
        pausePanel.SetActive(false);
    }

    private void PlayAudio() {
        try {
            if (!audioSrc.isPlaying && audioSrc.clip.loadState == AudioDataLoadState.Loaded) audioSrc.Play();
            if(audioSrc.isPlaying){
                activeTime += Time.deltaTime;
                percent = 100 * (activeTime / maxTime);
                sd.value = percent;
                UpdateScore();
                if (sd.value == 100) audioSrc.Stop();
            }
        } catch {
            Debug.Log("Cannot play audio");
        }
    }

    public void InitializeSession(){
        if (levelState == State.Init) {
            maxTime = audioSrc.clip.length;
            bpm = i_la.GetBpm ();
            multiplier = bpm > 200 ? 2f : 1f;
            i_pc.playerState = InGame_PlayerControl.State.isRunning;
            objectSpeed = (5.1f / (60 * multiplier / i_la.GetBpm ()));
            invokeDelay = i_la.GetInvokeDelay ();
            InvokeRepeating ("SpawnGround", 0.0f, multiplier * invokeDelay);
        }
    }

    private void SpawnGround(){
        try{
            currentVal = i_la.GetSpawnValue(spawnListIndex);
            if(currentVal == 11){
                CancelInvoke ("SpawnGround");
                i_op.GetObject(10);
                endingTrigger.SetActive (true);
                levelState = State.NearEnd;
            }
            if(levelState == State.Init ||levelState == State.Play){
                i_op.GetObject(currentVal);
                spawnListIndex++;
            }		
        }catch{
            Debug.Log ("CATCH");
        }
    }

    public void GetTrack(string url) {
        string type = url.Substring (url.Length - 3);
        StartCoroutine(tp.GetAudioClip (url, type));
    }

    public float GetObjectSpeed() {
        return objectSpeed;
    }

    public float GetBpm() {
        return bpm;
    }

    public void UpdateItemObtained() {
        itemObtained++;
        UpdateCombo(1);
    }

    public void UpdateItemMissed() {
        itemMissed++;
        UpdateCombo(0);
    }

    public void UpdateCombo(int status) {
        switch (status) {
            case 0:
                comboNum = 0;
                combo.text = "";
                break;
            case 1:
                comboNum++;
                if (comboNum >= 10) combo.text = comboNum + "x";
                break;
        }
    }

    public void UpdateScore() {
        if (itemObtained != 0) {
            float temp = 100f * ((float)itemObtained / (float)(itemObtained + itemMissed));
            scorePercent = System.Math.Round(temp, 2);
            scoreText.text = scorePercent + "%";
        }
    }

    public void FinalizeGameSession(){
        audioSrc.Stop();
        audioSrc.clip.UnloadAudioData();
        if (i_pc.playerState == InGame_PlayerControl.State.isDead) {
            gameOverPanel.SetActive(true);
        } else {
            resultText.text = CalculateResultGrade ();
            resultPanel.SetActive (true);
        }
    }

    public string CalculateResultGrade(){
        if (scorePercent == 100) {
            return "Rank SS (" + scorePercent + "%)";
        } else if (scorePercent >= 95) {
            return "Rank S (" + scorePercent + "%)";
        } else if (scorePercent >= 90) {
            return "Rank A (" + scorePercent + "%)";
        } else if (scorePercent >= 85) {
            return "Rank B (" + scorePercent + "%)";
        } else if (scorePercent >= 80) {
            return "Rank C (" + scorePercent + "%)";
        } else {
            return "Rank D (" + scorePercent + "%)";
        }
    }

    public void restartScene() {
        SceneManager.LoadScene("InGame_Main");
    }

    public void backToMenu() {
        SceneManager.LoadScene("Menu_TrackSelect");
    }

}
