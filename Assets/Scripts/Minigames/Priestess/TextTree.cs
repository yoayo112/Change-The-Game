/*
Project: Change the Game
File: TextTree.cs
Date Created: March 01, 2024
Author(s): Sean Thornton
Info:

Tree structure that represents the branching paths of possible lines to be typed in the typing minigame

The TextTree object contains other TextTrees as branches. Each tree in the structure carries a string representing the shared text of all of it's branches.
Branching represents differences between the strings in the array that constructor uses as input.
For example: TextTree({"you can type", "you can also", "you even can"}) will create a tree with the following structure:

 "you " ---- "can " ---- "type"
    |           └------ "also"   
    └------ "even can"


*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class TextTree
{
    public TextTree root;
    public string text;
    public List<TextTree> branches;
    public bool alive;

    public TextTree Get_Root() => root;
    public string Get_Text() => text;
    public List<TextTree> Get_Branches() => branches;
    public bool Is_Alive() => alive;

    public void Set_Root(TextTree root_) => root = root_;
    public void Set_Text(string text_) => text = text_;

    public void Kill() => alive = false;
    public void Revive() => alive = true;

    public TextTree(string[] strings_)
    {
        

        Set_Root(null);
        Set_Text(Find_Common_String(strings_));  //The text for this branch of the tree is the common starting string among all input strings. The root text can be empty if there is no common string.

        branches = new List<TextTree>();
        for (int i = 0; i < strings_.Length; i++) //To build out the branches, we remove this common string from each input string
            strings_[i] = strings_[i].Remove(0, text.Length);

        Build_Branches(strings_);

        Revive();
    }

    public static TextTree Build(string[] strings_)
    {
        for (int i = 0; i < strings_.Length; i++) {
            strings_[i] = strings_[i].ToLower();
            Select_Random_Words(ref strings_[i]);
        }

        return new TextTree(strings_);
    }

    /*public TextTree(string file_) : this(new string[0])
    {
        string line_;
        List<string> lines_ = new List<string>();

        try
        {
            StreamReader reader_ = new StreamReader(file_);
            line_ = reader_.ReadLine();
            while (line_ != null)
            {
                lines_.Add(line_);
                line_ = reader_.ReadLine();
            }
            reader_.Close();
        }
        catch(Exception e) //Having issues with Exception here.
        {
            Debug.Log("Exception: " + e.Message);
        }

        Build_Branches(lines_.ToArray());
    }*/

    public void Build_Branches(string[] strings_)
    // Adds branches to this tree based on given array of strings
    {
        if (strings_.Length == 0 || strings_[0].Length == 0)
        {
            return; //No more branches to build if input string or array is empty.
        }

        bool[] grouped_ = new bool[strings_.Length]; //grouped_[i] == true if the string strings_[i] has already been grouped
        List<string> group_ = new List<string>();
        for (int i = 0; i < strings_.Length; i++)
        {

            if (grouped_[i] || strings_[i].Length == 0)
            { //No branch is made if the string is already grouped or empty
                continue;
            }

            grouped_[i] = true;
            group_.Add(strings_[i]);

            //We compare the string only to strings further down the list as earlier strings have already been grouped
            for (int j = 1; i + j < strings_.Length; j++) 
            {
                if (grouped_[i + j])        //Go to next string if already grouped.
                    continue;

                if (Has_Common_String(strings_[i], strings_[i + j]))
                {                                                                    //If the second string has not been grouped AND the strings share a starting string:
                    grouped_[i + j] = true;                                          //Mark new string as grouped
                    group_.Add(strings_[i + j]);                                     //Add the second string to the group   
                }
            }

            Add_Branch(new TextTree(group_.ToArray()));
            //We create a new tree using the grouped array. This represents a branch to this tree and will 
            //recursively build until the string lengths are 0.

            group_.Clear();  //Clear the group_ for next iteration of loop
        }
    }

    public void Add_Branch(TextTree branch_)
    {
        branch_.Set_Root(this);
        branches.Add(branch_);
    }

    public void Remove_Branch(TextTree branch_)
    {
        branch_.Set_Root(null);
        branches.Remove(branch_);
    }

    public void Remove_Branch(string branchText_)
    {
        foreach (TextTree branch in branches)
        {
            if (branch.Get_Text() == branchText_)
            {
                Remove_Branch(branch);
                break;
            }
        }
    }

    public void Remove_All_Branches()
    {
        foreach (TextTree branch in branches)
            Remove_Branch(branch);
    }

    public string Get_Text_Upto_Branch()
    // Returns a string concatenating the text from the tree from the root upto and not including this branch
    {
        if (root == null)
            return string.Empty;
        return root.Get_Text_Upto_Branch() + root.text;
    }

    public string Get_Text_To_End()
    // Returns a string concatenating the text from this branch's first branch to the end, always taking the first branch.
    {
        if (branches.Count == 0)
            return string.Empty;
        return branches[0].text + branches[0].Get_Text_To_End();
    }

    private string Find_Common_String(string stringA_, string stringB_)
    // Returns common starting string between two given strings
    {
        string output_ = string.Empty;
        for (int i = 0; i < stringA_.Length && i < stringB_.Length; i++)
        {
            if (stringA_[i] == stringB_[i])
                output_ += stringA_[i];
            else
                break;
        }
        return output_;
    }

    private string Find_Common_String(string[] strings_)
    // Returns common starting string among all strings in given array
    {
        if (strings_.Length == 0)
            return string.Empty;

        string output_ = strings_[0];
        for (int i = 1; i < strings_.Length; i++)
        {
            output_ = Find_Common_String(output_, strings_[i]);
        }
        return output_;
    }

    private bool Has_Common_String(string stringA_, string stringB_)
    // Do the two strings share a starting string?
    {
        if (stringA_.Length == 0 || stringB_.Length == 0)
            return false;
        return stringA_[0] == stringB_[0];
    }

    private static void Select_Random_Words(ref string string_)
    // Converts string with bracketed words to a string with a random word chosen for each set up brackets. Words are deliniated by semi colons.
    // e.g. Select_Random_Words("These are [random;variable;chance] words [you;the player;anyone] can type.") may set the string to "These are random words the player can type." 
    {
        int start_;
        int end_;
        int length_;
        string[] words_;
        string word_;
        while (string_.Contains('[') && string_.Contains(']'))
        {
            start_ = string_.IndexOf('[');
            string_ = string_.Remove(start_, 1);

            end_ = string_.IndexOf(']');
            string_ = string_.Remove(end_, 1);
            end_--;     //End index is decremented to compensate removal of ']'

            length_ = end_ - start_ + 1;
            words_ = string_.Substring(start_, length_).Split(';');

            word_ = words_[Random.Range(0,words_.Length)];
            string_ = string_.Substring(0, start_) + word_ + string_.Substring(end_ + 1);
        }
    }

}
