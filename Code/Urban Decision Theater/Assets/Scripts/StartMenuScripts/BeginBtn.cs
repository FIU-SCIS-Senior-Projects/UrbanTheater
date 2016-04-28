using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginBtn : MonoBehaviour {

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
