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

    public static List<Character> enemies { get; private set; } //List holding the scripts of the enemies
    public static List<Character> players { get; private set; } //List holding the scripts of the players

    //Spawn Positions
    public GameObject[] partySpawns;
    public GameObject[] baddySpawns;

    private static List<Character> _turnQueue = new List<Character>(); // Total list managing turn order

    public static int turnNumber { get; private set; } // Current turn in this combat scenario
    private static int _currentTurnPos = 0; // Current turn Queue index

    private bool _playerVictory = false; // Only used for determining ending state of combat

    public CombatStates currentState = CombatStates.beginCombat; //Keeps track of current combat state

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

    void Start()
    {
        //disable the overworld camera
        GlobalService.Get_Camera().SetActive(false); //TODO change this back when combat ends!

        //Find all Player and enemy character controls, put them in a list, and sort.

        //Partry members first
        GameObject mainCharacter = GlobalService.Get_Player_Instance();
        List<GameObject> party = GlobalService.Get_Party_Instances();
        players = new List<Character>();

        //Also lets spawn, initialize and reposition the prefabs while were at it!
        mainCharacter.GetComponent<PlayerAction>().Set_Combat(true); //TODO! set combat to false when combat ends
        players.Add(mainCharacter.GetComponent<Character>());
        mainCharacter.transform.position = partySpawns[0].transform.position;
        Vector3 initialTarget = new Vector3(baddySpawns[0].transform.position.x, mainCharacter.transform.position.y, baddySpawns[0].transform.position.z);
        mainCharacter.transform.LookAt(initialTarget, Vector3.up);
        int j = 1;
        foreach(GameObject member in party)
        {
            players.Add(member.GetComponent<Character>());
            member.transform.position = partySpawns[j].transform.position;
            member.transform.LookAt(initialTarget, Vector3.up);
            member.GetComponent<PartyMovement>().Set_Combat(true); //TODO! set combat to false when combat ends
        }

        //ok now the neerdowells
        enemies = new List<Character>();
        List<string> enemyNames = GlobalService.Get_Main().Get_Enemies();
        initialTarget = new Vector3(partySpawns[0].transform.position.x, mainCharacter.transform.position.y, partySpawns[0].transform.position.z);
        for (int i = 0; i < enemyNames.Count; i++)
        {
            GameObject baddy = Instantiate(Resources.Load<GameObject>(enemyNames[i]));
            enemies.Add(baddy.GetComponent<Character>());
            baddy.transform.position = baddySpawns[i].transform.position;
            baddy.transform.LookAt(initialTarget, Vector3.up);
            baddy.GetComponent<BasicNPCMovement>().Set_Combat(true); // TODO: Set to false after combat ends
        }

        //Character[] allCharacters = GameObject.FindObjectsByType<Character>(FindObjectsSortMode.None);
        _turnQueue = new List<Character>(players.Union(enemies));
        turnNumber = 0;

        int playerCount_ = 0;
        int enemyCount_ = 0;


        //Break turnQueue into character and enemy lists
        foreach (Character character in _turnQueue)
        {
            if (character.Get_CharacterType() == CharacterType.player)
            {
                players.Add(character);
                character.Set_Position(playerCount_);
                playerCount_++;

            }
            if (character.Get_CharacterType() == CharacterType.enemy)
            {
                enemies.Add(character);
                character.Set_Position(enemyCount_);
                enemyCount_++;
            }
        }

        Update_Turn_Queue();

    }

    // Update is called once per frame
    void Update()
    {

        //Checking State
        switch (currentState)
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
        turnNumber++;

        Debug.Log(
                      "-----------------------------------------------------\n"
                    + "Turn Number: " + turnNumber + "\n"
                    + "Current Queue Position: " + _currentTurnPos + "\n"
                    + "Current character: " + _turnQueue[_currentTurnPos].Get_Name() + "\n"
                    + "-----------------------------------------------------"
                );

        CombatEventManager.Start_Turn(_currentTurnPos);
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
}
