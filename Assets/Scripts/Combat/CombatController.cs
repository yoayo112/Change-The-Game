/*
Project: Change the Game
File: CombatController.cs
Date Created: March 01, 2024
Author(s): Elijah Theander, Sean Thornton, Sky Vercauteren
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
    public enum CombatStates
    {
        beginCombat,
        startTurn,
        endTurn,
        endCombat,
        waiting
    }

    public static CombatController instance { get; private set; } //Singleton

    public static List<Character> enemies { get; set; } //List holding the scripts of the enemies
    public static List<Character> players { get; set; } //List holding the scripts of the players

    private static List<Character> _turnQueue = new List<Character>(); // Total list managing turn order

    public static int turnNumber { get; private set; } // Current turn in this combat scenario
    private static int _currentTurnPos = 0; // Current turn Queue index

    private bool _playerVictory = false; // Only used for determining ending state of combat

    public CombatStates currentState; //Keeps track of current combat state
    private bool _startedFighting = false;

    [Header("Use this to handle combat starting and ending")]
    public GameObject terminal;

    private void Awake()
    {

        //Sets CombatController as a singleton with highlander rules
        if (instance != null && instance != this)
        {
            Destroy(this);
            Debug.Log("There can be only one CombatController instance. Killing duplicate.");
        }
        else
            instance = this;
    }

    private void OnEnable()
    {
        CombatEventManager.onEndTurn += End_Turn_State;
    }
    private void OnDisable()
    {
        CombatEventManager.onEndTurn -= End_Turn_State;
    }

    public void Start_Combat()
    {
        //Character[] allCharacters = GameObject.FindObjectsByType<Character>(FindObjectsSortMode.None);
        _turnQueue = new List<Character>(players.Union(enemies));
        turnNumber = 0;

        int playerCount_ = 0;
        int enemyCount_ = 0;


        //sort and populate turn Queue
        foreach (Character character in _turnQueue)
        {
            if (character.Get_CharacterType() == CharacterType.player)
            {
                character.Set_Position(playerCount_);
                playerCount_++;

            }
            if (character.Get_CharacterType() == CharacterType.enemy)
            {
                character.Set_Position(enemyCount_);
                enemyCount_++;
            }
        }

        Update_Turn_Queue();
        StartCoroutine(Initial_Combat_Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        //Checking State
        switch (currentState)
        {
            case CombatStates.beginCombat:
                if (_startedFighting) { Start_Turn(); }
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
        turnNumber++;

        print(
                      "-----------------------------------------------------\n"
                    + "Turn Number: " + turnNumber + " \n"
                    + "Current Queue Position: " + _currentTurnPos + " \n"
                    + "Current character: " + _turnQueue[_currentTurnPos].Get_Name() + " \n"
                    + "-----------------------------------------------------"
                );
        if (_turnQueue[_currentTurnPos].Is_Alive())
        {
            CombatEventManager.Start_Turn(_currentTurnPos);
        }
        else
        {
            currentState = CombatStates.endTurn;
            Debug.Log("Character is dead - skipping.");
        }
    }

    public void End_Turn()
    {
        Debug.Log("Ending Turn");
        Debug.Log(Print_Dead_Characters());

        currentState = CombatStates.waiting; //Tell State Machine to wait.

        //Pick who's going next turn.
        if (_currentTurnPos == _turnQueue.Count - 1)
        {
            _currentTurnPos = 0;
            Update_Turn_Queue();
        }
        else
        {
            _currentTurnPos++;
        }

        //Check for game over condition
        if (Is_Combat_Running())
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
        foreach (Character enemy_ in enemies)
        {
            if (!enemy_.Is_Alive())
                deadEnemies_++;
        }

        int deadPlayers_ = 0;
        foreach (Character player_ in players)
        {
            if (!player_.Is_Alive())
                deadPlayers_++;
        }

        //returning false here tells the game that combat is over.
        if (deadPlayers_ == players.Count)
        {
            _playerVictory = false;
            return false;
        }

        if (deadEnemies_ == enemies.Count)
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
        terminal.GetComponentInChildren<CombatTerminal>().End_Combat(victoryMessage_);
    }

    //Make a big string of who's dead on both sides.
    public string Print_Dead_Characters()
    {
        string msg_ = "\nDead Players: ";
        string temp_ = "";
        foreach (Character player_ in players)
        {
            if (!player_.Is_Alive())
            {
                temp_ = temp_ + player_.Get_Name() + ", ";
            }
        }

        msg_ += temp_;
        msg_ += "\nDead Enemies: ";
        temp_ = "";
        foreach (Character enemy_ in enemies)
        {
            if (!enemy_.Is_Alive())
            {
                temp_ = temp_ + enemy_.Get_Name() + ", ";
            }
        }
        msg_ += temp_;

        return msg_;

    }

    public void Update_Turn_Queue()
    {
        _turnQueue.Sort();
        for (int i = 0; i < _turnQueue.Count; i++)
        {
            _turnQueue[i].Set_QueuePosition(i);
            Debug.Log("Turn Queue indexes: " + _turnQueue[i].Get_Name() + " is at position: " + _turnQueue[i].Get_QueuePosition());
        }
    }

    public IEnumerator Initial_Combat_Countdown()
    {
        yield return terminal.GetComponentInChildren<CombatTerminal>().countdown();
        _startedFighting = true;
    }
}
