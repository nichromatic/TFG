using System.Collections;
using System.Collections.Generic;
using ProceduralGenerator;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    public Generator generator;
    private GameObject generatedQuest;
    
    public Text questTitle;
    public Text questDescription;
    public Text questGiver;
    public Text questObjective;
    public Text questReward;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI(GameObject quest = null) {
        if (generator == null) {
            Debug.LogError("The generator object has not been set.", this);
            return;
        }
        if (quest == null) {
            generatedQuest = generator.GenerateGameObject();
        } else {
            generatedQuest = quest;
        }

        var allData = ObjectDataContainer.GetAllObjectData(generatedQuest);

        //questTitle.text = objectData.Find(data => data.GetObjectData().nodeName == "Quest").GetProperty("Title").GetValueAt(0);
        questTitle.text = ObjectDataContainer.FindNode("Quest", allData).GetProperty("Title").GetValueAt(0);

        //questDescription.text = objectData.Find(data => data.GetObjectData().nodeName == "Quest").GetProperty("Description").GetValueAt(0);
        questDescription.text = ObjectDataContainer.FindNode("Quest", allData).GetProperty("Description").GetValueAt(0);

        //questGiver.text = "Requested by: " + objectData.Find(data => data.GetObjectData().nodeName == "Giver").GetProperty("NPC").GetValueAt(0);
        questGiver.text = "Requested by: " + ObjectDataContainer.FindNode("Giver", allData).GetProperty("NPC").GetValueAt(0);

        questObjective.text = "Objective: " + ObjectDataContainer.FindNode("Action", allData).GetProperty("Type").GetValueAt(0)
            + " " + ObjectDataContainer.FindNode("Objective", allData).GetProperty("Amount").GetValueAt(0) + " " +
            ObjectDataContainer.FindNode("Objective", allData).GetProperty("Enemy").GetValueAt(0);

        bool isGold = ObjectDataContainer.FindNode("Gold", allData) != null;

        if (isGold) {
            questReward.text = "Reward: " + ObjectDataContainer.FindNode("Gold", allData).GetProperty("Amount").GetValueAt(0) + " gold";
        } else {
            questReward.text = "Reward: Treasure";
        }

    }

}
