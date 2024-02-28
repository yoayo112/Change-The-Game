﻿using System.Collections;
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

    //-------------------------------------------------------------------------------------
    //  Settings
    //-------------------------------------------------------------------------------------
    [Header("Game Settings")]
    public float lockoutTime = 1f;

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
    private string _typedLine = string.Empty;
    private string _previousLines = "";
    private int _typedCount = 0;

    private int _mistakeCount = 0;
    private bool _isLocked = false;

    private string[] _availableLines = { "you can type this line.", "you can also type this.", "you even can type this." };
    private bool[] _isActiveAvailableLines = { true, true, true };

    private void Update()
    {
        Check_Input();
    }

    private void Start()
    {
        Display_Available_Lines();
        Display_Typed_Line();
        Update_Mistake_Display();
    }

    //-------------------------------------------------------------------------------------
    //  Mutators
    //-------------------------------------------------------------------------------------
 

    private void Display_Typed_Line()
    //Updates typedLineOutput with _typedLine
    {
        string dispTypedLine_ = _typedLine;

        typedLineOutput.text = dispTypedLine_;
    }

    private void Display_Available_Lines()
        //Updates availableLineOutput with available lines, and progress on them.
    {
        string dispCurrentLetter_ = string.Empty;
        string dispRemainingLine_ = string.Empty;
        string dispTypedLine_ = _typedLine;

        availableLineOutput.text = string.Empty;
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (_isActiveAvailableLines[i])
            {
                dispCurrentLetter_ = Space_To_Underscore(_availableLines[i][_typedCount]);
                dispRemainingLine_ = _availableLines[i].Remove(0, _typedCount + 1);
                availableLineOutput.text += "<color=green>" + dispTypedLine_ + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + dispRemainingLine_ + "\n";
            }
            else
                availableLineOutput.text += "<color=grey>" + _availableLines[i] + "</color>\n";
        }
    }

    private void Add_Mistake()
        //Increments mistake counter, plays sound, and locks player input
    {
        _mistakeCount++;
        //Camera Shake
        Update_Mistake_Display();
        audioSource.PlayOneShot(audioClipArray[0], volume);
        StartCoroutine(Lock_Mistake());
    }

    private void Update_Mistake_Display()
        //Updates mistake counter
    {
        mistakesOutput.text = "Mistakes: " + _mistakeCount;
    }

    private void Check_Input()
        //Pulls the player input if it is a single key press and calls Enter_Letter
    {
        if(Input.anyKeyDown)
        {
            string keysPressed_ = Input.inputString;

            if (keysPressed_.Length == 1)
                Attempt_Letter(keysPressed_);
        }
    }

    private void Attempt_Letter(string typedLetter_)
        //When letter is typed, addes letter to typed Line if correct, or adds mistake otherwise. Will not add mistake if on lockout
    {
        bool isCorrect_ = false;
        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (Is_Correct_Letter(typedLetter_, _availableLines[i]) && _isActiveAvailableLines[i])
                isCorrect_ = true;
        }

        if (isCorrect_)
        {
            for (int i = 0; i < _availableLines.Length; i++)
            {
                if (!Is_Correct_Letter(typedLetter_, _availableLines[i]))
                    _isActiveAvailableLines[i] = false;
            }
            Enter_Letter(typedLetter_);
        }
        else if (!_isLocked)
            Add_Mistake();
    }

    private void Enter_Letter(string typedLetter_)
    //Adds typed letter to _typedLine, gets new _currentLetter from _remainingLine, and removes letter from _remainingLine
    {
        _typedCount++;
        if (_typedCount == _availableLines[0].Length) //Reset for testing
        {
            _typedCount = 0;
            _typedLine = string.Empty;
            for (int i = 0; i < _isActiveAvailableLines.Length; i++)
                _isActiveAvailableLines[i] = true;

        }
        else
            _typedLine += typedLetter_;

        Display_Typed_Line();
        Display_Available_Lines();



    }

    IEnumerator Lock_Mistake()
        //Locks out player input for lockoutTime seconds
    {
        _isLocked = true;
        yield return new WaitForSeconds(lockoutTime);
        _isLocked = false;
    }

    //-------------------------------------------------------------------------------------
    //  Accessors
    //-------------------------------------------------------------------------------------

    private bool Is_Correct_Letter(string letter_, string line_)
    {
        return char.ToLower(letter_[0]) == line_[_typedCount];
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


}

