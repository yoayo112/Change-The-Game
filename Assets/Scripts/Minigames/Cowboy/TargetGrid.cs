using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGrid
{
    private GameObject _target; //reference to target sprite.
    private Quaternion _rotation; // Rotation value for Instantiate method
    private Transform _parent;
    private Vector3 _spriteScale;
    private Dictionary<int, Vector3> _grid = new Dictionary<int, Vector3>(); // Positions of boxes in grid
    private Dictionary<int, GameObject> _currentTargets = new Dictionary<int, GameObject>(); // game objects by position
    private Dictionary<int, bool> _hasTarget = new Dictionary<int, bool>(); // true if target in position
    private Dictionary<int, float> _timeAlive = new Dictionary<int, float>(); // Age of target in position
    private float _maxAge; // Maximum age a target can be


    public TargetGrid(float maxTime_,Transform parent_, GameObject targetSprite_,Vector3 spriteScale_, Dictionary<int,Vector3> GridPos_, Quaternion rotate_)
    {
        _maxAge = maxTime_;
        _parent = parent_;
        _target = targetSprite_;
        _spriteScale = spriteScale_;
        _grid = GridPos_;
        _rotation = rotate_;

        Setup_Grid();
    }

    public bool Has_Target(int pos_)
    {
        return _hasTarget[pos_];
    }

    public float Time_Alive(int pos_)
    {
        return _timeAlive[pos_];
    }

    public void Add_Time(int pos_, float delta_)
    {
        _timeAlive[pos_] += delta_;
    }
    public bool Is_Too_Old(int pos_)
    {
        return (Time_Alive(pos_) > _maxAge);
    }

    public void Create_Target(int pos_)
    {
        if(!_hasTarget[pos_])
        {
            _currentTargets[pos_] = UnityEngine.Object.Instantiate(_target,_grid[pos_],_rotation);
            _currentTargets[pos_].transform.SetParent(_parent);
            _currentTargets[pos_].transform.localScale = _spriteScale;
            _hasTarget[pos_] = true;
            _timeAlive[pos_] = 0f;
        }
    }
    
    public void Kill_Target(int pos_)
    {
        _timeAlive[pos_] = 0f;
        _hasTarget[pos_] = false;
        UnityEngine.Object.Destroy(_currentTargets[pos_]);
    }

    public void Kill_All()
    {
        for(int i_ = 1; i_ <=9; i_++)
        {
            if(_hasTarget[i_]){Kill_Target(i_);}
        }
    }

    private void Setup_Grid()
    {
        for(int i_ = 1; i_ <= 9; i_++)
        {
            _currentTargets.Add(i_, null);
            _hasTarget.Add(i_, false);
            _timeAlive.Add(i_, 0f);
        }
    }

}
