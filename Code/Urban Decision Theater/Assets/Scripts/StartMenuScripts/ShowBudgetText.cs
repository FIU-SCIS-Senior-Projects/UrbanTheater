 using UnityEngine;
 using UnityEngine.UI;
 
 public class ShowBudgetText: MonoBehaviour 
 {
    public Text sliderText; // public is needed to ensure it's available to be assigned in the inspector.
    private PersistentData persistValue;

    // get the persistent data game object and set the initial value.
    void Start()
    {
        persistValue = GameObject.Find("PersistentData").GetComponent<PersistentData>();
        persistValue.BudgetValue = 20 * 1000000;
    }

    // when the user changes the slider value / update the persistent data value
    public void textUpdate (float textUpdateNumber)
    {
        float budgetValue = GameObject.Find("Budget Slider").GetComponent<Slider>().value;
        sliderText.text = budgetValue.ToString();
        persistValue.BudgetValue = budgetValue * 1000000;
    }

 }