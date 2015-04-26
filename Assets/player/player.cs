using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public class PlayerStats {
		public int lives = 3;
		public int percentage;
	}
	
	public PlayerStats playerstats = new PlayerStats();
	
	void Update() {
		if (transform.position.x < -10 || transform.position.y < -6 || transform.position.x > 10 || transform.position.y > 6) {
			GameMaster.KillPlayer(this);
		}
	}
}