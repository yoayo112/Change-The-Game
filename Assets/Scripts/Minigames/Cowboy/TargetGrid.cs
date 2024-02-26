using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGrid
{
    private GameObject target; //reference to target sprite.
    private Quaternion rotation; // Rotation value for Instantiate method
    private Dictionary<int, Vector3> grid = new Dictionary<int, Vector3>(); // Positions of boxes in grid
    private Dictionary<int, GameObject> currentTargets = new Dictionary<int, GameObject>(); // game objects by position
    private Dictionary<int, bool> hasTarget = new Dictionary<int, bool>(); // true if target in position
    private Dictionary<int, float> timeAlive = new Dictionary<int, float>(); // Age of target in position
    private float maxAge; // Maximum age a target can be


    public TargetGrid(float maxTime, GameObject targetSprite, Dictionary<int,Vector3> GridPos, Quaternion rotate)
    {
        maxAge = maxTime;
        target = targetSprite;
        grid = GridPos;
        rotation = rotate;

        SetupGrid();
    }

    public bool HasTarget(int pos)
    {
        return hasTarget[pos];
    }

    public float TimeAlive(int pos)
    {
        return timeAlive[pos];
    }

    public void AddTime(int pos, float delta)
    {
        timeAlive[pos] += delta;
    }
    public bool TooOld(int pos)
    {
        return (TimeAlive(pos) > maxAge);
    }

    public void CreateTarget(int pos)
    {
        if(!hasTarget[pos])
        {
            currentTargets[pos] = UnityEngine.Object.Instantiate(target,grid[pos],rotation);
            hasTarget[pos] = true;
            timeAlive[pos] = 0f;
        }
    }
    
    public void KillTarget(int pos)
    {
        timeAlive[pos] = 0f;
        hasTarget[pos] = false;
        UnityEngine.Object.Destroy(currentTargets[pos]);
    }

    public void KillAll()
    {
        for(int i = 1; i <=9; i++)
        {
            if(hasTarget[i]){KillTarget(i);}
        }
    }

    private void SetupGrid()
    {
        for(int i = 1; i <= 9; i++)
        {
            currentTargets.Add(i, null);
            hasTarget.Add(i, false);
            timeAlive.Add(i, 0f);
        }
    }

}
