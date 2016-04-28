using UnityEngine;

public class PersistentData : MonoBehaviour {

    // get set functions for persistent budget value
    private float budgetValue;

    // Save this GameObject when the scene changes
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public float BudgetValue
    {
        get
        {
            return budgetValue;
        }
        set
        {
            budgetValue = value;
        }
    }
}
