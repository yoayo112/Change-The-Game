using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatTerminal : MonoBehaviour
{
    public GameObject startCombat;
    public GameObject stopCombat;
    public GameObject fadeIn;
    private Animator _fade;

    // Start is called before the first frame update
    void Start()
    {
        _fade = fadeIn.GetComponentInChildren<Animator>();
    }

    public IEnumerator countdown()
    {
        _fade = fadeIn.GetComponentInChildren<Animator>();
        //wait for fade in.
        yield return GlobalService.AnimWait(_fade, "", "Fade from black");
        GlobalService.Find_Canvas_In_Children(gameObject, "transition").gameObject.SetActive(false);
        //start counting
        Animator counter = startCombat.GetComponentInChildren<Animator>();
        int i = 3;
        while (i > 0)
        {
            startCombat.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "" + i;
            yield return GlobalService.AnimWait(counter, ""+i, ""+i);
            i--;
        }
        startCombat.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        startCombat.gameObject.SetActive(false);
        yield return null;
    }

    public void End_Combat()
    {
        stopCombat.SetActive(true);
        stopCombat.GetComponentInChildren<Animator>().SetTrigger("over");
    }

    public void GTFO()
    {
        transform.GetComponentInChildren<BoxCollider>().enabled = true;
    }
}
