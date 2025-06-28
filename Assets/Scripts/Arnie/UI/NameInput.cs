using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NameInput : MonoBehaviour
{
    public Text textBox;
    public string nameInput;

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        nameInput = gameObject.GetComponent<Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.5f)
        {
            foreach (char l in Input.inputString)
            {
                if (l == '\b') // has backspace/delete been pressed?
                {
                    if (nameInput.Length != 0)
                    {
                        nameInput = nameInput.Substring(0, nameInput.Length - 1);
                    }
                }
                else if (((l == '\n') || (l == '\r')) && nameInput.Length > 0) // enter/return
                {
                    
                    //GameManager.GMinstance.UpdateScore(nameInput);
                }
                else
                {
                    if (nameInput.Length < 10)
                    {
                        nameInput += l;
                    }
                }
                textBox.text = nameInput;
            }
            
        }
        
    }

    public void NameSubmit()
    {
        Debug.Log("Name Submitted " + nameInput + " .");
    }
}
