using UnityEngine;
using System.Collections.Generic;

public class SpawnEnemys : MonoBehaviour
{
	public GameObject spawnPoint;
	public List<GameObject> EnemyPrefabs;
	public Dictionary<string, GameObject> Enemys;
	public float spawnCoolDown; 

	struct SpawnValue
	{
		private string enemyType;
		private int count;
		private float coolDown;

		public void SetEnemyType(string enemyType)
		{
			this.enemyType = enemyType;
		}
		public string GetEnemyType()
		{
			return enemyType;
		}
		
		public void SetCount(int count)
		{
			this.count = count;
		}
		public int GetCount()
		{
			return count;
		}

		public void SetCoolDown(float coolDown)
		{
			this.coolDown = coolDown;
		}
		public float GetCoolDown()
		{
			return coolDown;
		}
	}

	// Use this for initialization
	void Awake () 
	{
		foreach (GameObject enemyPrefab in EnemyPrefabs)
		{
			Enemys.Add(enemyPrefab.name, enemyPrefab);
		}
	}

	void SpawnEnemy(string enemyType, int count, float coolDown)
	{
		if (spawnCoolDown <= 0)
		{
			for (int i = 0; i < count; i++)
			{
				Instantiate(Enemys[enemyType], spawnPoint.transform.position, Quaternion.identity);
			}
			spawnCoolDown = coolDown;
		}
	}

	void SpawnEnemy(List<SpawnValue> EnemyList)
	{
		foreach (SpawnValue spawnValue in EnemyList)
		{
			SpawnEnemy(spawnValue.GetEnemyType(), spawnValue.GetCount(), spawnValue.GetCoolDown());
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (spawnCoolDown > 0)
		{
			spawnCoolDown -= Time.deltaTime;
		}
	}
}
