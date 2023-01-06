using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InGame_LoadAudio : MonoBehaviour {

    private TrackInfo gameTrack;
    private AudioSource audioSrc;
    private TrackAnalyzer ta;
    private float bpm, invokeDelay, trackProgress, prevTrackProgress, multiplier;
    private int spawnIndex, repeated, count;
    private bool wasObs, wasAcid, wasDoubleJump, wasInvoked = false;
    private List<int> SpectrumList, SpawnList;

    private void Awake() {
        audioSrc = GetComponent<AudioSource>();
        ta = new TrackAnalyzer (audioSrc);
        gameTrack = GameObject.Find("Payload").GetComponent<Payload>().GetPayload();
        SpectrumList = new List<int>();
        SpawnList = new List<int>();
        count = 0;
        prevTrackProgress = -1f;
        PlayTrack(gameTrack.GetPath());
    }

    private void Update() {
        try{
            if (!audioSrc.isPlaying && audioSrc.clip.loadState == AudioDataLoadState.Loaded) audioSrc.Play();
        }catch{
            Debug.Log("Cannot play audio source");
        }
        if (audioSrc.isPlaying) {
            trackProgress = Mathf.Clamp01(audioSrc.time / audioSrc.clip.length);
            if (!wasInvoked) {
                bpm = (float)ta.GetBpm();
                multiplier = bpm > 200 ? 2f: 1f;
                invokeDelay = 60f / (float)ta.GetBpm();
                InvokeRepeating("UpdateSpawnList", 0.0f, multiplier * invokeDelay);
                wasInvoked = true;
            }
            if (trackProgress < prevTrackProgress) {
                CancelInvoke("UpdateSpwanList");
                audioSrc.Stop();
                if ((new [] {1,3,10}).Contains(SpawnList[SpawnList.Count - 1])) SpawnList[SpawnList.Count - 2] = 2;        
                SpawnList[SpawnList.Count - 1] = 11;
            } else {
                ta.GenerateSpectrumData();
                ta.CreateFrequencyBands();
                ta.CreateAudioBand();
                prevTrackProgress = trackProgress;
            }
        }
    }

    public void PlayTrack(string url) {
        string type = url.Substring(url.Length - 3);
        StartCoroutine(ta.GetAudioClip(url, type));
    }

    private void UpdateSpawnList() {
        UpdateSpectrumList();
        FillSpawnList();
        spawnIndex++;
    }

    private void UpdateSpectrumList() {
        float[] temp = ta.GetAudioBand();
        int first = 0;
        float firstVal = temp[0];
        for (int i = 0; i < 8 ; i++) {
            if (temp[i] > firstVal) {
                firstVal = temp[i];
                first = i;
            }
        }
        SpectrumList.Add(first);
        count++;
    }

    /******************************* 
      Val    Object
      0      start
      1      ground_L					
      2      ground_M				
      3      ground_A			
      4      ground_M_spike			
      5      ground_M_saw			
      6      ground_M_TB		
      7      ground_M_LR
      8      ground_M_bullet_top
      9      ground_M_bullet_bottom
      10     ground_R
      11     end
    *******************************/
    private void FillSpawnList() {
        if (spawnIndex == 0) {
            SpawnList.Add(2);
        } else if (spawnIndex > 0) {
            int val0 = SpectrumList[spawnIndex - 1];
            int val1 = SpectrumList[spawnIndex];
            if (wasObs) {
                SpawnList.Add(2);
                wasObs = false;
                wasAcid = false;
            } else if (wasAcid) {
                if (val1 - val0 < 2 || val0 - val1 > 2) {
                    SpawnList.Add(1);
                    wasObs = false;
                    wasAcid = false;
                } else {
                    if (wasDoubleJump) {
                        SpawnList.Add(1);
                        wasObs = false;
                        wasAcid = false;
                    } else {
                        SpawnList.Add(3);
                        wasDoubleJump = true;
                    }
                }
            } else if (val1 - val0 >= 2) {
                if (val1 - val0 >= 4) {
                    SpawnList.Add(3);
                    SpawnList[spawnIndex - 1] = (SpawnList[spawnIndex - 2] == 3) ? 7 : 10;
                    wasAcid = true;
                } else {
                    SpawnList.Add(4);
                    wasObs = true;
                }
            } else if (val0 - val1 >= 2) {
                wasObs = true;
                SpawnList.Add(val0 - val1 >= 4 ? 5 : 6);
            } else {
                if (repeated == 2) {
                    SpawnList.Add(val1 < 4 ? 8 : 9);
                    wasObs = true;
                    repeated = 0;
				} else {
                    SpawnList.Add(2);
                    wasObs = false;
                    wasAcid = false;
                    repeated++;
                }
            }			
        }
    }

    public int GetSpawnValue(int index) {
        return SpawnList[index];
    }

    public string GetTrackName() {
        return gameTrack.GetName();
    }

    public float GetBpm() {
        return bpm;
    }

    public float GetInvokeDelay() {
        return invokeDelay;
    }

}