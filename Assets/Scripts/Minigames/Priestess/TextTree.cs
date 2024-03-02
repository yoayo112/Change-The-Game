using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// The TextTree object contains other TextTrees as branches. Each tree in the structure carries a string representing the shared text of all of it's branches.
// Branching represents differences between the strings in the array that cosntructor uses as input.
// For example: TextTree({"you can type", "you can also", "you even can"}) will create a tree with the following structure:
//
//         "you " ---- "can " ---- "type"
//             |           └------ "also"   
//             └------ "even can"
//
//  
public class TextTree
{
    public TextTree root;
    public List<TextTree> branches;
    public string text;

    public TextTree(string string_)
    {
        root = null;
        text = string_;
        branches = new List<TextTree>();
    }

    public TextTree(string[] strings_)
    {
        root = null;
        text = Find_Common_String(strings_);
        for (int i = 0; i < strings_.Length; i++)
            strings_[i] = strings_[i].Remove(0, text.Length);

        if (strings_[0] > 0)
            Build_Branches(strings_);
    }

    public Build_Branches(string[] strings_)
    {
        bool[] grouped = new bool[strings_.length];
        List<string> group_ = new List<string>();
        for (int i = 0; i < strings_.Length; i++)
        {
            if (grouped[i] || strings_[i].Length == 0)
                continue;

            grouped[i] = true;
            group_.Add(strings_[i]);
            for (int j = 1; i + j < strings_.Length; j++)
            {
                if ((Has_Common_String(strings_[i], strings_[i + j]) && !grouped[i + j]))
                {
                    grouped[i + j] = true;
                    group_.Add(strings_[i + j]);
                }
            }
            Add_Branch(new TextTree(group_.ToArray()));
        }
    }

    public TextTree Get_Root()
    {
        return root;
    }

    public string Get_Text()
    {
        return text;
    }

    public string Get_Text_Upto_Branch()
    {
        if (root == null)
            return string.Empty;
        return root.Get_Text_Upto_Branch() + text;
    }

    public List<TextTree> Get_Branches()
    {
        return branches;
    }

    public void Set_Root(TextTree root_)
    {
        root = root_;
    }

    public void Set_Text(string text_)
    {
        text = text_;
    }

    public Add_Branch(TextTree branch_)
    {
        branches.Add(branch_);
        branch_.Set_Root(this);
    }

    private string Find_Common_String(string stringA_, string stringB_)
    {
        string output = string.Empty;
        for (int i = 0; i < stringA_.Length || i < stringB_.Length; i++)
        {
            if (stringA_[i] == stringB_[i])
                output += stringA_[i];
            else
                break;
        }
        return output;
    }

    private string Find_Common_String(string[] strings_)
    {
        if (strings_.Length == 0)
            return string.Empty;

        string output = strings_[0];
        for (int i = 1; i < strings_.Length; i++)
        {
            output = Find_Common_String(output, strings_[i]);
        }
        return output;
    }

    private bool Has_Common_String(string stringA_, string stringB_)
    {
        if (stringA_.Length == 0 || stringB_.Length == 0)
            return false;
        return stringA_[0] == stringB_[0];
    }

    //private List<string> Find_Common_Strings(string[] strings_, string string_)
    //{
    //    List<string> output = new List<string>();
    //    string commonString_;
    //
    //    for (int i = 0; i < strings_.Length; i++)
    //    {
    //        commonString_ = Find_Common_String(strings_[i], string_);
    //        if (commonString_.Length > 0 && commonString_ != string_)
    //            output.Add(commonString_);
    //    }
    //
    //    return output;
    //}
}
