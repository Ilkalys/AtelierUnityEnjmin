using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using UnityEngine.UI;

public class WorldObject : MonoBehaviour
{
    public GameObject target;
    public string objectName;
    public GameObject SelectionRing;
    public Sprite buildImage;
    public int cost, sellValue, hitPoints, maxHitPoints;
    public Slider LifeDisplay;
    public Image LifeDisplayColoration;
    public Player player;
    protected string[] actions = { };
    protected bool currentlySelected = false;

    protected virtual void Awake() { }
   

    protected virtual void Start()
    {
        hitPoints = maxHitPoints;
        LifeDisplay.gameObject.SetActive(false);
        if (LifeDisplay)
        {
            LifeDisplay.maxValue = maxHitPoints;
            LifeDisplay.value = hitPoints;
        }
    }

    protected virtual void Update()
    {

    }

    public virtual void onGUI()
    {
        if(GameObject.Find("OrderPanel"))
             GameObject.Find("OrderPanel").GetComponent<OrderPanelScript>().ActualiseDisplaySprite(buildImage);
    }

    public void SetSelection(bool selected)
    {
        currentlySelected = selected;
        SelectionRing.SetActive(selected);
        LifeDisplay.gameObject.SetActive(selected);

    }
    public string [] GetActions()
    {
        return actions;
    }

    public virtual void PerformAction(string actionToPerform)
    {

    }

    public virtual void RightMouseClick(GameObject hitObject, Vector3 hitpoint, Player controller, int index)
    {
        
    }
    protected virtual void Animate()
    {

    }

    public virtual bool MouseClick(GameObject hitObject, Player controller)
    {
        if(currentlySelected && hitObject)
        {
            WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
            if (worldObject) return ChangeSelection(worldObject, controller);
        }
        return true;
    }

    private bool ChangeSelection(WorldObject worldObject, Player controller)
    {
        if (controller && controller.username == player.username)
        {
            SetSelection(false);
            return false;
        }
        else return true;
    }

    public string GetPlayerName()
    {
        return player.username;
    }

    public void TakeDamage(int damage)
    {

        hitPoints -= damage;
        if(hitPoints <= 0)
        {
            OnDeath();
        }
        if (LifeDisplay)
        {
            LifeDisplay.gameObject.SetActive(true);
            LifeDisplay.maxValue = maxHitPoints;

            LifeDisplay.value = hitPoints;

             if (maxHitPoints / 4 >= hitPoints)
            {
                if(LifeDisplayColoration)
                    LifeDisplayColoration.color = Color.red;
            }
            else if(maxHitPoints / 2 >= hitPoints)
            {
                if (LifeDisplayColoration)
                    LifeDisplayColoration.color = Color.yellow;
            }
        }
    }

    public virtual void OnDeath()
    {
        if (currentlySelected)
        {
            player.SelectedObjects.Remove(this);
        }
        Destroy(this.gameObject);
    }

    public GameObject GetTarget()
    {
        return this.target;
    }
    public void SetTarget(GameObject t)
    {
        this.target = t;
    }
}
