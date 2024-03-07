﻿/*
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
    public InkSplatter inkSplatter;

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
    private int _typedCount = 0;        //Running count of characters in _typedLine.
    private string _dispTypedLine = string.Empty;   //The line displayed on the left as player input. Includes backspaced errors. Backspaced letters are represented by capitals.

    private int _mistakeCount = 0;            //Running count of player mistakes. 
    private bool _isLocked = false;           //Will not accept additional mistake inputs while true.

    private string[] _availableLines = { "Healy Dan, grant divine healing to my wounded body",
                                         "Healy Dan, grant divine healing for my bloodied brethren",
                                         "Healy Dan, bestow powerful and awesome divine healing to my mortally wounded vessel",
                                         "Healy Dan, bestow powerful and awesome divine healing for my bloodied brethren",
                                         "Healy Dan, bestow powerful and awesome zealous might to your humble servant",
                                         "Healy Dan, bestow powerful and awesome zealous might for this glorious holy army",
                                         "Ghost Malone, strike down this heathen",
                                         "Ghost Malone, striketh down with ultimate might on the unbeliever",
                                         "Ghost Malone, strike down these forsaken infidels",
                                         "Ghost Malone, strengthen my body to spill your enemies blood",
                                         "Shady Haga, do some wicked ass cool shit that I can't think of",
                                         "Shady Haga, do some wicked stupid dumb shit I dunno",
                                         "Shady Haga, do some wicked ass cool shit i guess jesus",
                                         "Shady Haga, do some fine things for me"};

    //private bool[] _isActiveAvailableLines; // = { true, true, true };      //The line at _availableLines[i] is actively being typed by the player if _isActiveAvailableLines[i] == true;

    private TextTree _currentBranch;


    //-------------------------------------------------------------------------------------
    //  Unity Methods
    //-------------------------------------------------------------------------------------

    private void Update()
    {
        Check_Input();
    }

    private void Start()
    {
        _currentBranch = new TextTree(_availableLines);
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
        string dispCurrentLetter_;
        string dispRemainingLine_;
        string dispNextLine_;
        string dispLastLine_;

        string branchText_;

        availableLineOutput.text = string.Empty;

        foreach (TextTree branch_ in _currentBranch.branches)
        {
            dispCurrentLetter_ = string.Empty;
            dispRemainingLine_ = string.Empty;
            dispNextLine_ = string.Empty;
            dispLastLine_ = string.Empty;

            branchText_ = branch_.text;

            if (branch_.branches.Count > 0)
            {
                dispNextLine_ = branch_.branches[0].text;

                if (branch_.branches[0].branches.Count > 0)
                    dispNextLine_ += " . . . ";
            }


            if (_currentBranch.root != null && _currentBranch.root.root != null)
                dispLastLine_ = " . . . ";
             dispLastLine_ += _currentBranch.text;

            if (branch_.alive)
            {
                dispCurrentLetter_ = Space_To_Underscore(branchText_[_typedCount]);
                dispRemainingLine_ = branchText_.Remove(0, _typedCount + 1);

                availableLineOutput.text += "<color=green>" + dispLastLine_ + _typedLine + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + dispRemainingLine_ + dispNextLine_ + "\n";
            }
            else
                availableLineOutput.text += "<color=grey>" + dispLastLine_ + branchText_ + dispNextLine_ + "</color>\n";
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
        else return;    //Do nothing. No need to Check if line is complete.

        Check_Complete();
        Update_Typed_Line();
        Update_Available_Lines();
    }

    private void Enter_Letter(string typedLetter_)
    //Adds typed letter to _typedLine, gets new _currentLetter from _remainingLine, and removes letter from _remainingLine
    {
        _typedCount++;

        _typedLine += typedLetter_;
        _dispTypedLine += typedLetter_;
    }

    private void Add_Mistake()
    //Increments mistake counter, plays sound, and locks player input
    {
        _mistakeCount++;
        cameraShake.Shake();
        Update_Mistake_Counter();
        audioSource.PlayOneShot(audioClipArray[0], volume);
        StartCoroutine(Lock_Mistake());

        inkSplatter.Splat();
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

    private void Check_Complete()
    {
        foreach (TextTree branch_ in _currentBranch.branches)
        {
            if (branch_.Is_Alive() && branch_.text == _typedLine)
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
            if (Is_Correct_Letter(letter_, branch_.text) && branch_.Is_Alive())
                return true;
        }

        return false;
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
}