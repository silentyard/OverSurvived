using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class character_control : MonoBehaviour
{
    public bool player_1, collecting;
    public Vector2 max_speed, current_speed;
    public float player_hp = 1;
    public float lack_of_food_dmg, lack_of_fire_dmg, lack_of_house_dmg , out_of_screen;
    public int resource_type;        // 0 for wood/build/ship , 1 for food , 2 for fire
    public float[] ability_power , tired , tired_cost;
    public float tired_backup_speed, tire_tradeoff_ratio = 0.4f;
    float inputX, inputY;
    singleton_manager manager;
    Rigidbody2D rigid;
    public Animator animator;
    resource resource_script;
    Vector2 last_position;

    public Sprite P1_die, P2_die;

    void Start()
    {
        manager = singleton_manager.get_sigleton();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        last_position = transform.position;
        for (int i = 0; i < 3; i++)
        {
            tired[i] = 1;
        }


        if (player_1) { 
            if (Random.value > 0.5f) { 
                ability_power[0] = Random.Range(0.5f, 0.7f);
                ability_power[1] = Random.Range(0.3f, 0.5f);
            }
            else { 
                ability_power[0] = Random.Range(0.3f, 0.5f);
                ability_power[1] = Random.Range(0.5f, 0.7f);
            }
            ability_power[2] = Random.Range(0.4f, 0.6f);
        }
        else
        {
            character_control p1_script = GameObject.Find("player_1").GetComponent<character_control>();
            for (int i = 0; i < 3; i++)
            {
                ability_power[i] = 1 - p1_script.ability_power[i];
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player_hp <= 0)
            return;
        moving_control();
        check_lack_of_resource();
        tired_backup();
    }

    void moving_control()
    {

        if (player_1)
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
        }
        else
        {
            inputX = Input.GetAxis("Horizontal_wasd");
            inputY = Input.GetAxis("Vertical_wasd");
        }

        animator.SetFloat("x-axi", inputX);
        animator.SetFloat("y-axi", inputY);
        current_speed = new Vector2(max_speed.x * inputX, max_speed.y * inputY);
        rigid.velocity = current_speed;

        last_position = transform.position;
    }

    void check_lack_of_resource()
    {
        if(manager.food <= 0)
        {
            take_damage(lack_of_food_dmg);
            print("dmg from food");
        }
        if(!UI_controller.instance.fire_exist() )
        {
            take_damage(lack_of_fire_dmg);
            print("dmg from fire");
        }
        if(!UI_controller.instance.house_exist())
        {
            take_damage(lack_of_house_dmg);
            print("dmg from house");
        }
        if ( Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > 55)
        {
            take_damage(out_of_screen);
            print(transform.name + " dmg from screen");
        }
    }

    void take_damage(float dmg)
    {
        player_hp -= dmg * Time.deltaTime;
        if (player_1)
            UI_controller.instance.UpdateP1_health(player_hp);
        else
            UI_controller.instance.UpdateP2_health(player_hp);

        if (player_hp <= 0)
        {
            rigid.velocity = Vector3.zero;
            GetComponent<BoxCollider2D>().enabled = false;
            animator.enabled = false;
            manager.dead_count += 1;
            if (player_1)
                GetComponent<SpriteRenderer>().sprite = P1_die;
            else
                GetComponent<SpriteRenderer>().sprite = P2_die;
        }

        if (manager.dead_count == 2)
            manager.gameover("ace");//gameover
    }

    void tired_backup()
    {
        for(int i = 0; i < 2; i++)
        {
            tired[i] += tired_backup_speed;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (!collecting && ( (player_1 && Input.GetKeyDown(KeyCode.Slash)) || (!player_1 && Input.GetKeyDown(KeyCode.Space))) && col.transform.tag == "resource" )
        {
            resource_script = col.gameObject.GetComponent<resource>();
            if ((player_1 && resource_script.own_by_P1) || (!player_1 && !resource_script.own_by_P1))
            {
                
                if (resource_script.wood || resource_script.buildingHouse || resource_script.ship)
                    resource_type = 0;
                else if (resource_script.food)
                    resource_type = 1;
                else if (resource_script.fire)
                    resource_type = 2;
                else
                    resource_type = -1;

                if (!resource_script.completed && tired[resource_type] >= tired_cost[resource_type] && ( (resource_script.buildingHouse && manager.wood >= manager.wood_cost_by_house) || (resource_script.ship && manager.wood >= manager.wood_cost_by_ship) || (!resource_script.ship && !resource_script.buildingHouse) ) )
                {
                    collecting = true;

                    if (col.transform.position.x < transform.position.x)
                    {
                        animator.SetBool("left", true);
                        animator.SetBool("right", false);
                    }
                    else
                    {
                        animator.SetBool("right", true);
                        animator.SetBool("left", false);
                    }

                    if (resource_script.wood)
                    {
                        animator.SetBool("slashing", true);
                    }
                    else if (resource_script.food)
                    {
                        animator.SetBool("collecting", true);
                    }
                    else if (resource_script.buildingHouse)
                    {
                        animator.SetBool("building", true);
                        manager.wood -= manager.wood_cost_by_house;
                        for (int i = 0; i < manager.wood_cost_by_house; i++)
                            UI_controller.instance.DeleteWood();
                    }

                    else if (resource_script.ship)
                    {
                        animator.SetBool("building", true);
                        manager.wood -= manager.wood_cost_by_ship;
                        for (int i = 0; i < manager.wood_cost_by_ship; i++)
                            UI_controller.instance.DeleteWood();
                    }


                    resource_script.start_collect();
                    tired[resource_type] -= tired_cost[resource_type];
                    for(int i = 0; i < 2; i++)
                    {
                        if (i != resource_type)
                            tired[i] += tire_tradeoff_ratio * tired_cost[resource_type]; 
                    }
                    if (player_1)
                        UI_controller.instance.UpdateP1(tired[0], tired[1], tired[2]);
                    else
                        UI_controller.instance.UpdateP2(tired[0], tired[1], tired[2]);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (collecting && col.transform.tag == "resource")
            resource_script.stop_collect();
    }
}
