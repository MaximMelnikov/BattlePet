using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Creature : MonoBehaviourEx
{
    public Player owner { get; set; }
    public int id { get; internal set; }    
    public int experience = 0;
    public int hp;
    public int power;
    public int speed = 0;
    public string petName;
    public List<Ability> abilities = new List<Ability>();    
    public Animation anim;
    private SpriteRenderer _selectedSpellSprite;
    private SphereCollider _col;
    public LineRenderer targetArrow;
    public Creature target;
    public CreatureBar creatureBar;    
    public List<Classification.Type> strongAgainst = new List<Classification.Type>();
    public List<Classification.Type> weakTo = new List<Classification.Type>();

    public int level
    {
        get
        {
            for (int i = 1; i < 10; i++)
            {
                if ((creatureBase.firstLevelExp * 2 ^ (i - 1)) > experience)
                {
                    return i;
                }
            }
            return 1;
        }
    }

    private CreatureSettings _creatureBase;
    public CreatureSettings creatureBase
    {
        get { return _creatureBase; }
        set
        {
            if (_creatureBase == null)
            {
                _creatureBase = value.GetCopy();
                _creatureBase.fullHp = LevelModificator(_creatureBase.fullHp);
            }
        }
    }

    private Ability _selectedSpell;
    public Ability selectedSpell
    {
        get { return _selectedSpell; }
        set
        {
            _selectedSpell = value;
            if (_selectedSpell != null)
            {
                _selectedSpellSprite.sprite = _selectedSpell.AbilitySettings.icon;
                _selectedSpellSprite.DOKill();
                _selectedSpellSprite.DOFade(0, 0);
                _selectedSpellSprite.DOFade(1, 0.5f);
            }
            else
            {
                _selectedSpellSprite.DOKill();
                _selectedSpellSprite.DOFade(0, 0.3f);
            }
        }
    }

    public Vector3 center
    {
        get { return transform.position + _col.center; }
    }

    private float _levelModificator = 0.8f;
    int LevelModificator(int val)
    {
        int value = val;
        if (level != 1)
        {
            value = (int)(val * level * _levelModificator);
        }
        return value;
    }

    private void Start()
    {
        anim = GetComponent<Animation>();
        hp = creatureBase.fullHp;
        power = creatureBase.fullPower;
        speed = creatureBase.speed;
        anim.Play(creatureBase.idleAnims[0]);
        _col = gameObject.AddComponent<SphereCollider>();
        _col.radius = 1;
        _col.center = new Vector3(0, 0.5f, 0);
        gameObject.layer = LayerMask.NameToLayer("Creature");
        petName = creatureBase.petName;

        foreach (var i in creatureBase.abilities)
        {
            abilities.Add(new Ability(i));
        }
        creatureBar = (Instantiate(Resources.Load("CreatureBar")) as GameObject).GetComponent<CreatureBar>();
        creatureBar.transform.position = transform.position;
        creatureBar.Init(hp);

        _selectedSpellSprite = new GameObject(name+ "selectedSpellSprite").AddComponent<SpriteRenderer>();
        _selectedSpellSprite.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        _selectedSpellSprite.transform.localRotation = Quaternion.Euler(90, 30, 0);
        _selectedSpellSprite.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        strongAgainst = CreatureLoader.matrix.StrongAgainst(creatureBase.type[0]);
        weakTo = CreatureLoader.matrix.WeakTo(creatureBase.type[0]);
    }

    void LoadArrow()
    {
        targetArrow = (Instantiate(Resources.Load("Arrow")) as GameObject).GetComponent<LineRenderer>();
        targetArrow.SetPosition(0, new Vector3(transform.position.x, 0.2f, transform.position.z));
    }

    Creature _tempTarget;
    private void Update()
    {
        if (mouseDown)
        {
            distance = Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(center));
            Creature mouseOverCreature = InputController.Instance.mouseOverCreature;
            if (distance>70 && !Gui.Instance.radialMenu.gameObject.activeSelf)
            {
                targetArrow.gameObject.SetActive(true);
                if (mouseOverCreature != null && 
                        (
                            (selectedSpell == null)
                            || ((selectedSpell!=null && mouseOverCreature.IsMyCreature && (selectedSpell.AbilitySettings.friendCast || (selectedSpell.AbilitySettings.selfCastOnly && mouseOverCreature == this) ))
                                || (selectedSpell != null && !mouseOverCreature.IsMyCreature && selectedSpell.AbilitySettings.enemieCast))
                        )
                    )
                {
                    _tempTarget = mouseOverCreature;
                    if (mouseOverCreature != this)
                    {
                        targetArrow.SetPosition(1, new Vector3(mouseOverCreature.transform.position.x, 0.2f, mouseOverCreature.transform.position.z));
                    }else
                    {
                        targetArrow.gameObject.SetActive(false);
                    }
                }
                else
                {
                    _tempTarget = this;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    // create a plane at 0,0,0 whose normal points to +Y:
                    Plane hPlane = new Plane(Vector3.up, Vector3.zero);
                    // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
                    float distance = 0;
                    // if the ray hits the plane...
                    if (hPlane.Raycast(ray, out distance))
                    {
                        // get the hit point:
                        Vector3 v = ray.GetPoint(distance);
                        targetArrow.SetPosition(1, new Vector3(v.x, 0.2f, v.z));
                    }
                }
            }
        }
    }

    #region Gui
    Coroutine RadialMenu;
    public float distance;
    bool mouseDown;
    public void MouseDown()
    {
        if (hp>0 && !stun && !Match.Instance.turning)
        {
            mouseDown = true;
            RadialMenu = Wait(0.7f, () =>
            {
                if (distance < 70)
                {
                    Gui.Instance.radialMenu.Show(this);
                }
            });
        }
    }

    public void MouseUp()
    {
        mouseDown = false;
        if (hp > 0 && !stun && !Match.Instance.turning)
        {
            if (RadialMenu != null)
            {
                StopCoroutine(RadialMenu);
                Gui.Instance.radialMenu.Hide();
            }
            RadialMenu = null;            

            if (!Gui.Instance.radialMenu.gameObject.activeSelf)
            {
                if (_tempTarget != null)
                {
                    target = _tempTarget;
                    targetArrow.gameObject.SetActive(true);
                    targetArrow.SetPosition(1, new Vector3(target.transform.position.x, 0.2f, target.transform.position.z));
                }
            }

            if (selectedSpell != null)
            {
                if ((target.IsMyCreature && (selectedSpell.AbilitySettings.friendCast || (selectedSpell.AbilitySettings.selfCastOnly && target == this)))
                    || (!target.IsMyCreature && selectedSpell.AbilitySettings.enemieCast))
                {
                    
                }
                else if (Gui.Instance.radialMenu.gameObject.activeSelf)
                {
                    target = this;
                    targetArrow.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion    

    #region Battle
    public void Positioning(Transform parent)
    {
        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;
        if (IsMyCreature)
        {
            LoadArrow();
        }
    }

    public Creature AutoTargeting()
    {
        int crNum = owner.selectedCreatures.IndexOf(this);
        
        if (crNum <= Match.Instance.players[1].selectedCreatures.Count)
        {
            target = Match.Instance.players[1].selectedCreatures[crNum];
        }
        else
        {
            target = Match.Instance.players[1].selectedCreatures.Last();
        }

        targetArrow.SetPosition(1, new Vector3(target.transform.position.x, 0.2f, target.transform.position.z));
        return target;
    }

    public bool IsMyCreature
    {
        get
        {
            return owner.isMe;
        }
    }
        
    public void StartStep()
    {        
        foreach (var i in creatureBar.buffs)
        {
            if (!i.spell.usedInThisStep)
            {
                StartCoroutine(i.spell.AffectTarget(this, StartStep));
                return;
            }
        }

        Step();
    }

    public void Step()
    {
        if (selectedSpell != null && !stun && hp>0)
        {
            transform.DOKill();
            transform.DOScale(1.5f, 0.2f);
            SetPower(power - selectedSpell.AbilitySettings.cost);
            selectedSpell.Cast(this, target, PostStep);
        }
        else
        {
            PostStep();
        }
    }
    private void PostStep()
    {
        transform.DOKill();
        transform.DOScale(1, 0.2f);
        selectedSpell = null;
        Match.Instance.NextStep();
    }
    public void Heal(int count)
    {
        hp = Mathf.Clamp(hp+count, 0, creatureBase.fullHp);
        creatureBar.SetHP(hp);
        anim.PlayQueued(stun ? creatureBase.disorientationAnims[0] : creatureBase.idleAnims[0], QueueMode.CompleteOthers);
    }

    public int totalDamage = 0;
    public void Damage(int count)
    {
        int tempHp = hp;
        hp = Mathf.Clamp(hp-count, 0, creatureBase.fullHp);
        totalDamage += tempHp - hp;
        creatureBar.SetHP(hp);
        if (hp > 0)
        {
            anim.Play(creatureBase.hitAnims[0]);
            anim.PlayQueued(stun ? creatureBase.disorientationAnims[0] : creatureBase.idleAnims[0], QueueMode.CompleteOthers);
        }
        else
        {
            Die();
        }
    }

    public void SetPower(int val)
    {
        power = Mathf.Clamp(val, 0, 100);
        creatureBar.SetPower(power);
    }

    public void Die()
    {
        anim.Play(creatureBase.dieAnims[0]);
        anim.wrapMode = WrapMode.ClampForever;
        foreach (var item in creatureBar.buffs)
        {            
            item.transform.DOScale(0, 0.1f).OnComplete(() => {
                Destroy(item.gameObject);
            });
        }
        creatureBar.buffs.Clear();
    }

    private bool _stun = false;
    public bool stun
    {
        get {
            return _stun;
        }
        set {
            _stun = value;
            if (_stun)
            {
                anim.Play(creatureBase.disorientationAnims[0]);
                anim.wrapMode = WrapMode.Loop;
            }
            else
            {
                anim.Play(creatureBase.idleAnims[0]);
                anim.wrapMode = WrapMode.Loop;
            }
        }
    }

    #endregion
}