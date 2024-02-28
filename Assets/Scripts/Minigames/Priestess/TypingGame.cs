using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingGame : MonoBehaviour
{
    // Line bank needed
    public TextMeshProUGUI LineOutput = null;
    public Text mistakesOutput = null;
    public float lockoutTime = 1f;

    //-------------------------------------------------------------------------------------
    //  Audio handling
    //-------------------------------------------------------------------------------------
    [Header("Audio Members and Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public float volume = 0.5f;

    private string _typedLine = string.Empty;
    private char _currentLetter;
    private string _remainingLine = string.Empty;
    private string _currentLine = "should be 28 chars or less";
    private string _previousLines = "";
    private bool _isLineComplete = false;
    private int _mistakes = 0;
    private bool _isLocked = false;

    private void Update()
    {
        Check_Input();
    }

    private void Start()
    {
        Set_Current_Line();
        Update_Mistake_Display();
    }

    //-------------------------------------------------------------------------------------
    //  Mutators
    //-------------------------------------------------------------------------------------
    private void Set_Current_Line()
        //Sets _remainingLine and resets _typedLine and _currentLetter
    {
        _isLineComplete = false;
        _typedLine = string.Empty;
        _currentLetter = _currentLine[0];
        Set_Remaining_Line(_currentLine.Remove(0, 1));
    }

    private void Set_Remaining_Line(string newString_)
        //Sets _remainingLine to given string
    {
        _remainingLine = newString_;
        Display_Line();
    }

    private void Display_Line()
        //Updates LineOutput with _typedLine, _currentLetter, and _remainingLine
    {
        string dispCurrentLetter_ = string.Empty;
        string dispRemainingLine_ = _remainingLine;
        string dispTypedLine_ = _typedLine;

        if (char.IsWhiteSpace(_currentLetter))
            dispCurrentLetter_ = "_\u200B";
        else
            dispCurrentLetter_ = char.ToString(_currentLetter);

        LineOutput.text = _previousLines + "<color=green>" + dispTypedLine_ + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + dispRemainingLine_;
    }

    

    private void Add_Mistake()
        //Increments mistake counter, plays sound, and locks player input
    {
        _mistakes++;
        //Camera Shake
        Update_Mistake_Display();
        audioSource.PlayOneShot(audioClipArray[0], volume);
        StartCoroutine(Lock_Input());

    }

    private void Update_Mistake_Display()
        //Updates mistake counter
    {
        mistakesOutput.text = "Mistakes: " + _mistakes;
    }

    private void Check_Input()
        //Pulls the player input if it is a single key press and calls Enter_Letter
    {
        if(Input.anyKeyDown)
        {
            string keysPressed_ = Input.inputString;

            if (keysPressed_.Length == 1)
                Enter_Letter(keysPressed_);
        }
    }

    private void Enter_Letter(string typedLetter_)
        //When letter is typed, addes letter to typed Line if correct, or adds mistake otherwise.
    {
        if (!_isLocked)
        {
            if (Is_Correct_Letter(typedLetter_))
            {
                Remove_Letter();
                if (_isLineComplete)
                    Next_Line();

            }
            else
                Add_Mistake();
        }
    }

    private void Remove_Letter()
    //Adds typed letter to _typedLine, gets new _currentLetter from _remainingLine, and removes letter from _remainingLine
    {
        if (_remainingLine.Length == 0)
            _isLineComplete = true;
        else
        {
            _typedLine += _currentLetter;
            _currentLetter = _remainingLine[0];
            string newString_ = _remainingLine.Remove(0, 1);
            Set_Remaining_Line(newString_);
        }
    }

    private void Next_Line()
    {
        _previousLines += "<color=green>" + _currentLine + "</color>\n";
        Set_Current_Line();
    }

    IEnumerator Lock_Input()
        //Locks out player input for lockoutTime seconds
    {
        _isLocked = true;
        yield return new WaitForSeconds(lockoutTime);
        _isLocked = false;
    }

    //-------------------------------------------------------------------------------------
    //  Accessors
    //-------------------------------------------------------------------------------------

    private bool Is_Correct_Letter(string letter_)
    {
        return _currentLetter == char.ToLower(letter_[0]);
    }

 
}

