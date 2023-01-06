using System.Collections.Generic;
using UnityEngine;

public class InGame_ObjectPool : MonoBehaviour {

    private GameObject g1, g2, g3, g4, g5, g6, g7, g8, g9, g10, gi;
    private Stack<GameObject> s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, si; 
    [SerializeField] private Vector3 spawnPosition;

    private void Awake() {
        s1 = new Stack<GameObject>();
        s2 = new Stack<GameObject>();
        s3 = new Stack<GameObject>();
        s4 = new Stack<GameObject>();
        s5 = new Stack<GameObject>();
        s6 = new Stack<GameObject>();
        s7 = new Stack<GameObject>();
        s8 = new Stack<GameObject>();
        s9 = new Stack<GameObject>();
        s10 = new Stack<GameObject>();
        si = new Stack<GameObject>();
        g1 = Resources.Load("Prefabs/Ground_L") as GameObject;
        g2 = Resources.Load("Prefabs/Ground_M") as GameObject;
        g3 = Resources.Load("Prefabs/Ground_A") as GameObject;
        g4 = Resources.Load("Prefabs/Ground_M_spike") as GameObject;
        g5 = Resources.Load("Prefabs/Ground_M_TB") as GameObject;
        g6 = Resources.Load("Prefabs/Ground_M_saw") as GameObject;
        g7 = Resources.Load("Prefabs/Ground_M_LR") as GameObject;
        g8 = Resources.Load("Prefabs/Ground_M_BB") as GameObject;
        g9 = Resources.Load("Prefabs/Ground_M_BT") as GameObject;
        g10 = Resources.Load("Prefabs/Ground_R") as GameObject;
        gi = Resources.Load("Prefabs/item") as GameObject;
    }

    private void InstantiateObject(int num) {
        GameObject temp;
        switch(num) {
            case 0:
                // DO NOTHING
                break;
            case 1:
                temp = Instantiate(g1, spawnPosition, transform.rotation);
                s1.Push(temp);
                break;
            case 2:
                temp = Instantiate(g2, spawnPosition, transform.rotation);
                s2.Push(temp);
                break;
            case 3:
                temp = Instantiate(g3, new Vector3 (20.4f, 0f, 0.1f), transform.rotation);
                s3.Push(temp);
                break;
            case 4:
                temp = Instantiate(g4, spawnPosition, transform.rotation);
                s4.Push(temp);
                break;
            case 5:
                temp = Instantiate(g5, spawnPosition, transform.rotation);
                s5.Push(temp);
                break;
            case 6:
                temp = Instantiate(g6, spawnPosition, transform.rotation);
                s6.Push(temp);
                break;
            case 7:
                temp = Instantiate(g7, spawnPosition, transform.rotation);
                s7.Push(temp);
                break;
            case 8:
                temp = Instantiate(g8, spawnPosition, transform.rotation);
                s8.Push(temp);
                break;
            case 9:
                temp = Instantiate(g9, spawnPosition, transform.rotation);
                s9.Push(temp);
                break;
            case 10:
                temp = Instantiate(g10, spawnPosition, transform.rotation);
                s10.Push(temp);
                break;
            case 99:
                temp = Instantiate(gi, spawnPosition, transform.rotation);
                si.Push(temp);
                break;
		}
	}

    public void GetObject(int num) {
        GameObject temp;
        switch (num) {
            case 0:
                // DO NOTHING
                break;
            case 1:
                if (s1.Count == 0) InstantiateObject(1);
                temp = s1.Pop();
                temp.transform.position = spawnPosition;
                temp.SetActive(true);
                SpawnItems(0);
                break;
            case 2:
                if (s2.Count == 0) InstantiateObject(2);
                temp = s2.Pop();
                temp.transform.position = spawnPosition;
                temp.SetActive(true);
                SpawnItems(0);
                break;
            case 3:
                if (s3.Count == 0)  InstantiateObject(3);
                temp = s3.Pop();
                temp.transform.position = new Vector3(20.4f, 0f, 0.1f);
                temp.SetActive(true);
                SpawnItems(1);
                break;
            case 4:
                if (s4.Count == 0)  InstantiateObject(4);
                temp = s4.Pop();
                temp.transform.position = spawnPosition;
                temp.SetActive(true);
                SpawnItems(1);
                break;
            case 5:
                if (s5.Count == 0) InstantiateObject(5);
                temp = s5.Pop();
                temp.transform.position = spawnPosition;
                temp.SetActive(true);
                SpawnItems(2);
                break;
            case 6:
                if (s6.Count == 0) InstantiateObject (6);
                temp = s6.Pop ();
                temp.transform.position = spawnPosition;
                temp.SetActive (true);
                SpawnItems (2);
                break;
            case 7:
                if (s7.Count == 0)  InstantiateObject (7);
                temp = s7.Pop ();
                temp.transform.position = spawnPosition;
                temp.SetActive (true);
                SpawnItems (0);
                break;
            case 8:
                if (s8.Count == 0)  InstantiateObject (8);
                temp = s8.Pop ();
                temp.transform.position = spawnPosition;
                temp.SetActive (true);
                SpawnItems (1);
                break;
            case 9:
                if (s9.Count == 0) InstantiateObject (9);
                temp = s9.Pop ();
                temp.transform.position = spawnPosition;
                temp.SetActive (true);
                SpawnItems (2);
                break;
            case 10:
                if (s10.Count == 0) InstantiateObject (10);
                temp = s10.Pop ();
                temp.transform.position = spawnPosition;
                temp.SetActive (true);
                SpawnItems (0);
                break;
        }
    }

    public void DeactiveObject(GameObject obj, int num) {
        obj.SetActive(false);
        switch (num) {
            case 0:
                Destroy(obj);
                break;
            case 1:
                s1.Push(obj);
                break;
            case 2:
                s2.Push(obj);
                break;
            case 3:
                s3.Push(obj);
                break;
            case 4:
                s4.Push(obj);
                break;
            case 5:
                s5.Push(obj);
                break;
            case 6:
                s6.Push(obj);
                break;
            case 7:
                s7.Push(obj);
                break;
            case 8:
                s8.Push(obj);
                break;
            case 9:
                s9.Push(obj);
                break;
            case 10:
                s10.Push(obj);
                break;
            case 99:
                si.Push(obj);
                break;
        }
    }

    private void SpawnItems(int posId) {
        GameObject temp;
        if (si.Count == 0) InstantiateObject(99);
        temp = si.Pop ();
        if (posId == 0) {
            temp.transform.position = new Vector3(21.42f, 2.5f, 0.1f);
            temp.SetActive(true);
            temp.GetComponent<InGame_ObjectControl>().SetItemId(0);
        } else if (posId == 1) {
            temp.transform.position = new Vector3(21.42f, 4f, 0.1f);
            temp.SetActive(true);
            temp.GetComponent<InGame_ObjectControl>().SetItemId(1);
        } else if (posId == 2) {
            temp.transform.position = new Vector3 (21.42f, 2f, 0.1f);
            temp.SetActive(true);
            temp.GetComponent<InGame_ObjectControl>().SetItemId(2);
        }
    }

}
