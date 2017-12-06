using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinDialog : MonoBehaviour {
    [SerializeField]
    private List<RenderTexture> images;
    [SerializeField]
    private WinDialogCreaturePrefab prefab;
    [HideInInspector]
    public List<WinDialogCreaturePrefab> prefabs;
    [SerializeField]
    private Image imageLine;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI moneyText;
    public void Show(Player player, bool isWin)
    {        
        int totalDamage = 0;
        float modificator = 1;

        gameObject.SetActive(true);
        prefab.gameObject.SetActive(false);
        if (isWin)
        {
            titleText.text = "You Win";
            imageLine.color = new Color(0, 1, 0, 0.1f);
            modificator = 1.5f;
        }
        else
        {
            titleText.text = "You Loose";
            imageLine.color = new Color(1, 0, 0, 0.1f);
        }
        for (int i = 0; i < player.selectedCreatures.Count; i++)
        {
            totalDamage += (int)(player.selectedCreatures[i].totalDamage * modificator);
            WinDialogCreaturePrefab pref = Instantiate(prefab, prefab.transform.parent);
            pref.gameObject.SetActive(true);
            pref.image.texture = images[i];
            pref.expText.text = "+" + (int)(player.selectedCreatures[i].totalDamage * modificator) + " exp";
        }
        moneyText.text = totalDamage + "$";
    }

    public void Close()
    {
        SceneManager.LoadScene(0);
    }
}