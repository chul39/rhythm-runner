using UnityEngine;

public class InGame_ObjectControl : MonoBehaviour {

    private InGame_LevelManager i_lm;
    private InGame_ObjectPool i_op;
    private int id;
    private int itemid;
    private float speed;

    private void Start() {
        i_lm = GameObject.Find("LevelManager").GetComponent<InGame_LevelManager>();
        i_op = GameObject.Find("LevelGenerator").GetComponent<InGame_ObjectPool>();
        speed = 0f;
        itemid = -1;
        GetObjectId();
    }
    
    private void Update() {
        switch (i_lm.levelState) {
            case InGame_LevelManager.State.Awake:
                // Do nothing
                break;
            case InGame_LevelManager.State.Init:
                UpdateObjectPosition();
                break;
            case InGame_LevelManager.State.Play:
                UpdateObjectPosition();
                break;
            case InGame_LevelManager.State.Pause:
                speed = 0f;
                break;
            case InGame_LevelManager.State.NearEnd:
                UpdateObjectPosition();
                break;
            case InGame_LevelManager.State.End:
                speed = 0f;
                break;
        }
        if (id == 99) {
            float posY = gameObject.transform.position.y;
            if (posY == 2.5f) {
                SetItemId (0);
            }else if(posY == 4f){
                SetItemId (1);
            }else if(posY == 2f){
                SetItemId (2);
            }
        }
    }

    private void UpdateObjectPosition() {
        if (speed == 0f) {
            speed = i_lm.GetObjectSpeed();
        }
        transform.position -= Vector3.right * (speed * Time.deltaTime);
        if (transform.position.x < -5f && id == 99) {
            i_lm.UpdateItemMissed();
            i_op.DeactiveObject (this.gameObject, id);
        }
        if (transform.position.x < -20f) {
            if (id == 0) {
                Destroy(this.gameObject);
            } else {
                i_op.DeactiveObject(this.gameObject, id);
            }
        }
    }

    public void SetItemId(int num) {
        itemid = num;
    }

    public int GetItemId() {
        return itemid;
    }

    private void GetObjectId() {
        switch (this.gameObject.name) {
            case "StartPoint":
                id = 0;
                break;
            case "Ground_L(Clone)":
                id = 1;
                break;
            case "Ground_M(Clone)":
                id = 2;
                break;
            case "Ground_A(Clone)":
                id = 3;
                break;
            case "Ground_M_spike(Clone)":
                id = 4;
                break;
            case "Ground_M_saw(Clone)":
                id = 5;
                break;
            case "Ground_M_TB(Clone)":
                id = 6;
                break;
            case "Ground_M_LR(Clone)":
                id = 7;
                break;
            case "Ground_BB(Clone)":
                id = 8;
                break;
            case "Ground_BT(Clone)":
                id = 9;
                break;
            case "Ground_R(Clone)":
                id = 10;
                break;
            case "item(Clone)":
                id = 99;
                break;
        }
    }

}
