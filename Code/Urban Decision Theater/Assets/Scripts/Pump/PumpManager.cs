using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PumpManager : MonoBehaviour {
	
	public GameObject[] toPlace;
	PumpPlacement pumpPlacement;
    PersistentData persistValue;

    HUDBudget hudBudget;
	const float pumpCost = 5000000f;
	float budget;
    
	void Start () {
		pumpPlacement = GetComponent<PumpPlacement>();
		hudBudget = GameObject.Find("HUDBudget").GetComponent<HUDBudget>();
        persistValue = GameObject.Find("PersistentData").GetComponent<PersistentData>();
        budget = persistValue.BudgetValue;
	}

	void OnGUI() {
		for (int i = 0; i < toPlace.Length; i ++) {
			if (GUI.Button( new Rect(Screen.width/20,Screen.height/15 + Screen.height/12 * i,100,30), toPlace[i].name)) {

                float new_budget = budget - pumpCost;
                if (new_budget >= 0)
                {
                    pumpPlacement.SetItem(toPlace[i]);
                    persistValue.BudgetValue = budget = new_budget;
                    hudBudget.DisplayValue();
                }
			}
		}
	}
}
