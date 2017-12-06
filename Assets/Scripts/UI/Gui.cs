using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gui : MonoBehaviour {
    public static Gui Instance;
    public RadialMenu radialMenu;
    public ScrollLog scrollLog;
    public WinDialog winDialog;
    public CreatureTooltip creatureTooltip;
    Gui()
    {
        Instance = this;
    }
}