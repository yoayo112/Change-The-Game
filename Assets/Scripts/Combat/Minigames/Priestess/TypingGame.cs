/*
Project: Change the Game
File: TypingGame.cs
Date Created: Feburary 26, 2024
Author(s): Sean Thornton, Sky Vercauteren
Info:

Script that controls the Priestess' typing minigame.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingGame : MinigameBase
{
    public const int NUM_AVAILABLE_LINES = 3;
    public const int MAX_GREEN_CHAR = 20;
    public const int AVAILABLE_LINES_WIDTH = 50;

    public InkSplatter inkSplatter;

    //-------------------------------------------------------------------------------------
    //  Output
    //-------------------------------------------------------------------------------------
    [Header("Output")]
    public TextMeshProUGUI typedLineOutput = null;
    public TextMeshProUGUI[] availableLineOutputs = new TextMeshProUGUI[NUM_AVAILABLE_LINES];
    public Text mistakesOutput = null;
    public CameraShake cameraShake;

    //-------------------------------------------------------------------------------------
    //  Settings
    //-------------------------------------------------------------------------------------
    [Header("Game Settings")]
    public float lockoutTime = 0.5f;

    //-------------------------------------------------------------------------------------
    //  Variables
    //-------------------------------------------------------------------------------------
    private string _typedLine = string.Empty; //Represents the player's actual typed line, removing errors they have backspaced. 
    private int _typedCount = 0;        //Running count of characters in _typedLine.
    private int _totalTypedCount = 0;
    private string _dispTypedLine = string.Empty;   //The line displayed on the left as player input. Includes backspaced errors. Backspaced letters are represented by capitals.

    private int _mistakeCount = 0;            //Running count of player mistakes. 
    private bool _isLocked = false;           //Will not accept additional mistake inputs while true.

    //private bool[] _isActiveAvailableLines; // = { true, true, true };      //The line at _availableLines[i] is actively being typed by the player if _isActiveAvailableLines[i] == true;

    private TextTree _currentBranch;
    private int _correctCount = 0;
    private string _currentSpell = string.Empty;
    private int _finalSpellType = 0;

    //-------------------------------------------------------------------------------------
    // Character Type Declaration
    //-------------------------------------------------------------------------------------

    private const PlayerCharacterType _myCharacter = PlayerCharacterType.priestess;


    //-------------------------------------------------------------------------------------
    //  Unity Methods
    //-------------------------------------------------------------------------------------

    void OnEnable()
    {
        MinigameEventManager.onStart += Start_Minigame;
        MinigameEventManager.onUpdateTimer += Update_Timer;
        MinigameEventManager.onTimeOver += Time_Over;
    }
    void OnDisable()
    {
        MinigameEventManager.onStart -= Start_Minigame;
        MinigameEventManager.onUpdateTimer -= Update_Timer;
        MinigameEventManager.onTimeOver -= Time_Over;
    }


    private void Update()
    {
        Check_Input();
    }

    public void Start_Minigame(PlayerCharacterType whichCharacter_)
    {
        if(whichCharacter_ == _myCharacter)
        {
            if (!_isRunning)
            {
                _currentBranch = TextTree.Build(@"Assets\Scripts\Combat\Minigames\Priestess\Spells.txt");
                Update_Available_Lines();
                Update_Typed_Line();
                Update_Mistake_Counter();
                _effectiveness = 0f;
                _isRunning = true;
            }
        }
    }

    //ok attempting to reset seans code :3
    public void reset()
    {
        _typedLine = string.Empty;
        _typedCount = 0;
        _totalTypedCount = 0;
        _dispTypedLine = string.Empty;
        _mistakeCount = 0;
        _isLocked = false;
        _currentBranch = TextTree.Build(@"Assets\Scripts\Combat\Minigames\Priestess\Spells.txt");
        _correctCount = 0;
        _currentSpell = string.Empty;
        _finalSpellType = 0;
        Update_Available_Lines();
        Update_Typed_Line();
        Update_Mistake_Counter();
        _effectiveness = 0f;

    }

    public void Update_Timer(PlayerCharacterType whichCharacter_, int seconds_)
    {
        //lol I am not gonna fuck with this since you made this game :p user experience is all you!

        //but if you wanted to implement some sort of timer that is displayed to the user,
        // this method is constantly being updated with the seconds left in the game
        // you can do stuff with seconds_ here if you want.

        //Also, Elijah built a 3 (or 5?) second countdown from the initial display of the game to the game actually starting. 
        
        //SO: this methis is used twice, each used and updated by both timers, So you can use these seconds as "countdown until start" AND "countdown until end".
        //I believe lol. 

        if( whichCharacter_ == _myCharacter)
        {
            // do timer shit.
        }
    }

    public void Time_Over(PlayerCharacterType whichCharacter_)
    {
        if( whichCharacter_ == _myCharacter)
        {
            // this is the end state of the game. for now:

            //calculate effectiveness
            float _denominator = (_currentSpell.Length == 0)? 1 : _currentSpell.Length;
            float _numorator = _correctCount;
            _effectiveness = _numorator / _denominator;

            //then modify the effectiveness based on how many splits the player passed (x0, x1, x2+).
            //count the number of splits/branches from the root.
            int branches_ = _currentBranch.Count_Branches();
            int modVal_ = 0;
            if (branches_ == 0 && _correctCount == 0)
                modVal_ = 0;
            else if (branches_ == 0 && _correctCount >= 1)
                modVal_ = 1;
            else if (branches_ >= 1 && _correctCount >= 1)
                modVal_ = branches_ + 1;

            //set the games end conditions.
            _isRunning = false; //the condition being checked by the overworld to see if it's done.
            //Also, board needs to be reset in case we want to play it again next turn
            reset();
        }
    
    }

    //-------------------------------------------------------------------------------------
    //  Display Methods
    //-------------------------------------------------------------------------------------

    private void Update_Typed_Line()
    //Updates typedLineOutput with _typedLine
    {
        typedLineOutput.text = _dispTypedLine;
    }

    private void Update_Available_Lines()
    //Updates availableLineOutputs with available lines, and progress on them.
    {
        string dispCurrentLetter_;
        string dispRemainingLine_;
        string dispNextLine_;
        string dispGreenLine_;

        string lastLine_;

        Clear_Available_Lines();
        
        int i = 0;   //Index in foreach loop. 

        foreach (TextTree branch_ in _currentBranch.branches)
        {
            if (i == NUM_AVAILABLE_LINES)    //Will only iterate as long as there are available lines.
                break;

            dispCurrentLetter_ = string.Empty;
            dispRemainingLine_ = string.Empty;
            dispNextLine_ = string.Empty;
            dispGreenLine_ = string.Empty;


            dispNextLine_ = branch_.Get_Text_To_End();

            lastLine_ = branch_.Get_Text_Upto_Branch();

            int maxRemove_ = Math.Max(0, branch_.text.Length + dispNextLine_.Length + lastLine_.Length - AVAILABLE_LINES_WIDTH);  //Maximum characters to remove from the left.
                                                                                                                    //Total length of line - number of characters shown on available line.
            if (branch_.isAlive)
                dispGreenLine_ = lastLine_ + _typedLine;

            else
                dispGreenLine_ = lastLine_ + branch_.text;

            if (_totalTypedCount > MAX_GREEN_CHAR)   //Begin removing letters from the left when the length of the typed line is more than the max number of green characters displayed.
            {
                int remove_ = Math.Min(_totalTypedCount - MAX_GREEN_CHAR, maxRemove_);    //Only remove up to maxRemove_ characters.
                dispGreenLine_ = dispGreenLine_.Remove(0, remove_);
            }

            if (branch_.isAlive)
            {
                dispCurrentLetter_ = Space_To_Underscore(branch_.text[_typedCount]);
                dispRemainingLine_ = branch_.text.Remove(0, _typedCount + 1);

                availableLineOutputs[i].text += "<color=green>" + dispGreenLine_ + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + dispRemainingLine_ + dispNextLine_ + "\n";
            }
            else
                availableLineOutputs[i].text += "<color=grey>" + dispGreenLine_ + dispNextLine_ + "</color>\n";

            i++;
        }
    }

    private void Update_Mistake_Counter()
    //Updates mistake counter
    {
        mistakesOutput.text = "Mistakes: " + _mistakeCount;
    }

    //-------------------------------------------------------------------------------------
    //  Basic Game Methods
    //-------------------------------------------------------------------------------------

    private void Check_Input()
    //Pulls the player input if it is a single key press and calls Enter_Letter. Calls Back_Space if backspace is pressed
    {
        if(_isRunning) //this still seems to be running in the background and collecting input during the shooting game?
        {
            if (Input.anyKeyDown)
            {
                string keysPressed_ = Input.inputString;

                if (keysPressed_ == "\b")
                    Back_Space();
                else if (keysPressed_ == "q")
                    inkSplatter.Splat(10);
                else if (keysPressed_.Length == 1)
                    Attempt_Letter(keysPressed_);
            }
        }
    }

    private void Attempt_Letter(string typedLetter_)
    //When letter is typed, adds letter to typed line, checks if it is a mistake, locks out mistakes if it is.
    {
        typedLetter_ = typedLetter_.ToLower();

        if (Is_Correct_Letter(typedLetter_))
        {
            Deactivate_Invalid_Lines(typedLetter_);
            Enter_Letter(typedLetter_);
        }
        else if (!_isLocked)
        {
            Add_Mistake();
            Deactivate_All_Lines();
            Enter_Letter(typedLetter_);
        }
        else return;    //Do nothing. No need to Check if line is complete.

        Check_Complete();
        Update_Typed_Line();
        Update_Available_Lines();
    }

    private void Enter_Letter(string typedLetter_)
    //Adds typed letter to _typedLine, gets new _currentLetter from _remainingLine, and removes letter from _remainingLine
    {
        _typedCount++;
        _totalTypedCount++;

        _typedLine += typedLetter_;
        _dispTypedLine += typedLetter_;
    }

    private void Add_Mistake()
    //Increments mistake counter, plays sound, and locks player input
    {
        _mistakeCount++;
        cameraShake.Shake();
        Update_Mistake_Counter();
        
        //THIS IS CURRENTLY THE RELOAD SOUND IN THE OTHER MINIGAME.
        //KEEP THIS COMMENTED UNTIL WE HAVE AUDIO MANAGEMENT OTHERWISE ALL KEY PRESS CAUSE "AUUUGHH?"

        //audioSource.PlayOneShot(audioClipArray[0], volume);

        //TODO: Audio Management Object.
        
        StartCoroutine(Lock_Mistake());

        //inkSplatter.Splat(10);
    }

    private void Back_Space()
    {
        if (_typedCount == 0)
            return;

        _typedCount--;
        _totalTypedCount--;
        _typedLine = _typedLine.Remove(_typedLine.Length - 1);

        Activate_Valid_Lines();
        Remove_Last_Typed_Letter();
        Update_Typed_Line();
        Update_Available_Lines();
    }

    private void Remove_Last_Typed_Letter()
    {
        char testChar_;
        string newChar_ = string.Empty;
        for (int i = _dispTypedLine.Length - 1; i >= 0; i--)
        {
            bool done_ = false;
            testChar_ = _dispTypedLine[i];
            if (char.IsWhiteSpace(testChar_))
            {
                newChar_ = "_";
                done_ = true;
            }
            else if (char.IsLower(testChar_))
            {
                testChar_ = char.ToUpper(testChar_);
                newChar_ = char.ToString(testChar_);
                done_ = true;
            }
            if (done_)
            {
                _dispTypedLine = _dispTypedLine.Remove(i, 1);
                _dispTypedLine = _dispTypedLine.Insert(i, newChar_);
                return;
            }
        }
    }

    private void Check_Complete()
    {
        foreach (TextTree branch_ in _currentBranch.branches)
        {
            if (branch_.isAlive && branch_.text == _typedLine)
            {
                _typedLine = string.Empty;
                _typedCount = 0;
                _currentBranch = branch_;
                return;
            }
        }
    }

    private void Activate_All_Lines()
    {
        foreach (TextTree branch_ in _currentBranch.branches)
            branch_.Revive();
    }

    private void Activate_Valid_Lines()
    {
        string branchText_;
        foreach (TextTree branch_ in _currentBranch.branches)
        {
            branchText_ = branch_.text;
            if (branchText_.Length < _typedCount)
                continue;
            if (_typedLine == branchText_.Remove(_typedCount, branchText_.Length - _typedCount))
                branch_.Revive();
        }
    }

    private void Deactivate_All_Lines()
    {
        foreach (TextTree branch_ in _currentBranch.branches)
            branch_.Kill();
    }

    private void Deactivate_Invalid_Lines(string typedLetter_)
    {
        string branchText_;
        foreach (TextTree branch_ in _currentBranch.branches)
        {
            branchText_ = branch_.text;
            if (!Is_Correct_Letter(typedLetter_, branchText_))
                branch_.Kill();
        }
    }

    private bool Is_Correct_Letter(string letter_, string line_)
    {
        if (_typedCount >= line_.Length)
            return false;
        return char.ToLower(letter_[0]) == line_[_typedCount];
    }

    private bool Is_Correct_Letter(string letter_)
    {
        foreach (TextTree branch_ in _currentBranch.branches)
        {
            if (Is_Correct_Letter(letter_, branch_.text) && branch_.isAlive)
            {
                _currentSpell = branch_.Get_Full_Text(); // Do we know this is the buest guess at the attempted spell?
                _correctCount++;
                return true;
            }
        }

        return false;
    }

    private void Clear_Available_Lines() 
    {
        for (int i = 0; i < availableLineOutputs.Length; i++)
            availableLineOutputs[i].text = string.Empty;
    }

    private string Space_To_Underscore(char character)
    {
        if (char.IsWhiteSpace(character))
            return "_";
        else
            return char.ToString(character);
    }

    private char Last_Char(string line)
    {
        return line[line.Length - 1];
    }

    //-------------------------------------------------------------------------------------
    //  Coroutines
    //-------------------------------------------------------------------------------------

    IEnumerator Lock_Mistake()
    //Locks out player mistakes for lockoutTime seconds
    {
        _isLocked = true;
        yield return new WaitForSeconds(lockoutTime);
        _isLocked = false;
    }

    //-------------------------------------------------------------------------------------
    // Helper Functions
    //-------------------------------------------------------------------------------------
    override public int Get_Action_Type()
    //checks the diety name in order to dertermine the type of magic to be used
    //0 - for health spells
    //1 - for dps spells
    //3 - for buff/debuff spells
    {
        //TODO: update these deities with real gods!
        string _fullSpell = _currentBranch.Get_Full_Text().ToLower();
        if (_fullSpell.Contains("healy dan"))
        {
            Debug.Log("Casting a Heal Type Spell");
            return 1;
        }
        else if (_fullSpell.Contains("ghost malone"))
        {
            Debug.Log("Casting a DPS Type Spell");
            return 0;
        }
        else if (_fullSpell.Contains("shady haga"))
        {
            Debug.Log("Casting a Buff Type Spell");
            return 2;
        }
        return 0;
    }
}