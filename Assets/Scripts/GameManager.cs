using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public GameObject enemySpawn;
	public float GameTime = 0f;

	public JSONObject currentWaves;

	private List<List<SpawnEnemys.SpawnValue>> EnemyWave = new List<List<SpawnEnemys.SpawnValue>>();

	// Use this for initialization
	void Awake()
	{
		TextAsset textAsset = (TextAsset)Resources.Load("Waves_Lvl01", typeof(TextAsset));
		if (textAsset == null) { Debug.LogError("Missing Resources/Waves_Lvl01.txt !"); return; };
		//Debug.Log(textAsset);
		currentWaves = JSONParser.parse(textAsset.text);
		Debug.Log("Waves loaded:" + currentWaves["Name"]);

		JSONObject jsonWaves = currentWaves["Waves"];
		
		for (int i = 0; i < jsonWaves.Count; i++)
		{
			EnemyWave.Add(new List<SpawnEnemys.SpawnValue>());

			JSONObject jsonMobs = jsonWaves[i]["Mobs"];

			for (int j = 0; j < jsonMobs.Count; j++)
			{
				EnemyWave[i].Add(new SpawnEnemys.SpawnValue((string)jsonMobs[j]["Type"], (float)jsonMobs[j]["CoolDown"]));
			}
		}

		enemySpawn.GetComponent<SpawnEnemys>().SetWaveList(EnemyWave);
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameTime += Time.deltaTime;
	}
}
