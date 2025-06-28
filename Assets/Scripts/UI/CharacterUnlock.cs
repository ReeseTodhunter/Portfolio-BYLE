using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnlock : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI UIText;
    [SerializeField] GameObject lockedModel;
    [SerializeField] GameObject unlockedModel;

    [SerializeField] List<int> requiredAchievements = new List<int>();
    [SerializeField] [TextArea] string characterText = "Dart\nExtra dodge\nCan slow time";
    bool trulyLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<characterSelection>().selectable /*&& (AchievementSystem.achievements[16] || AchievementSystem.achievements[19])*/)
        {
            foreach (int achievement in requiredAchievements)
            {
                if (!AchievementSystem.achievements[achievement])
                {
                    return;
                }
            }

            UIText.text = characterText;

            unlockedModel.transform.position = lockedModel.transform.position;
            Destroy(lockedModel);

            GetComponent<characterSelection>().selectable = true;
        }
    }

    private void Update()
    {
        if (!trulyLocked)
        {
            if (!GetComponent<characterSelection>().selectable /*&& (AchievementSystem.achievements[16] || AchievementSystem.achievements[19])*/)
            {
                foreach (int achievement in requiredAchievements)
                {
                    if (!AchievementSystem.achievements[achievement])
                    {
                        trulyLocked = true;
                        return;
                    }
                }

                UIText.text = characterText;

                unlockedModel.transform.position = lockedModel.transform.position;
                Destroy(lockedModel);

                GetComponent<characterSelection>().selectable = true;
            }
        }
    }
}
