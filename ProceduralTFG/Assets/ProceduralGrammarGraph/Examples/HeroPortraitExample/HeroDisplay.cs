using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProceduralGenerator;

public class HeroDisplay : MonoBehaviour
{

    public GameObject victoryText;
    public GameObject defeatText;
    public GameObject tieText;

    public Text heroStats;
    public Text enemyStats;

    public Generator heroGenerator;
    public Generator enemyGenerator;

    void Start() {
        CallGenerator();
    }

    public void CallGenerator() {
        var heroData = ObjectDataContainer.GetAllObjectData(heroGenerator.GenerateGameObject());
        var enemyData = ObjectDataContainer.GetAllObjectData(enemyGenerator.GenerateGameObject());

        float heroAttack = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("Attack").GetValueAt(0)) * 10;
        float heroDef = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("Defense").GetValueAt(0)) * 10;
        float heroAtkBonus = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("AttackBonus").GetValueAt(0));
        
        float enemyAttack = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("Attack").GetValueAt(0)) * 10;
        float enemyDef = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("Defense").GetValueAt(0)) * 10;
        float enemyAtkBonus = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("AttackBonus").GetValueAt(0));
        
        string heroStatsStr = "ATK = " + Mathf.RoundToInt(heroAttack) + " / DEF = " + Mathf.RoundToInt(heroDef) + " / ATK BONUS = x" + heroAtkBonus.ToString("0.00");
        string enemyStatsStr = "ATK = " + Mathf.RoundToInt(enemyAttack) + " / DEF = " + Mathf.RoundToInt(enemyDef) + " / ATK BONUS = x" + enemyAtkBonus.ToString("0.00");

        heroStats.text = heroStatsStr;
        enemyStats.text = enemyStatsStr;

        defeatText.SetActive(false);
        victoryText.SetActive(false);
        tieText.SetActive(false);
    }

    public void Fight() {
        if (heroGenerator.generatedGameObject == null || enemyGenerator.generatedGameObject == null) {
            Debug.Log("There are no fighters yet!");
        }

        var heroData = ObjectDataContainer.GetAllObjectData(heroGenerator.generatedGameObject);
        var enemyData = ObjectDataContainer.GetAllObjectData(enemyGenerator.generatedGameObject);

        float heroAttack = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("Attack").GetValueAt(0));
        float heroDef = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("Defense").GetValueAt(0));
        float heroAtkBonus = float.Parse(ObjectDataContainer.FindNode("Stats", heroData).GetProperty("AttackBonus").GetValueAt(0));
        
        float enemyAttack = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("Attack").GetValueAt(0));
        float enemyDef = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("Defense").GetValueAt(0));
        float enemyAtkBonus = float.Parse(ObjectDataContainer.FindNode("Stats", enemyData).GetProperty("AttackBonus").GetValueAt(0));
        
        float heroAdvantage = (heroAttack * heroAtkBonus) - enemyDef;
        float enemyAdvantage = (enemyAttack * enemyAtkBonus) - heroDef;

        if (heroAdvantage > enemyAdvantage) {
            victoryText.SetActive(true);
            defeatText.SetActive(false);
            tieText.SetActive(false);
            Debug.Log("Victory!");
        } else if (enemyAdvantage > heroAdvantage) {
            defeatText.SetActive(true);
            victoryText.SetActive(false);
            tieText.SetActive(false);
            Debug.Log("Defeat ...");
        } else if (enemyAdvantage == heroAdvantage) {
            tieText.SetActive(true);
            victoryText.SetActive(false);
            defeatText.SetActive(false);
            Debug.Log("A tie!");
        }
    }
}
