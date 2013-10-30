﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	NavMeshAgent nav;
	GameObject player;
	
	// Use this for initialization
	void Start () {
		nav=GetComponent<NavMeshAgent>();
		player=GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		nav.destination=player.transform.position;
	}
}
