using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{

    public Canvas canvas;
    public Camera camera;
    void Start()
    {
        
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
    }

}
