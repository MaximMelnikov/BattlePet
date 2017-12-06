using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Shows info about creature
/// </summary>
public class CreatureTooltip : MonoBehaviourEx{
    private Creature creature;
    [SerializeField]
    private RectTransform border;
    [SerializeField]
    private TextMeshProUGUI TitleText;
    [SerializeField]
    private TextMeshProUGUI HealthText;
    [SerializeField]
    private TextMeshProUGUI EnergyText;
    [SerializeField]
    private TextMeshProUGUI SpeedText;
    [SerializeField]
    private TextMeshProUGUI ClassText;
    [SerializeField]
    private Transform abilityPrefab;
    private List<Transform> abilities = new List<Transform>();

    private void Start()
    {
        abilityPrefab.gameObject.SetActive(false);
        InputController.Instance.OnMouseDown += OnMouseDown;
        InputController.Instance.OnMouseUp += OnMouseUp;
        gameObject.SetActive(false);
    }

    DateTime clickTime;
    private void OnMouseDown(object sender, InputController.InputEventArgs e)
    {
        clickTime = DateTime.UtcNow.AddSeconds(0.2f);
    }

    private void OnMouseUp(object sender, InputController.InputEventArgs e)
    {
        if (DateTime.Compare(DateTime.UtcNow, clickTime) < 0) //check for fast click
        {
            if (e.creature)
            {
                Show(e.creature, e.mousePos);
            }
            else if (gameObject.activeSelf)
            {
                Hide();
            }
        }
    }
    /// <summary>
    /// Fill tooltip by creature stats
    /// </summary>
    /// <param name="creature"></param>
    private void Content(Creature creature)
    {
        this.creature = creature;
        TitleText.text = string.Format("<color=#{0}>{1}</color> <size=32>({2})</size>\n<size=28>- Level</size> <size=34>{3}</size> <size=28>-</size>", creature.creatureBase.quality, creature.petName, creature.creatureBase.name, creature.level);
        HealthText.text = string.Format("<size=26>Health</size>\n{0}/{1}", creature.hp, creature.creatureBase.fullHp);
        EnergyText.text = string.Format("<size=26>Energy</size>\n{0}/{1}", creature.power, creature.creatureBase.fullPower);
        SpeedText.text = string.Format("<size=26>Speed</size>\n{0}/{1}", creature.speed, creature.creatureBase.speed);
        string classText = string.Format("Type: {0}\n", String.Join(", ", creature.creatureBase.type.Select(s => s.ToString()).ToArray()));
        classText += CreatureLoader.matrix.info[CreatureLoader.matrix.GetEnumPos(creature.creatureBase.type[0])] + "\n";
        classText += string.Format("Strong against: {0}\n", String.Join(", ", creature.strongAgainst.Select(s => s.ToString()).ToArray()));
        ClassText.text = classText;
        classText += string.Format("Weak to: {0}\n", String.Join(", ", creature.weakTo.Select(s => s.ToString()).ToArray()));
        ClassText.text = classText;

        foreach (var i in abilities)
        {
            Destroy(i.gameObject);
        }
        abilities.Clear();

        foreach (var i in creature.abilities)
        {
            Transform ability = Instantiate(abilityPrefab, abilityPrefab.parent);
            ability.gameObject.SetActive(true);
            abilities.Add(ability);
            ability.GetComponentInChildren<Image>().sprite = i.AbilitySettings.icon;
            ability.GetComponentInChildren<TextMeshProUGUI>().text = i.AbilitySettings.spellName;
        }
    }
    
    private void Show (Creature creature, Vector2 mousePos) {
        if (mousePos.x > 0 && mousePos.x <= Screen.width/2)
        {            
            border.anchorMin = new Vector2(0.5f, 0);
            border.anchorMax = Vector2.one;
        }
        else if (mousePos.x > Screen.width / 2 && mousePos.x <= Screen.width)
        {
            border.anchorMin = Vector2.zero;
            border.anchorMax = new Vector2(0.5f, 1);
        }
        gameObject.SetActive(true);
        Content(creature);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}