/*
Project: Change the Game
File: CombatController.cs
Date Created: March 01, 2024
Author(s): Elijah Theander
Info:

State Machine based handling of turn based combat.
Directs Player and Enemy characters to take their turns,
Processes characters ending turns, and keeps track of
whether or not combat is finished.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatController : MonoBehaviour
{
    private List<CharacterStats> _enemiesList = new List<CharacterStats>(); //List holding the scripts of the enemies
    private List<CharacterStats> _playersList = new List<CharacterStats>(); //List holding the scripts of the players
    private List<CharacterStats> _turnQueue = new List<CharacterStats>(); // Total list managing turn order

    private int _turnNumber = 0; // Current turn in this combat scenario
    private int _currentTurnPos = 0; // Current turn Queue index

    private bool _playerVictory = false; // Only used for determining ending state of combat

    public enum CombatStates 
    {
        beginCombat,
        startTurn,
        endTurn,
        endCombat,
        waiting
    }

    public CombatStates currentState = CombatStates.beginCombat; //Keeps track of current combat state

    private void OnEnable()
    {
        CombatEventManager.onEndTurn += End_Turn_State;
    }
    private void OnDisable()
    {
        CombatEventManager.onEndTurn -= End_Turn_State;
    }

    void Start()
    {

        //Find all Player and enemy character controls, put them in a list, and sort.
        CharacterStats[] allCharacters = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        _turnQueue = new List<CharacterStats>(allCharacters);
        _turnQueue.Sort();

        //Break turnQueue into character and enemy lists
        foreach(CharacterStats character in _turnQueue)
        {
            if(character.myType == CharacterType.player)
            {
                _playersList.Add(character);
            }
            if(character.myType == CharacterType.enemy)
            {
                _enemiesList.Add(character);
            }
        }

        //Assign members of turnqueue list their place in line.
        for(int i = 0; i < _turnQueue.Count; i++)
        {
            _turnQueue[i].Set_QueuePosition(i);
            Debug.Log("Turn Queue indexes: " + _turnQueue[i].Get_Name() + "is at position: " + _turnQueue[i].Get_QueuePosition());

        }
    }

    // Update is called once per frame
    void Update()
    {

        //Checking State
        switch(currentState)
        {
            case CombatStates.beginCombat:
                Start_Turn();
                break;
            
            case CombatStates.startTurn:
                Start_Turn();
                break;

            case CombatStates.endTurn:
                End_Turn();
                break;

            case CombatStates.endCombat:
                End_Combat();
                break;

            case CombatStates.waiting:
                break;
        }
        
    }

    public void Start_Turn()
    {
        currentState = CombatStates.waiting;
        _turnNumber++;

        Debug.Log(
                      "-----------------------------------------------------\n"
                    + "Turn Number: " + _turnNumber + "\n"   
                    + "Current Queue Position: " + _currentTurnPos + "\n"
                    + "Current character: " + _turnQueue[_currentTurnPos].Get_Name() + "\n"
                    + "-----------------------------------------------------"
                );

        CombatEventManager.Start_Turn(_currentTurnPos);
    }

    public void End_Turn()
    {   
        Debug.Log(
                      "Ending Turn\n" + Print_Dead_Characters() + "\n"
                    + "----------------------------------------------"
                );

        currentState = CombatStates.waiting; //Tell State Machine to wait.

        //Pick who's going next turn.
        if(_currentTurnPos == _turnQueue.Count - 1)
        {
            _currentTurnPos = 0;
        }
        else
        {
            _currentTurnPos ++;
        }

        //Check for game over condition
        if(Is_Combat_Running())
        {
            currentState = CombatStates.startTurn;
        }
        else
        {
            currentState = CombatStates.endCombat;
        }
    }

    //Checks both enemy and player lists to see if one side is completely dead.
    public bool Is_Combat_Running()
    {
        int deadEnemies_ = 0;
        foreach(CharacterStats enemy_ in _enemiesList)
        {
            if(!enemy_.Is_Alive())
            {
                deadEnemies_ ++;
            }
        }

        int deadPlayers_ = 0;
        foreach(CharacterStats player_ in _playersList)
        {
            if(!player_.Is_Alive())
            {
                deadPlayers_++;
            }
        }

        //returning false here tells the game that combat is over.
        if(deadPlayers_ == _playersList.Count)
        {
            _playerVictory = false;
            return false;
        }

        if(deadEnemies_ == _enemiesList.Count)
        {
            _playerVictory = true;
            return false;
        }

        return true;
    }

    //This gets called by the onEndTurn event.
    //Tells the State Machine in Update() to call End_Turn()
    public void End_Turn_State()
    {
        currentState = CombatStates.endTurn;
    }

    public void End_Combat()
    {
        currentState = CombatStates.waiting;

        string victoryMessage_ = "";
        if (_playerVictory)
        {
            victoryMessage_ = "Player Victory!";
        }
        else
        {
            victoryMessage_ = "Player Defeat!";
        }
        Debug.Log("Combat Over! " + victoryMessage_);
        //Handle end state here, command to transition scene, post combat screen start, etc.
    }

    //Make a big string of who's dead on both sides.
    public string Print_Dead_Characters()
    {
        string msg_ = "\nDead Players: ";
        string temp_ = "";
        foreach(CharacterStats player_ in _playersList)
        {
            if(!player_.Is_Alive())
            {
                temp_ = temp_ + player_.Get_Name() + ", ";
            }
        }

        msg_ += temp_;
        msg_ += "\nDead Enemies: ";
        temp_ = "";
        foreach(CharacterStats enemy_ in _enemiesList)
        {
            if(!enemy_.Is_Alive())
            {
                temp_ = temp_ + enemy_.Get_Name() + ", ";
            }
        }
        msg_ += temp_;
        return msg_;
    }
}
