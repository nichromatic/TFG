using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralGenerator;

public class QuestManager : MonoBehaviour
{
    public Generator g;
    public QuestDisplay display;

    public void CallGenerator() {
        GameObject generatedQuest = g.GenerateGameObject();
        var allData = ObjectDataContainer.GetAllObjectData(generatedQuest);
        Debug.Log("Generated quest: " + ObjectDataContainer.FindNode("Quest", allData).GetProperty("Title").GetValueAt(0));
        display.UpdateUI(generatedQuest);
    }
}
