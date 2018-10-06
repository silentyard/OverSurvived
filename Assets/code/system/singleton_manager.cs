using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singleton_manager : ScriptableObject
{
    public static singleton_manager singleton = null;
    public int dead_count , wood = 2, food = 2 , wood_cost_by_house, wood_cost_by_ship , result; // 0 for P1 win , 1 for P2 win , 2 for both win , 3 for both dead
    public List<float> fire_duration = new List<float>();
    public List<float> house_duration = new List<float>();

 
    private singleton_manager()
    {
        wood_cost_by_house = 2;
        wood_cost_by_ship = 3;
    }

    public static singleton_manager get_sigleton()
    {
        if (singleton == null)
        {
            singleton = ScriptableObject.CreateInstance<singleton_manager>();
        }
        return singleton;
    }

    public void gameover(string caller_name)
    {
        
        if (caller_name == "player_1")          // serviver
            result = 0;
        else if (caller_name == "player_2")
            result = 1;
        else if (dead_count == 0)
            result = 2;
        else if (dead_count == 2 || (dead_count == 1 && caller_name == "timeup"))
            result = 3;

        Initailize.initialize_script.fade_out();
    }
}
