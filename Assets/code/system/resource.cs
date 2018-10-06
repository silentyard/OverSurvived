using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resource : MonoBehaviour
{
    Sprite ori_sprite;
    public Sprite ing_sprite , after_sprite;
    public float time;                              //採集所需時間
    public int duration;
    public bool wood, food, fire , mutex , own_by_P1 , buildingHouse , completed, ship;

    bool stop_cor_while ;
    //audio clip
    public AudioClip chop, burn, build, get_food;
    
    singleton_manager manager;
    character_control control_script;
    AudioSource audio_source;
    SpriteRenderer sprite;
    BoxCollider2D box_col;

    void Start()
    {
        manager = singleton_manager.get_sigleton();
        audio_source = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        box_col = GetComponent<BoxCollider2D>();
        ori_sprite = sprite.sprite;
    }

    private void OnEnable()
    {
        refresh_status();
    }

    void refresh_status()
    {
        if (ori_sprite)
        {
            sprite.sprite = ori_sprite;
            if (!box_col)
                box_col = GetComponent<BoxCollider2D>();
            
            box_col.enabled = true;
            completed = false;
            mutex = false;
        }
        else
            Start();
    }

    public void start_collect() {
        if (!completed)
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
            UI_controller.instance.P1_progress_bar.transform.position = transform.position + 6 * Vector3.up;
            UI_controller.instance.P1_child.fillAmount = 0;

            float total_frame = (time / control_script.ability_power[control_script.resource_type]) / Time.deltaTime;

            while ( !stop_cor_while && UI_controller.instance.P1_child.fillAmount < 0.98f )
            {
                UI_controller.instance.P1_child.fillAmount += 0.98f / total_frame;
                yield return null;
            }
            UI_controller.instance.P1_progress_bar.gameObject.SetActive(false);
        }
        else
        {
            UI_controller.instance.P2_progress_bar.gameObject.SetActive(true);
            UI_controller.instance.P2_progress_bar.transform.position = transform.position + 6 * Vector3.up;
            UI_controller.instance.P2_child.fillAmount = 0;

            float total_frame = (time / control_script.ability_power[control_script.resource_type]) / Time.deltaTime;

            while ( !stop_cor_while && UI_controller.instance.P2_child.fillAmount < 0.98f)
            {
                UI_controller.instance.P2_child.fillAmount += 0.98f / total_frame;
                yield return null;
            }
            UI_controller.instance.P2_progress_bar.gameObject.SetActive(false);
        }

        if (!stop_cor_while)
        {
            control_script.collecting = false;
            completed = true;
            if (wood || food) {
                GetComponent<BoxCollider2D>().enabled = false;
                if (wood)
                {
                    manager.wood++;
                    UI_controller.instance.AddWood();
                    Dynamic_resource_manager.instance.Destroy_Wood(gameObject);//delay setActive(false)
                }
                else if (food)
                {
                    manager.food++;
                    UI_controller.instance.AddFood();
                    Dynamic_resource_manager.instance.Destroy_Food(gameObject);
                }
            }
            else if (buildingHouse)
            {
                manager.house_duration.Add(duration);
                UI_controller.instance.Add_Duration_Bar(transform.position + 6 * Vector3.up , false);
                StartCoroutine(count_down(duration));
            }
            else if (fire)
            {
                manager.fire_duration.Add(duration);
                UI_controller.instance.Add_Duration_Bar(transform.position + 6 * Vector3.up , true);
                StartCoroutine(count_down(duration));
            }
            else if (ship)
            {
                print(control_script.transform.name + " take ship");
                manager.gameover(control_script.transform.name);
            }
            sprite.sprite = after_sprite;
        }
        else
        {
            sprite.sprite = ori_sprite;
        }

        control_script.animator.SetBool("collecting", false);
        control_script.animator.SetBool("slashing", false);
        control_script.animator.SetBool("building", false);
        audio_source.Stop();
    }

    IEnumerator count_down(int time)
    {
        yield return new WaitForSeconds(time);
        refresh_status();
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
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if ((col.gameObject.transform.name == "player_1" && own_by_P1) || (col.gameObject.transform.name == "player_2" && !own_by_P1) )
        {
            mutex = false;
        }
    }
}
