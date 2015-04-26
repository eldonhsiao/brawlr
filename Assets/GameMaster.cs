using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	public static GameMaster gm;
	
	// Use this for initialization
	void Start () {
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster>();
		}
	}
	
	public Transform playerPrefab;
	public Transform spawnPoint;
	
	// Update is called once per frame
	public void RespawnPlayer () {
		Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
	}
	
	public static void KillPlayer (Player player) {
		Destroy (player.gameObject);
		gm.RespawnPlayer();
	}
}
