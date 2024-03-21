using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_globalInit : MonoBehaviour
{
    void Awake()
    {
        GameObject _player = GameObject.Find("COWBOY_PREFAB(Clone)");
        GameObject _priestess = GameObject.Find("PRIESTESS_PREFAB");
        
        GlobalService.Set_Player_Instance(_player);
        GlobalService.Get_Main().Set_Party(new List<GameObject>());
        GlobalService.Get_Main().Party_Push(_priestess, "PRIESTESS_PREFAB");
        List<string> enemies = new List<string>(); enemies.Add("SQUID_PREFAB"); enemies.Add("SQUID_PREFAB");
        GlobalService.Get_Main().Set_Enemies(enemies);

        _player.GetComponentInChildren<AudioSource>().Stop();
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
