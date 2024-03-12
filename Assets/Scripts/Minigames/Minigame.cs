using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{

    public Canvas canvas;
    public CinemachineVirtualCamera virtualCamera;

    public Character target;
    public Player player;

    private Vector3 _cameraPos;
    private Quaternion _cameraRot;

    public void Set_Player(Player player_) => player = player_;
    public void Set_Target(Character target_) => target = target_;
    void Start()
    {
        _cameraPos = Camera.main.position;
        _cameraRot = Camera.main.rotation;
    }
    void Update()
    {
        
    }

    private void OnEnable() //Subscrizzle.
    {
        MinigameEventManager.onStart += Start_Game;
    }
    private void OnDisable() //De-subscrizzle
    {
        MinigameEventManager.onStart -= Start_Game;
    }

    public virtual void Start_Game() 
    {
        canvas = new Canvas();
        Move_Camera_POV();
        Aim_Camera_Target();
    }


}
