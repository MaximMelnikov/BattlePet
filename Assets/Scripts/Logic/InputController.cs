using System;
using UnityEngine;

public class InputController : MonoBehaviour {
    public static InputController Instance;
    RaycastHit[] hits;
    public Creature mouseOverCreature;
    public EventHandler<InputEventArgs> OnMouseDown = delegate { };
    public EventHandler<InputEventArgs> OnMouseUp = delegate { };

    public class InputEventArgs : EventArgs
    {
        public Creature creature;
        public Vector2 mousePos;
        public InputEventArgs(Vector2 mousePos, Creature creature = null)
        {
            this.creature = creature;
            this.mousePos = mousePos;
        }
    }

    void Awake()
    {
        Instance = this;
    }
    bool buttonDown;
    Creature selectedCreature;
    void Update () {
        if (Input.GetMouseButton(0))
        {
            LayerMask mask = (1 << LayerMask.NameToLayer("Creature"));
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 999999, mask);
            mouseOverCreature = null;
            if (hits != null && hits.Length > 0)
            {
                mouseOverCreature = hits[0].collider.gameObject.GetComponent<Creature>();
                if (mouseOverCreature == null)
                {
                    OnMouseDown.Invoke(this, new InputEventArgs(Input.mousePosition));
                }
                else if (selectedCreature == null)
                {
                    if (!buttonDown)
                    {
                        buttonDown = true;
                        selectedCreature = mouseOverCreature;
                        if (mouseOverCreature.IsMyCreature)
                        {
                            mouseOverCreature.MouseDown();                            
                        }
                        OnMouseDown.Invoke(this, new InputEventArgs(Input.mousePosition, selectedCreature));
                    }
                }
            }else
            {
                if (!buttonDown)
                {
                    buttonDown = true;
                    OnMouseDown.Invoke(this, new InputEventArgs(Input.mousePosition));
                }                
            }
        }
        if (buttonDown && !Input.GetMouseButton(0))
        {
            buttonDown = false;
            if (selectedCreature)
            {
                OnMouseUp.Invoke(this, new InputEventArgs(Input.mousePosition, selectedCreature));
                selectedCreature.MouseUp();
                selectedCreature = null;
            }
            else
            {
                OnMouseUp.Invoke(this, new InputEventArgs(Input.mousePosition));
            }
        }
    }
}
