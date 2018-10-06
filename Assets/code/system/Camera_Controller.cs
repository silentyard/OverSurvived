using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera_Controller : MonoBehaviour {

    public float smooth_limit = 5;
    public float speed = 5;
    public static Camera_Controller instance;
    bool P1_controlling = true;

    //underbar
    public Image underbar;
    public Sprite underbar_P1, underbar_P2;
    float movement;
    // Use this for initialization
    void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        CameraMove();
	}

    void CameraMove()
    {
        if (P1_controlling)
            movement = Input.GetAxis("P1_Camera");
        else
            movement = Input.GetAxis("P2_Camera");

        transform.position = new Vector3(Mathf.Clamp(transform.position.x + Mathf.SmoothStep(0, movement, smooth_limit) * speed * Time.deltaTime , -245 , 245) , transform.position.y , transform.position.z);
    }

    public void Switch_host()
    {
        if (P1_controlling)
        {
            P1_controlling = false;
            underbar.sprite = underbar_P2;
        }
        else
        {
            P1_controlling = true;
            underbar.sprite = underbar_P1;
        }
    }
}
