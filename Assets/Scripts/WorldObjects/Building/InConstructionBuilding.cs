using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InConstructionBuilding : WorldObject
{
    public bool isConstruct;

    public BuildingDrop BD;

    public GameObject[] statusCreation = new GameObject[4];
    
    public float speedConstruction;
    [SerializeField]private float currentConstruct;

    public int maxConstruct;
    protected override void Start()
    {
        isConstruct = false;
        currentConstruct = 0;
        base.Start();
    }

    public bool Drop()
    {
        if (BD.Drop())
        {
            BD.MR.enabled = false;
            statusCreation[0].SetActive(true);
            return true;
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentConstruct < maxConstruct)
        {
            Unit col = other.gameObject.transform.root.GetComponent<Unit>();
            if (col && col is BuilderUnit && col.player.name == this.player.name && col.target == this.gameObject)
            {
                foreach (GameObject go in statusCreation)
                {
                    go.SetActive(false);

                }
                currentConstruct += speedConstruction * Time.deltaTime;
                if(currentConstruct < maxConstruct / 4)
                {
                    statusCreation[0].SetActive(true);
                }
                else if (currentConstruct < (maxConstruct * 2 / 4))
                {
                    statusCreation[1].SetActive(true);
                }
                else if (currentConstruct < (maxConstruct * 3 / 4))
                {
                    statusCreation[2].SetActive(true);
                }
                else if (currentConstruct < maxConstruct)
                {
                    statusCreation[3].SetActive(true);
                }
                else EndOfConstruction();
            }
        }
    }

    private void EndOfConstruction()
    {
        BD.MR.enabled = true;
        this.GetComponent<Building>().enabled = true;
        isConstruct = true;
        this.enabled = false;
        NavMeshObstacle NMO = this.GetComponent<NavMeshObstacle>();
        if (NMO)
        {
            NMO.enabled = true;
            GameObject.FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        }
    }
    public override void OnDeath()
    {
        if (currentlySelected)
        {
            player.SelectedObjects.Remove(this.GetComponent<Building>());
        }
        base.OnDeath();
    }
}
