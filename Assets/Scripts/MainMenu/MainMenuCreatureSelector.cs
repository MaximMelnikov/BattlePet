using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuCreatureSelector : MonoBehaviour
    {
        private int currentCreature = 0;
        [SerializeField]
        private List<CreatureSettings> creatures;
        [SerializeField]
        private TextMeshPro levelText;
        [SerializeField]
        private TextMeshPro nameText;
        [SerializeField]
        private Button moreInfoBtn;
        [SerializeField]
        private Text moreInfoText;
        [SerializeField]
        private Button prevBtn;
        [SerializeField]
        private Button nextBtn;
        [SerializeField]
        private CreatureInfo creatureInfo;

        void Start()
        {
            List<CreatureSettings> creaturesQueue = new List<CreatureSettings>(creatures);

            foreach (var creature in creatures)
            {
                bool creatureUnlocked = false;
                for (int i = 0; i < ProfileManager.Instance.lastLoadedProfile.creatures.Count; i++)
                {
                    if (creature.title == ProfileManager.Instance.lastLoadedProfile.creatures[i].pet)
                    {
                        creatureUnlocked = true;
                        break;
                    }
                }
                if (creatureUnlocked)
                {
                    creaturesQueue.Remove(creature);
                    creaturesQueue.Insert(0, creature);
                }
            }
            creatures = creaturesQueue;

            for (int i = 0; i < creatures.Count; i++)
            {
                GameObject g = Instantiate(creatures[i].prefab);
                g.transform.position = new Vector3(i * 2, 0, 0);
                g.transform.localEulerAngles = new Vector3(0, 180, 0);
                g.GetComponent<Animation>().Play();
            }
            SetCurrentCreature();
        }

        void SetCurrentCreature()
        {
            prevBtn.gameObject.SetActive(true);
            nextBtn.gameObject.SetActive(true);
            currentCreature = Mathf.Clamp(currentCreature, 0, creatures.Count - 1);
            if (currentCreature == 0)
            {
                prevBtn.gameObject.SetActive(false);
            }
            if (currentCreature == creatures.Count - 1)
            {
                nextBtn.gameObject.SetActive(false);
            }
            nameText.text = creatures[currentCreature].title;

            Camera.main.transform.DOMoveX(currentCreature * 2, 0.5f);

            foreach (var i in ProfileManager.Instance.lastLoadedProfile.creatures)
            {
                if (i.pet == creatures[currentCreature].title)
                {
                    moreInfoBtn.interactable = true;
                    moreInfoText.text = "More Info";
                    levelText.text = "lvl" + i.level;

                    return;
                }
            }

            moreInfoBtn.interactable = false;
            moreInfoText.text = "Unlock monster to use it";
            levelText.text = "";
        }

        public void Next()
        {
            currentCreature++;
            SetCurrentCreature();
        }

        public void Previous()
        {
            currentCreature--;
            SetCurrentCreature();
        }

        public void MoreInfoClick()
        {
            creatureInfo.Show(creatures[currentCreature]);
        }
    }
}