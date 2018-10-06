using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class Initailize : MonoBehaviour {

    public static Initailize initialize_script;
    Animator animator;
    bool fading;

    void Awake () {
        initialize_script = this;
        Screen.SetResolution(1366, 768, false);
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        fade_in();
        fading = false;
        DontDestroyOnLoad(transform.gameObject);
        DontDestroyOnLoad(transform.parent.gameObject);
    }

	void Update () {
        if ( (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 2 )  && !fading && (Input.GetKeyDown(KeyCode.Space)))
        {
            fade_out();
        }
	}

    public void fade_out()
    {
        animator.Play("fade_out");
    }

    public void fade_in()
    {
        animator.Play("fade_in");
    }

    public void set_fading()
    {
        fading = true;
    }

    public void reset_fading()
    {
        fading = false;
    }

    public void next()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else if (SceneManager.GetActiveScene().buildIndex == 2)
            SceneManager.LoadScene(0);
        fade_in();
    }
}
