using UnityEngine;
using System.Collections.Generic;

public class SpawnEnemys : MonoBehaviour
{
	public GameObject spawnPoint;
	public List<GameObject> EnemyPrefabs = new List<GameObject>();

	private const float WAVETIMER = 15f;
	public float nextWave = 0f;

	public Dictionary<string, GameObject> Enemys = new Dictionary<string, GameObject>();
	public float spawnCoolDown;

	private List<SpawnValue> Wave = new List<SpawnValue>();
	private List<List<SpawnValue>> WaveList = new List<List<SpawnValue>>(); 

	public struct SpawnValue
	{
		public string enemyType;
		public float coolDown;

		public SpawnValue(string enemyType, float coolDown)
		{
			this.enemyType = enemyType;
			this.coolDown = coolDown;
		}
	}

	// Use this for initialization
	void Awake ()
	{
		spawnPoint = this.gameObject;

		nextWave = WAVETIMER;

		foreach (GameObject enemyPrefab in EnemyPrefabs)
		{
			Enemys.Add(enemyPrefab.name, enemyPrefab);
		}
	}

	void SpawnEnemy(string enemyType, float coolDown)
	{
		Debug.Log("Spawn: " + enemyType + Enemys[enemyType]);
		Instantiate(Enemys[enemyType], spawnPoint.transform.position, Quaternion.identity);
		spawnCoolDown = coolDown;
	}

	public void SetSingelWave(List<SpawnValue> EnemyList)
	{
		Wave = EnemyList;
	}

	public void SetWaveList(List<List<SpawnValue>> EnemyList)
	{
		WaveList = EnemyList;
	}

	// Update is called once per frame
	void Update () 
	{
		if (spawnCoolDown > 0)
		{
			spawnCoolDown -= Time.deltaTime;
		}
		else
		{
			if (Wave.Count != 0)
			{
				SpawnEnemy(Wave[0].enemyType, Wave[0].coolDown);
				Wave.RemoveAt(0);
			}			
		}

		if (nextWave > 0)
		{
			nextWave -= Time.deltaTime;
			return;
		}

		if (WaveList.Count == 0)
		{
			return;
		}

		SetSingelWave(WaveList[0]);
		WaveList.RemoveAt(0);
		nextWave = WAVETIMER;
	}
}
