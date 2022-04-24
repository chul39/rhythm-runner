using UnityEngine;

public class Payload : MonoBehaviour {

    private static Payload _instance;
    private TrackInfo ti;

    private void Awake() {
        if (!_instance) _instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject) ;
    }

    public void SetPayload(TrackInfo t) {
        ti = t;
    }

    public TrackInfo GetPayload() {
        return ti;
    }

}
