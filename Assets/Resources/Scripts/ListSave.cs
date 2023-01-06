using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListSave : MonoBehaviour {

    private static ListSave _instance;
    private List<TrackInfo> trackList;

    private void Awake() {
        if (!_instance) _instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        trackList = new List<TrackInfo>();
    }

    public List<TrackInfo> GetTrackList(){
        return trackList;
    }

}
