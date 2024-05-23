/*
Project: Change the Game
File: Enums.cs
Date Created: March 04, 2024
Author(s): Sean Thornton, Elijah Theander
Info:

File to store all globally available enums
*/

public enum CharacterType //Globally Public Enum for if a character is a player or enemy
{
    player,
    enemy
}

public enum StatusType
{
    inked,
    blinded,
    weakened
    //etc...
}

public enum PlayerCharacters
(
    cowboy,
    priestess
)