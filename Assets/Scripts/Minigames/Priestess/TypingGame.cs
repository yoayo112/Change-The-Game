using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingGame : MonoBehaviour
{
    // Word bank needed
    public Text wordOutput = null;

    private string typedWord = string.Empty;
    private string remainingWord = string.Empty;
    private string currentWord = "this is a longer test.";





    private void Start()
    {
        SetCurrentWord();
    }

    private void SetCurrentWord()
    {
        // Get word from bank
        typedWord = string.Empty;
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newString)
    {
        remainingWord = newString;
        DisplayWord();
    }



    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if(Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if (keysPressed.Length == 1)
                EnterLetter(keysPressed);
        }
    }

    private void EnterLetter(string typedLetter)
    {
        if(IsCorrectLetter(typedLetter))
        {
            typedWord += typedLetter;
            RemoveLetter();
            DisplayWord();

            if (isWordComplete())
                SetCurrentWord();

        }
    }

    private bool IsCorrectLetter(string letter)
    {
        return remainingWord.IndexOf(letter) == 0;
    }



    private bool isWordComplete()
    {
        return remainingWord.Length == 0;
    }

    private void DisplayWord()
    {
        wordOutput.text = "<color=green>" + typedWord + "</color>" + remainingWord;
    }

    private void RemoveLetter()
    {
        string newString = remainingWord.Remove(0, 1);
        SetRemainingWord(newString);
    }
}
