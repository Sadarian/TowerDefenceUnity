using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject enemySpawn;
	public float GameTime;

	// Use this for initialization
	void Awake ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameTime += Time.deltaTime;
	}
}
