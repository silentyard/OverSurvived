using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resource : MonoBehaviour
{
    Sprite ori_sprite;
    public Sprite ing_sprite , after_sprite;
    public float time;                              //採集所需時間
    public int fire_duration , fire_index;
    public bool wood, food, fire , burning , mutex , own_by_P1 , buildingHouse;

    bool stop_cor_while;
    //audio clip
    public AudioClip chop, burn, build, get_food;
    
    singleton_manager manager;
    character_control control_script;
    AudioSource audio_source;
    SpriteRenderer sprite;

    void Start()
    {
        manager = singleton_manager.get_sigleton();
        audio_source = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        ori_sprite = sprite.sprite;
    }

    private void Update()
    {
        Burn();
    }

    void Burn()
    {
        if (burning && manager.fire[fire_index] <= 0)
        {
            burning = false;
            manager.fire[fire_index] = 0;
        }
    }

    public void start_collect() {
        StartCoroutine("collect_item");
    }

    public void stop_collect()
    {
        stop_cor_while = true;
        audio_source.Stop();
        control_script.collecting = false;
    }

    IEnumerator collect_item()
    {
        stop_cor_while = false;
        if (wood)
        {
            audio_source.clip = chop;
            audio_source.Play();
        }
        else if (food)
        {
            audio_source.clip = get_food;
            audio_source.Play();
        }
        else if (buildingHouse)
        {
            audio_source.clip = build;
            audio_source.Play();
        }
        sprite.sprite = ing_sprite;


        if (control_script.player_1)
        {
            UI_controller.instance.P1_progress_bar.gameObject.SetActive(true);
            UI_controller.instance.P1_progress_bar.transform.position = Camera.main.WorldToScreenPoint(transform.position + 5 * Vector3.up);
            UI_controller.instance.P1_child.fillAmount = 0;

            float total_frame = (time / control_script.ability_power[control_script.resource_type]) / Time.deltaTime;

            while ( !stop_cor_while && UI_controller.instance.P1_child.fillAmount < 0.95f )
            {
                UI_controller.instance.P1_child.fillAmount += 0.95f / total_frame;
                yield return null;
            }
            UI_controller.instance.P1_progress_bar.gameObject.SetActive(false);
        }
        else
        {
            UI_controller.instance.P2_progress_bar.gameObject.SetActive(true);
            UI_controller.instance.P2_progress_bar.transform.position = Camera.main.WorldToScreenPoint(transform.position + 5 * Vector3.up);
            UI_controller.instance.P2_child.fillAmount = 0;

            float total_frame = (time / control_script.ability_power[control_script.resource_type]) / Time.deltaTime;

            while ( !stop_cor_while && UI_controller.instance.P2_child.fillAmount < 0.95f)
            {
                UI_controller.instance.P2_child.fillAmount += 0.95f / total_frame;
                yield return null;
            }
            UI_controller.instance.P2_progress_bar.gameObject.SetActive(false);
        }

        if (!stop_cor_while)
        {
            if (wood || food)
                GetComponent<BoxCollider2D>().enabled = false;

            sprite.sprite = after_sprite;
            if (wood)
            {
                manager.wood++;
                UI_controller.instance.AddWood();
            }
            else if (fire && !burning)
            {
                manager.fire[fire_index] = fire_duration;
                burning = true;
            }
            else if (food)
            {
                manager.food++;
                UI_controller.instance.AddFood();
            }
        }
        else
        {
            sprite.sprite = ori_sprite;
        }

        audio_source.Stop();
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (!mutex && col.gameObject.tag == "player")
        {
            mutex = true;
            control_script = col.gameObject.GetComponent<character_control>();

            if (col.transform.name == "player_1")
                own_by_P1 = true;
            else
                own_by_P1 = false;
            print(col.transform.name);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if ((col.gameObject.transform.name == "player_1" && own_by_P1) || (col.gameObject.transform.name == "player_2" && !own_by_P1) )
        {
            mutex = false;
            print("release mutex");
        }
    }
}
