using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingGame : MonoBehaviour
{
    // Word bank needed
    public Text wordOutput = null;
    public Text mistakesOutput = null;
    public float lockoutTime = 1f;

    //-------------------------------------------------------------------------------------
    //  Audio handling
    //-------------------------------------------------------------------------------------
    [Header("Audio Members and Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public float volume = 0.5f;

    private string _typedWord = string.Empty;
    private char _currentLetter;
    private string _remainingWord = string.Empty;
    private string _currentWord = "this is a longer test.";
    private bool _isWordComplete = false;
    private int _mistakes = 0;
    private bool _isLocked = false;

    private void Update()
    {
        CheckInput();
    }

    private void Start()
    {
        Set_Current_Word();
        Update_Mistake_Display();
    }

    private void Set_Current_Word()
    {
        // Get word from bank
        _isWordComplete = false;
        _typedWord = string.Empty;
        _currentLetter = _currentWord[0];
        Set_Remaining_Word(_currentWord.Remove(0, 1));
    }

    private void Set_Remaining_Word(string newString_)
    {
        _remainingWord = newString_;
        Display_Word();
    }

    private void Check_Input()
    {
        if(Input.anyKeyDown)
        {
            string keysPressed_ = Input.inputString;

            if (keysPressed_.Length == 1)
                Enter_Letter(keysPressed_);
        }
    }

    private void Enter_Letter(string typedLetter_)
    {
        if (!_isLocked)
        {
            if (Is_Correct_Letter(typedLetter_))
            {
                Remove_Letter();
                if (_isWordComplete)
                    Set_Current_Word();

            }
            else
                Add_Mistake();
        }
    }

    private bool Is_Correct_Letter(string letter_)
    {
        return _currentLetter == char.ToLower(letter_[0]);
    }

    private void Display_Word()
    {
        char dispCurrentLetter_;
        if (char.IsWhiteSpace(_currentLetter))
            dispCurrentLetter_ = '█';
        else
            dispCurrentLetter_ = _currentLetter;

        wordOutput.text = "<color=green>" + _typedWord + "</color><color=yellow>" + dispCurrentLetter_ + "</color>" + _remainingWord;
    }

    private void Remove_Letter()
    {
        if (_remainingWord.Length == 0)
            _isWordComplete = true;
        else
        {
            _typedWord += _currentLetter;
            Next_Letter();
            string newString = _remainingWord.Remove(0, 1);
            Set_Remaining_Word(newString);
        }
    }

    private void Next_Letter()
    {
            _currentLetter = _remainingWord[0];
    }

    private void Add_Mistake()
    {
        _mistakes++;
        //Camera Shake
        Update_Mistake_Display();
        audioSource.PlayOneShot(audioClipArray[0], volume);
        StartCoroutine(Lock_Input());

    }

    private void Update_Mistake_Display()
    {
        mistakesOutput.text = "Mistakes: " + _mistakes;
    }

    IEnumerator LockInput()
    {

        _isLocked = true;
        yield return new WaitForSeconds(lockoutTime);
        _isLocked = false;

    }
}

