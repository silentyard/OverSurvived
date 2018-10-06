using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_controller : MonoBehaviour {

    
    public Image P1_work1, P1_work2, P1_work3 , P2_work1, P2_work2, P2_work3, P1_health, P2_health, P1_progress_bar, P2_progress_bar;
    public GameObject Player1, Player2, wood_group, food_group , bar_group , wood_prefab, food_prefab , bar_prefab;
    public static UI_controller instance;
    public float health_bar_y_offset;
    public Text time;

    Vector3 P1_rect_pos , P2_rect_pos;
    public Image P1_child, P2_child;
    List<GameObject> wood_list = new List<GameObject>(), food_list = new List<GameObject>();
    public List<Image> fire_bar_list = new List<Image>() , house_bar_list = new List<Image>();
    bool P1_stop_cor, P2_stop_cor;

    private void Awake()
    {
        instance = this;
        P1_child = P1_progress_bar.transform.GetChild(0).gameObject.GetComponent<Image>();
        P2_child = P2_progress_bar.transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        P1_health.transform.position = Camera.main.WorldToScreenPoint(Player1.transform.position + new Vector3(0, health_bar_y_offset, 0));
        P2_health.transform.position = Camera.main.WorldToScreenPoint(Player2.transform.position + new Vector3(0, health_bar_y_offset, 0));
    }

    #region public API

    public void UpdateP1(float work1, float work2, float work3)
    {
        P1_work1.fillAmount = work1;
        P1_work2.fillAmount = work2;
        P1_work3.fillAmount = work3;
    }

    public void UpdateP2(float work1, float work2, float work3)
    {
        P2_work1.fillAmount = work1;
        P2_work2.fillAmount = work2;
        P2_work3.fillAmount = work3;
    }

    public void UpdateP1_health(float hp_ratio)
    {
        P1_health.transform.GetChild(1).GetComponent<Image>().fillAmount = hp_ratio;
    }

    public void UpdateP2_health(float hp_ratio)
    {
        P2_health.transform.GetChild(1).GetComponent<Image>().fillAmount = hp_ratio;
    }

    public void UpdateTime(string text)
    {
        time.text = text;
    }

    public void Add_Duration_Bar(Vector3 position , bool is_fire)
    {
        bool has_empty = false;

        if (is_fire) { 
            for (int i = 0; i < fire_bar_list.Count; i++)
            {
                if (!fire_bar_list[i].transform.parent.gameObject.activeSelf)
                {
                    fire_bar_list[i].fillAmount = 1;
                    fire_bar_list[i].transform.parent.position = position;
                    fire_bar_list[i].transform.parent.gameObject.SetActive(true);
                    has_empty = true;
                    break;
                }
            }
            if (!has_empty) {     
                GameObject bar = Instantiate(bar_prefab, bar_group.transform);
                bar.transform.position = position;
                bar.SetActive(true);
                fire_bar_list.Add(bar.transform.GetChild(0).gameObject.GetComponent<Image>());
            }
        }
        else
        {
            for (int i = 0; i < house_bar_list.Count; i++)
            {
                if (!house_bar_list[i].transform.parent.gameObject.activeSelf)
                {
                    house_bar_list[i].fillAmount = 1;
                    house_bar_list[i].transform.parent.position = position;
                    house_bar_list[i].transform.parent.gameObject.SetActive(true);
                    has_empty = true;
                    break;
                }
            }
            if (!has_empty)
            {
                GameObject bar = Instantiate(bar_prefab, bar_group.transform);
                bar.transform.position = position;
                bar.SetActive(true);
                house_bar_list.Add(bar.transform.GetChild(0).gameObject.GetComponent<Image>());
            }
        }
    }

    public void AddWood()
    {
        GameObject wood = Instantiate(wood_prefab, wood_group.transform);
        wood_list.Add(wood);
    }

    public void AddFood()
    {
        GameObject food = Instantiate(food_prefab, food_group.transform);
        food_list.Add(food);
    }

    public void DeleteWood()
    {
        GameObject wood = wood_list[wood_list.Count - 1];
        wood_list.RemoveAt(wood_list.Count - 1);
        Destroy(wood);
    }

    public void DeleteFood()
    {
        GameObject food = food_list[food_list.Count - 1];
        food_list.RemoveAt(food_list.Count - 1);
        Destroy(food);
    }
    #endregion
}
