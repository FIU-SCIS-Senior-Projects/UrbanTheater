using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HUDBudget : MonoBehaviour {

    private PersistentData persistValue;
    private Text hudBudget;
    private float budgetValue;

	// Get budget value from previous scene (PersistentData) and get the reference of the HUDBudget text
	void Awake () {
        hudBudget = GameObject.Find("BudgetText").GetComponent<Text>();
        try {
            persistValue = GameObject.Find("PersistentData").GetComponent<PersistentData>();
            budgetValue = persistValue.BudgetValue;

            hudBudget.text = budgetValue.ToString("#,#");
        }
        catch(NullReferenceException)
        {
            hudBudget.text = "Missing Object";
        }
    }

    public void DisplayValue()
    {
        try
        {
            budgetValue = persistValue.BudgetValue;
            if (budgetValue == 0)
                hudBudget.text = budgetValue.ToString();
            else
                hudBudget.text = budgetValue.ToString("#,#");
        }
        catch(NullReferenceException)
        {
            hudBudget.text = "Missing Object";
        }
    }
}
