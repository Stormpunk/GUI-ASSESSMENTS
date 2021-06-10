using System.Collections;
using UnityEngine;

public class HUDScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), "+");
    }
}
