using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TrackPlayer {

    protected AudioSource audioSrc;

    public TrackPlayer(AudioSource newAudioSrc) {
        this.audioSrc = newAudioSrc;
    }

    public IEnumerator GetAudioClip(string url, string type) {
        switch(type) {
            case "mp3":
                WWW mp3 = new WWW(url);
                while (!mp3.isDone) {
                    yield return 0;
                }
                audioSrc.clip = NAudioPlayer.FromMp3Data(mp3.bytes);
                break;
            case "wav":
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV)) {
                    yield return www.SendWebRequest();
                    if (www.isNetworkError) {
                        Debug.Log(www.error);
                    } else {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        if (clip) audioSrc.clip = clip;
                    }
                }
                break;
            case "ogg":
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS)) {
                    yield return www.SendWebRequest ();
                    if (www.isNetworkError) {
                        Debug.Log(www.error);
                    } else {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent (www);
                        if (clip) audioSrc.clip = clip;
                    }
                }
                break;
        }
    }

    public int GetBpm() {
        return UniBpmAnalyzer.AnalyzeBpm(audioSrc.clip);
    }

}