using UnityEngine;

public class InGame_SpinningSaw : MonoBehaviour {

    private void Update() {
        transform.Rotate(Vector3.forward * Time.deltaTime * 100);
    }
    
}
