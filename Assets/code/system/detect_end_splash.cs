using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class detect_end_splash : MonoBehaviour {

    public Sprite[] splash;
    singleton_manager manager;
    Image image;

    // Use this for initialization
    void Awake () {
        manager = singleton_manager.get_sigleton();
        image = GetComponent<Image>();
        image.sprite = splash[manager.result];
	}
}
