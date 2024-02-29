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
    private int _typedCount = 0;
    private string _dispTypedLine = string.Empty;

    private int _mistakeCount = 0;
    private bool _isLocked = false;

    private string[] _availableLines = { "you can type this line", "you can also type this", "you even can type this" };
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
        typedLineOutput.text = _dispTypedLine;
    }

    private void Display_Available_Lines()
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
    //When letter is typed, addes letter to typed Line if correct, or adds mistake otherwise. Will not add mistake if on lockout
    {
        typedLetter_ = typedLetter_.ToLower();
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
        {
            Add_Mistake();
            for (int i = 0; i < _availableLines.Length; i++)
                _isActiveAvailableLines[i] = false;
            Enter_Letter(typedLetter_);
        }
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
            for (int i = 0; i < _isActiveAvailableLines.Length; i++)
                _isActiveAvailableLines[i] = true;

        }
        else
        {
            _typedLine += typedLetter_;
            _dispTypedLine += typedLetter_;
        }

        Display_Typed_Line();
        Display_Available_Lines();
    }

    private void Back_Space()
    {
        _typedCount--;
        if (_typedCount >= 0)
            _typedLine = _typedLine.Remove(_typedLine.Length - 1);
        else
        { 
            _typedCount++;
            return;
        }

        for (int i = 0; i < _availableLines.Length; i++)
        {
            if (_typedLine == _availableLines[i].Remove(_typedCount, _availableLines[i].Length - _typedCount))
                _isActiveAvailableLines[i] = true;
        }

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
                 break;
            }
        }
        
        Display_Typed_Line();
        Display_Available_Lines();
    }

    IEnumerator Lock_Mistake()
    //Locks out player mistakes for lockoutTime seconds
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

    private char Last_Char(string line)
    {
        return line[line.Length - 1];
    }
}