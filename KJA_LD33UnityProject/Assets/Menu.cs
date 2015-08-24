using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    public void NextLevel(string levelName)
    {
        Application.LoadLevel("main Level");
    }

}
