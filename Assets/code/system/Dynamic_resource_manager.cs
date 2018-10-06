using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic_resource_manager : MonoBehaviour {

    public static Dynamic_resource_manager instance;

    public GameObject[] woods;
    public GameObject[] foods;

    public float wood_respawn_cd, wood_last_respawn;
    public float food_respawn_cd, food_last_respawn;
    public int wood_count, food_count, max_wood, max_food;

	// Use this for initialization
	void Start () {
        instance = this;

        wood_last_respawn = -wood_respawn_cd;
        food_last_respawn = -food_respawn_cd;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - wood_last_respawn > wood_respawn_cd && wood_count < max_wood)
            Spawn_wood();
        if (Time.time - food_last_respawn > food_respawn_cd && food_count < max_food)
            Spawn_food();
	}

    void Spawn_wood()
    {
        int index = Random.Range(0, 6);
        while(woods[index].activeSelf == true)
        {
            index = Random.Range(0, 6);
        }
        woods[index].SetActive(true);
        wood_count += 1;
        wood_last_respawn = Time.time;
    }

    void Spawn_food()
    {
        int index = Random.Range(0, 6);
        while (foods[index].activeSelf == true)
        {
            index = Random.Range(0, 6);
        }
        foods[index].SetActive(true);
        food_count += 1;
        food_last_respawn = Time.time;
    }

    public void Destroy_Wood(GameObject target)
    {
        target.SetActive(false);
        wood_count -= 1;
    }

    public void Destroy_Food(GameObject target)
    {
        target.SetActive(false);
        food_count -= 1;
    }
}
