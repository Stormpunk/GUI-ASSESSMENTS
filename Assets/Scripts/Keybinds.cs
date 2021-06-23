using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keybinds : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public Text forward, backwards, left, right, jump, cast;
    public GameObject currentKey;
    // Start is called before the first frame update
    void Start()
    {
        keys.Add("Forward", KeyCode.W);
        keys.Add("Backwards", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);
        keys.Add("Cast", KeyCode.Mouse1);

        forward.text = keys["Forward"].ToString();
        backwards.text = keys["Backwards"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
        cast.text = keys["Cast"].ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnGUI()
    {
        if(currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey = null;
            }
        }
    }
    public void ChangedKey(GameObject clicked)
    {
        currentKey = clicked;
    }
}
