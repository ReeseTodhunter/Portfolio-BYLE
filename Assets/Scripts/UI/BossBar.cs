using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BossBar : MonoBehaviour
{
    public GameObject UI;
    public static BossBar instance;
    private BTAgent agent;
    public float secondaryBarDelay = .5f;
    public float secondaryBarSpeed = .25f;
    public Slider primarySlider, secondarySlider;
    private float timeSinceDamaged = 0;
    public TextMeshProUGUI text, eliteModifierText;
    public Color BossMainColor, BossSecondaryColor;
    public Color EliteMainColor, EliteSecondaryColor;
    public Image primaryFill,secondaryFill;
    public Sprite eliteBarFrame,bossBarFrame;
    public Image barFrame;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        UI.SetActive(false);
    }

    public void BindAgentToBossBar(BTAgent _agent)
    {
        if(_agent.enemyType == BTAgent.EnemyType.standard) { return; }
        switch (_agent.enemyType)
        {
            case BTAgent.EnemyType.Elite:
                primaryFill.color = EliteMainColor;
                secondaryFill.color = EliteSecondaryColor;
                text.color = EliteMainColor;
                text.text = _agent.DisplayName;
                eliteModifierText.gameObject.SetActive(true);
                eliteModifierText.color = EliteMainColor;
                string modifiers = "";
                int x = 0;
                foreach(BTModifier mod in _agent.modifiers)
                {
                    if(x == 0){modifiers += mod.title;}
                    else{modifiers += " , " + mod.title;}
                    x++;
                }
                eliteModifierText.text = modifiers;
                barFrame.sprite = eliteBarFrame;
                barFrame.color = EliteSecondaryColor;
                break;
            default:
                primaryFill.color = BossMainColor;
                secondaryFill.color = BossSecondaryColor;
                text.color = BossMainColor;
                text.text = _agent.DisplayName;
                barFrame.sprite = bossBarFrame;
                eliteModifierText.gameObject.SetActive(false);
                barFrame.color = BossSecondaryColor;
                break;
        }
        UI.SetActive(true);
        agent = _agent;
        primarySlider.value = 1;
        secondarySlider.value = 1;
    }
    public void UnBindAgentToBossBar(BTAgent _agent)
    {
        if(agent != _agent) { return; }
        agent = null;
        UI.SetActive(false);
    }
    private void Update()
    {
        if(Time.time - timeSinceDamaged < secondaryBarDelay) { return; }
        secondarySlider.value -= Time.deltaTime * secondaryBarSpeed;
        if (secondarySlider.value <= primarySlider.value)
        {
            secondarySlider.value = primarySlider.value;
        }
    }
    public void UpdateBar(BTAgent _agent)
    {
        if(agent != _agent) { return; }
        if(agent == null) { return; }
        timeSinceDamaged = Time.time;
        primarySlider.value = agent.GetHealth() / agent.GetMaxHealth();
    }
    public void ClearBar()
    {
        agent = null;
        UI.SetActive(false);
    }
    private string GetModifierName(BTAgent _agent)
    {
        foreach (BTModifier mod in _agent.modifiers)
        {
            if(mod.title != "") {return mod.title; }
        }
        return "";
    }
}
public static class RandomEliteNames
{
    public static List<string> names = new List<string>{ 
        "Aarkan",
        "Baeron",
        "Caqor",
        "Daak",
        "Efluvium",
        "Forcam",
        "Gogam",
        "Hagmeg",
        "Isoldi",
        "Jarvmn",
        "Kadmac",
        "Lamd",
        "Mawoen",
        "Nawdioc",
        "Orax",
        "Pelvm",
        "Qxatal",
        "Radagast",
        "Satar",
        "Tcam",
        "Uval",
        "Valus",
        "Wadaal",
        "Xanta",
        "Yaoc",
        "Zeal"
    };
    public static List<string> titles = new List<string>{
        "The Supplicant",
        "Reverant of Byle",
        "Crusher of bones",
        "The unstoppable",
        "Shaper of futures",
        "The murderer",
        "The fiend",
        "The great",
        "The clever"
    };

    public static string GetRandomName()
    {
        int rnd = Random.Range(0, names.Count - 1);
        return names[rnd];
    }
    public static string GetRandomTitle()
    {
        int rnd = Random.Range(0, titles.Count - 1);
        return titles[rnd];
    }
}