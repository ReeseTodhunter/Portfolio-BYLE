using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupFade : MonoBehaviour
{
    [SerializeField] Text textField;
    public Color standardCol;
    public Color burnCol;
    public Color poisonCol;
    public Color stunCol;
    public Color critColor;
    private Color currentCol;
    private Vector3 startSize, endSize;
    private float timer = 0;
    private void Awake()
    {
        if (textField == null) Destroy(gameObject);
    }

    public void SetText(string itemText, EffectType damageType = EffectType.None, bool _isCrit = false)
    {
        if(_isCrit)
        {
            currentCol = critColor;
            transform.localScale *= 2f;
        }
        else
        {
            switch(damageType)
            {
                case EffectType.None:
                    currentCol = standardCol;
                    break;
                case EffectType.Burn:
                    currentCol = burnCol;
                    break;
                case EffectType.Poison:
                    currentCol = poisonCol;
                    break;
                case EffectType.Stun:
                    currentCol = stunCol;
                    break;
            }
        }
        startSize = transform.localScale * 1.5f;
        endSize = transform.localScale * 0.5f;
        textField.color = currentCol;
        textField.text = itemText;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        transform.Translate(Vector3.up * Time.deltaTime * 3.0f); // Make canvas rise
        transform.localScale = Vector3.Lerp(startSize,endSize,timer);
        textField.color -= new Color(0.0f, 0.0f, 0.0f, Time.deltaTime / 1.5f); // Fade text out
        if (textField.color.a <= 0.0f)
        {
            Destroy(gameObject); // Destroy self when text fully gone
        }
    }
}
