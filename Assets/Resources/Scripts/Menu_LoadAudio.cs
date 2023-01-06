using System.IO;
using UnityEngine;
using SFB;

[RequireComponent(typeof(AudioSource))]
public class Menu_LoadAudio : MonoBehaviour {

    private AudioSource audioSrc;
    private TrackPlayer tp;
    private TrackInfo newTrackinfo;
    private float receivedBpm, bpm;

    private void Start() {
        audioSrc = GetComponent<AudioSource>();
        tp = new TrackPlayer(audioSrc);
        bpm = -1;
    }

    public void PlayTrack(string url) {
        string type = url.Substring(url.Length - 3);
        StartCoroutine(tp.GetAudioClip(url, type));
    }

    public TrackInfo GetBrowseFile() {
        var extensions = new [] {
            new ExtensionFilter("Sound Files", "mp3", "wav", "ogg" ),
            new ExtensionFilter("All Files", "*" ),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        string path = paths[0];
        if (path.Length != 0) {
            name = Path.GetFileName(path);
            return newTrackinfo = new TrackInfo(name.Substring(0, name.Length - 4), path, path.Substring(path.Length - 3));
        } else {
            return newTrackinfo = new TrackInfo();
        }
    }

    public void SetBpm() {
        bpm = tp.GetBpm();
    }

    public float GetBpm() {
        return bpm;
    }

    public void resetBpm() {
        bpm = -1f;
    }
		
}
