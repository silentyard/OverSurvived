using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackControl : MonoBehaviour {
    singleton_manager manager;
    float cost;
    bool audio_isPlaying;

    void Start () {
        manager = singleton_manager.get_sigleton();
        manager.dead_count = 0;
    }

    private void Update()
    {
        for (int i = 0; i < UI_controller.instance.fire_bar_list.Count; i++)
        {
            cost = manager.fire_duration[i] / Time.deltaTime;
            if (UI_controller.instance.fire_bar_list[i].fillAmount > 0)
                UI_controller.instance.fire_bar_list[i].fillAmount -= 1 / cost;

            if (UI_controller.instance.fire_bar_list[i].fillAmount <= 0 && UI_controller.instance.fire_bar_list[i].transform.parent.gameObject.activeSelf)
            {
                UI_controller.instance.fire_bar_list[i].fillAmount = 0;
                UI_controller.instance.fire_bar_list[i].transform.parent.gameObject.SetActive(false);
            }
        }

        //fire audio
        if (UI_controller.instance.fire_exist() && !audio_isPlaying)
        {
            GetComponent<AudioSource>().Play();
            audio_isPlaying = true;
        }
        else if(!UI_controller.instance.fire_exist() && audio_isPlaying)
        {
            GetComponent<AudioSource>().Stop();
            audio_isPlaying = false;
        }

        for (int i = 0; i < UI_controller.instance.house_bar_list.Count; i++)
        {
            cost = manager.house_duration[i] / Time.deltaTime;
            if (UI_controller.instance.house_bar_list[i].fillAmount > 0)
                UI_controller.instance.house_bar_list[i].fillAmount -= 1 / cost;

            if (UI_controller.instance.house_bar_list[i].fillAmount <= 0 && UI_controller.instance.house_bar_list[i].transform.parent.gameObject.activeSelf)
            {
                UI_controller.instance.house_bar_list[i].fillAmount = 0;
                UI_controller.instance.house_bar_list[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
