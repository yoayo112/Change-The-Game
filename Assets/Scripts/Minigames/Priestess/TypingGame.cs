using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingGame : MonoBehaviour
{
    // Word bank needed
    public Text wordOutput = null;

    private string typedWord = string.Empty;
    private char currentLetter;
    private string remainingWord = string.Empty;
    private string currentWord = "this is a longer test.";
    private bool isWordComplete = false;

    private void Start()
    {
        SetCurrentWord();
    }

    private void SetCurrentWord()
    {
        // Get word from bank
        isWordComplete = false;
        typedWord = string.Empty;
        currentLetter = currentWord[0];
        SetRemainingWord(currentWord.Remove(0, 1));
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
            RemoveLetter();
            if (isWordComplete)
                SetCurrentWord();

        }
    }

    private bool IsCorrectLetter(string letter)
    {
        return currentLetter == letter[0];
    }

    private void DisplayWord()
    {
        char dispCurrentLetter;
        if (char.IsWhiteSpace(currentLetter))
            dispCurrentLetter = '█';
        else
            dispCurrentLetter = currentLetter;

        wordOutput.text = "<color=green>" + typedWord + "</color><color=yellow>" + dispCurrentLetter + "</color>" + remainingWord;
    }

    private void RemoveLetter()
    {
        if (remainingWord.Length == 0)
            isWordComplete = true;
        else
        {
            typedWord += currentLetter;
            GetNextLetter();
            string newString = remainingWord.Remove(0, 1);
            SetRemainingWord(newString);
        }
    }

    private void GetNextLetter()
    {

            currentLetter = remainingWord[0];
    }
}
