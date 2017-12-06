using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class CreatureInfo : MonoBehaviour
    {
        private CreatureSettings _creature;
        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _healthText;
        [SerializeField]
        private TextMeshProUGUI _energyText;
        [SerializeField]
        private TextMeshProUGUI _speedText;
        [SerializeField]
        private TextMeshProUGUI _classText;
        [SerializeField]
        private Transform _abilityPrefab;
        private List<Transform> _abilities = new List<Transform>();

        private void Content(CreatureSettings creature)
        {
            ProfileCreature profileCreature = null;
            foreach (var i in ProfileManager.Instance.lastLoadedProfile.creatures)
            {
                if (i.pet == creature.title)
                {
                    profileCreature = i;
                }
            }

            this._creature = creature;
            string titleString = string.Format("<color=#{0}>{1}</color> <size=32>({2})</size>", creature.quality, creature.petName, creature.name);

            if (profileCreature != null)
            {
                titleString += string.Format("\n<size=28>- Level</size> <size=34>{0}</size> <size=28>-</size>", profileCreature.level);
                titleString += string.Format("\n<size=28>{0} experience to next level</size>", (creature.firstLevelExp * 2 ^ (profileCreature.level - 1) - profileCreature.experience));
            }
            _titleText.text = titleString;
            _healthText.text = "<size=26>Health</size>\n" + creature.fullHp;
            _energyText.text = "<size=26>Energy</size>\n" + creature.fullPower;
            _speedText.text = "<size=26>Speed</size>\n" + creature.speed;
            string classText = "Type: " + String.Join(", ", creature.type.Select(s => s.ToString()).ToArray()) + "\n";
            classText += CreatureLoader.matrix.info[CreatureLoader.matrix.GetEnumPos(creature.type[0])] + "\n";
            classText += "Strong against: " + String.Join(", ", CreatureLoader.matrix.StrongAgainst(creature.type[0]).Select(s => s.ToString()).ToArray()) + "\n";
            _classText.text = classText;
            classText += "Weak to: " + String.Join(", ", CreatureLoader.matrix.WeakTo(creature.type[0]).Select(s => s.ToString()).ToArray()) + "\n";
            _classText.text = classText;

            foreach (var i in _abilities)
            {
                Destroy(i.gameObject);
            }
            _abilities.Clear();

            foreach (var i in creature.abilities)
            {
                Transform ability = Instantiate(_abilityPrefab, _abilityPrefab.parent);
                ability.gameObject.SetActive(true);
                _abilities.Add(ability);
                ability.GetComponentInChildren<Image>().sprite = i.icon;
                ability.GetComponentInChildren<TextMeshProUGUI>().text = i.spellName;
            }
        }

        public void Show(CreatureSettings creature)
        {
            _abilityPrefab.gameObject.SetActive(false);
            gameObject.SetActive(true);
            Content(creature);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}