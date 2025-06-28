using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CricketText : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textField;
    public Color standardCol;

    private Color currentCol;
    private Vector3 startSize, endSize;
    private float timer = 0;


    List<string> potentialTextLines = new List<string>{ "You'll never be good enough",
        "Quit now, it is for the best",
        "You can't do this",
        "You'll never be him",
        "The crash wasnt you fault",
        "You have to let them go",
        "No one likes you",
        "Maybe if you were better this wouldn't have happened"
    };



    private void Awake()
    {
        SetText("hey there delialah");
    }

    public void SetText(string itemText)
    {
        currentCol = standardCol;
        startSize = transform.localScale * 1.5f;
        endSize = transform.localScale * 2.5f;

        int rand = Random.Range(0, potentialTextLines.Count);

        textField.text = potentialTextLines[rand];
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
        transform.Translate(Vector3.up * Time.deltaTime * 1.5f); // Make canvas rise
        transform.localScale = Vector3.Lerp(startSize, endSize, timer / 3f);
        textField.color -= new Color(0.0f, 0.0f, 0.0f, Time.deltaTime / 3f); // Fade text out
        if (textField.color.a <= 0.0f)
        {
            Destroy(gameObject); // Destroy self when text fully gone
        }
    }
}