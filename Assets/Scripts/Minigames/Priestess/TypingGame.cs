/*
Project: Change the Game
File: TypingGame.cs
Date Created: Feburary 26, 2024
Author(s): Sean Thornton
Info:

Script that controls the Priestess' typing minigame.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingGame : MonoBehaviour
{
    //-------------------------------------------------------------------------------------
    //  Output
    //-------------------------------------------------------------------------------------
    [Header("Output")]
    public TextMeshProUGUI typedLineOutput = null;
    public TextMeshProUGUI availableLineOutput = null;
    public Text mistakesOutput = null;
    public CameraShake cameraShake;

    //-------------------------------------------------------------------------------------
    //  Settings
    //-------------------------------------------------------------------------------------
    [Header("Game Settings")]
    public float lockoutTime = 0.5f;
    public float shakeMagnitude = 1f;
    public float shakeDuration = 1f;

    //-------------------------------------------------------------------------------------
    //  Audio handling
    //-------------------------------------------------------------------------------------
    [Header("Audio Members and Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public float volume = 0.5f;

    //-------------------------------------------------------------------------------------
    //  Variables
    //-------------------------------------------------------------------------------------
    private string _typedLine = string.Empty; //Represents the player's actual typed line, removing errors they have backspaced. 
    private int _typedCount = 0;              //Running count of characters in _typedLine.
    private string _dispTypedLine = string.Empty;   //The line displayed on the left as player input. Includes backspaced errors. Backspaced letters are represented by capitals.

    private int _mistakeCount = 0;            //Running count of player mistakes. 
    private bool _isLocked = false;           //Will not accept additional mistake inputs while true.

    private string[] _availableLines = { "you can type this line", "you can also type this", "you even can type this" };    //Line displayed on the right as lines available to type.
    private bool[] _isActiveAvailableLines = { true, true, true };      //The line at _availableLines[i] is actively being typed by the player if _isActiveAvailableLines[i] == true;

    private TextTree currentTextTree_;

    //-------------------------------------------------------------------------------------
    //  Unity Methods
    //-------------------------------------------------------------------------------------

    private void Update()
    {
        Check_Input();
    }

    private void Start()
    {
        Update_Available_Lines();
        Update_Typed_Line();
        Update_Mistake_Counter();
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
    //Updates availableLineOutput with available lines, and progress on them.
    {
        string dispCurrentLetter_ = string.Empty;
        string dispRemainingLine_ = string.Empty;

        availableLineOutput.text = string.Empty;
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (_isActiveAvailableLines[i])
            {
                dispCurrentLetter_ = Space_To_Underscore(_availableLines[i][_typedCount]);
                dispRemainingLine_ = _availableLines[i].Remove(0, _typedCount + 1);
                availableLineOutput.text += "<color=green>" + _typedLine + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + dispRemainingLine_ + "\n";
            }
            else
                availableLineOutput.text += "<color=grey>" + _availableLines[i] + "</color>\n";
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
        if (Input.anyKeyDown)
        {
            string keysPressed_ = Input.inputString;

            if (keysPressed_ == "\b")
                Back_Space();
            else if (keysPressed_.Length == 1)
                Attempt_Letter(keysPressed_);
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
        //else do nothing
    }

    private void Enter_Letter(string typedLetter_)
    //Adds typed letter to _typedLine, gets new _currentLetter from _remainingLine, and removes letter from _remainingLine
    {
        _typedCount++;
        if (_typedCount == _availableLines[0].Length) //Reset for testing
        {
            _typedCount = 0;
            _typedLine = string.Empty;
            _dispTypedLine += typedLetter_ + "\n";
            Activate_All_Lines();
        }
        else
        {
            _typedLine += typedLetter_;
            _dispTypedLine += typedLetter_;
        }

        Update_Typed_Line();
        Update_Available_Lines();
    }

    private void Add_Mistake()
    //Increments mistake counter, plays sound, and locks player input
    {
        _mistakeCount++;
        StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        Update_Mistake_Counter();
        audioSource.PlayOneShot(audioClipArray[0], volume);
        StartCoroutine(Lock_Mistake());
    }

    private void Back_Space()
    {
        if (_typedCount == 0)
            return;

        _typedCount--;
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

    private void Activate_All_Lines()
    {
        for (int i = 0; i < _availableLines.Length; i++)
            _isActiveAvailableLines[i] = true;
    }

    private void Activate_Valid_Lines()
    {
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (_typedLine == _availableLines[i].Remove(_typedCount, _availableLines[i].Length - _typedCount))
                _isActiveAvailableLines[i] = true;
        }
    }

    private void Deactivate_All_Lines()
    {
        for (int i = 0; i < _availableLines.Length; i++)
            _isActiveAvailableLines[i] = false;
    }

    private void Deactivate_Invalid_Lines(string typedLetter_)
    {
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (!Is_Correct_Letter(typedLetter_, _availableLines[i]))
                _isActiveAvailableLines[i] = false;
        }
    }

    //-------------------------------------------------------------------------------------
    //  Accessors
    //-------------------------------------------------------------------------------------

    private bool Is_Correct_Letter(string letter_, string line_)
    {
        return char.ToLower(letter_[0]) == line_[_typedCount];
    }

    private bool Is_Correct_Letter(string letter_)
    {
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (Is_Correct_Letter(letter_, _availableLines[i]) && _isActiveAvailableLines[i])
                return true;
        }

        return false;
    }

    //-------------------------------------------------------------------------------------
    //  Functions
    //-------------------------------------------------------------------------------------

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
}