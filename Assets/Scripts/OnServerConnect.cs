using UnityEngine;
using System.Collections;

public class OnServerConnect : MonoBehaviour {
	public GameObject playerPrefab;

	void OnConnectedToServer(){
		Network.Instantiate (playerPrefab, new Vector3 (0f, 1f, 0f), Quaternion.identity, 0);
	}
}
