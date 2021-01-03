
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;
using System.Runtime.InteropServices;

public class UserInput : MonoBehaviour
{
    public Transform selectionAreaTransform;
    public OrderPanelScript OPS;
    public LayerMask layer;

    private Player player;
    private bool isDragging;
    private Vector3 firstPointBox, secondPointBox, firstMousePos;
  
    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player.human)
        {
            MoveCamera();
            MouseActivity();
            if(OPS)
                OPS.ReinitDisplay();
            if (player.SelectedObjects.Count == 1)
            {
                InConstructionBuilding ICB = player.SelectedObjects[0].gameObject.GetComponent<InConstructionBuilding>();
                if (player.SelectedObjects[0] is Building && ICB && !ICB.isConstruct)
                {
                        ICB.onGUI();
                }
                else
                {
                    player.SelectedObjects[0].onGUI();
                }
            }
        }
    }

    private void MoveCamera()
    {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = Vector3.zero;
        Vector3 origin = Camera.main.transform.position;

        //HORIZONTAL CAMERA MOVEMENT
        if (xpos >= 0 && xpos < ResourceManager.ScroolWidth)
        {
            movement.x -= ResourceManager.ScrollSpeed;
        }
        else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScroolWidth)
        {
            movement.x += ResourceManager.ScrollSpeed;
        }
        //VERTICAL CAMERA MOVEMENT
        if (ypos >= 0 && ypos < ResourceManager.ScroolWidth)
        {
            movement.z += ResourceManager.ScrollSpeed;
        }
        else if (ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScroolWidth)
        {
            movement.z -= ResourceManager.ScrollSpeed;
        }
        movement = Camera.main.transform.TransformDirection(movement);

        //HEIGHT CAMERA MOVEMENT
        movement.z = movement.y;
        movement.y = 0;
        movement.y -= ResourceManager.ScrollSpeed  * Input.mouseScrollDelta.y;

        //CAMERA RESTRICTION CHECK
        Vector3 destination = origin + movement;
        if (destination.y > ResourceManager.MaxCameraHeight) destination.y = ResourceManager.MaxCameraHeight;
        else if (destination.y < ResourceManager.MinCameraHeight) destination.y = ResourceManager.MinCameraHeight;

        if (destination.x > ResourceManager.MaxCameraX) destination.x = ResourceManager.MaxCameraX;
        else if (destination.x < ResourceManager.MinCameraX) destination.x = ResourceManager.MinCameraX;

        if (destination.z > ResourceManager.MaxCameraZ) destination.z = ResourceManager.MaxCameraZ;
        else if (destination.z < ResourceManager.MinCameraZ) destination.z = ResourceManager.MinCameraZ;

        if (destination != origin)
        {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed * (origin.y / 2));
        }
    }

    private void MouseActivity()
    {
        //Check if the mouse is not touching the UI
        if (MouseGameArea())
        {
            if (player.buildingInHand)
            {
                //LEFT MOUSE CLIC With a building in Hand : place it and deduce Money
                if (Input.GetMouseButtonUp(0))
                {
                    if (player.buildingInHand.Drop())
                    {
                        player.AddMoney(-player.buildingInHand.cost);
                        int index = 0;
                        foreach (WorldObject SelectedObject in player.SelectedObjects)
                        {
                            if (SelectedObject is BuilderUnit)
                            {
                                BuilderUnit SelectedObjectBuilder = (BuilderUnit)SelectedObject;
                                SelectedObjectBuilder.target = player.buildingInHand.gameObject;
                                SelectedObjectBuilder.RightMouseClick(player.buildingInHand.gameObject, player.buildingInHand.gameObject.transform.position, player, index);
                            }
                            SelectedObject.SetSelection(false);
                            index++;
                        }
                        player.SelectedObjects.Clear();
                        player.buildingInHand.SetSelection(true);
                        player.SelectedObjects.Add(player.buildingInHand);
                        player.buildingInHand = null;
                    }
                }
                //RIGHT MOUSE CLIC With a building in Hand : Cancel building
                else if (Input.GetMouseButtonUp(1))
                {
                    
                    GameObject.Destroy(player.buildingInHand.gameObject);
                    player.buildingInHand = null;
                }
            }
            else
            {
                //LEFT MOUSE DOWN : place position of the mouse in the world
                if (Input.GetMouseButtonDown(0))
                {
                    isDragging = false;
                    firstMousePos = Input.mousePosition;
                   // secondPointBox = Input.mousePosition;

                    Vector3 tmp = FindHitPoint();
                    if (tmp != ResourceManager.InvalidPosition)
                        firstPointBox = tmp;
                }
                //LEFT MOUSE STILL PUSH : create the green selection's square and calcul size
                else if (Input.GetMouseButton(0))
                {
                    if (!isDragging)
                    {
                        Vector3 hitPoint = FindHitPoint();
                        if ((hitPoint != ResourceManager.InvalidPosition) && Vector3.Distance(firstPointBox, hitPoint) > 0.1)
                        {
                            isDragging = true;
                            secondPointBox = hitPoint;
                            DrawDragBox();
                            selectionAreaTransform.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Vector3 tmp = FindHitPoint();
                        if (tmp != ResourceManager.InvalidPosition)
                            secondPointBox = tmp;
                        DrawDragBox();
                    }
                }
                //LEFT MOUSE UP : check Objects who was in the square or under the mouse depanding if the player drag or not
                else if (Input.GetMouseButtonUp(0))
                {
                    selectionAreaTransform.gameObject.SetActive(false);

                    if (!isDragging)
                    {
                        NotDraggedClick();

                    }
                    else
                    {
                        DraggedClick();
                    }
                }
                //RIGHT MOUSE CLIC : Trigger Action for all selected Object depending what is under the mouse
                else if (Input.GetMouseButtonDown(1))
                {
                    Vector3 tmp = FindHitPoint();
                    if (tmp != ResourceManager.InvalidPosition)
                        firstPointBox = tmp;

                    GameObject hitObject = FindHitObject();

                    if (hitObject && firstPointBox != ResourceManager.InvalidPosition)
                    {
                        //Something already selected
                        if (player.SelectedObjects.Count > 0)
                        {
                            int index = 0;
                            foreach (WorldObject SelectedObject in player.SelectedObjects)
                            {
                                SelectedObject.RightMouseClick(hitObject.gameObject, firstPointBox, player, index);
                                index++;
                            }
                        }
                    }
                }


            }
        }else if (!player.buildingInHand)
        {
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                selectionAreaTransform.gameObject.SetActive(false);
                DraggedClick();
            }
        }
        
    }

    //Deselect previous selection and Select all Unit in the "drag zone"
    private void DraggedClick()
    {
        List<WorldObject> hitObjects = FindHitUnitsOwnedByPlayer();
        foreach (WorldObject SelectedObject in player.SelectedObjects)
        {
            SelectedObject.SetSelection(false);
        }
        player.SelectedObjects = hitObjects;
        foreach (WorldObject SelectedObject in player.SelectedObjects)
        {
            SelectedObject.SetSelection(true);
        }
    }

    //Deselect previous selection and Select the object under the mouse
    private void NotDraggedClick()
    {

        Vector3 tmp = FindHitPoint();
        if (tmp != ResourceManager.InvalidPosition)
            firstPointBox = tmp;
        if (FindHitObject())
        {
            WorldObject hitObject = FindHitObject().transform.root.GetComponent<WorldObject>();
            if (hitObject && firstPointBox != ResourceManager.InvalidPosition)
            {
                //Something already selected
                if (player.SelectedObjects.Count > 0)
                {
                    List<WorldObject> staySelected = new List<WorldObject>();
                    foreach (WorldObject SelectedObject in player.SelectedObjects)
                    {
                        if (SelectedObject.MouseClick(hitObject.gameObject, player))
                            staySelected.Add(SelectedObject);
                    }
                    player.SelectedObjects = staySelected;
                    if (staySelected.Count == 0)
                    {
                        player.SelectedObjects.Add(hitObject);

                        hitObject.SetSelection(true);
                    }
                }
                //nothing was selected
                else if (hitObject.tag != "Ground")
                {
                    WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
                    if (worldObject)
                    {
                        player.SelectedObjects.Add(worldObject);

                        worldObject.SetSelection(true);
                    }
                }
            }
            else
            {
                foreach (WorldObject SelectedObject in player.SelectedObjects)
                {
                    SelectedObject.SetSelection(false);
                }
                player.SelectedObjects.Clear();
               

            }
        }
    }

    //Check if the mouse is not in the UI
    public bool MouseGameArea()
    {
        Vector3 mousePos = Input.mousePosition;
        bool insideWitdh = mousePos.y >= 0 + ResourceManager.ORDERS_BAR_WIDTH && mousePos.y <= Screen.width;

        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - ResourceManager.RESOURCE_BAR_HEIGHT;
         
        return insideWitdh && insideHeight;
    }

    //Find an object under the mouse
    private GameObject FindHitObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    //Find all unit in the "drag zone"
    private List<WorldObject> FindHitUnitsOwnedByPlayer()
    {
        Vector3 lowerLeft = new Vector3(
       Mathf.Min(firstPointBox.x, secondPointBox.x), firstPointBox.y,
       Mathf.Min(firstPointBox.z, secondPointBox.z)
       );
        Vector3 upperRight = new Vector3(
            Mathf.Max(firstPointBox.x, secondPointBox.x), firstPointBox.y,
            Mathf.Max(firstPointBox.z, secondPointBox.z)
            );
        Vector3 center = lowerLeft + ((upperRight - lowerLeft) / 2) ;
        Vector3 halfExtents = (upperRight - lowerLeft) / 2;
         halfExtents.y = 100;


        Collider[] hitcollider = Physics.OverlapBox(center, halfExtents, this.transform.rotation, LayerMask.GetMask("Unit"));
        List<WorldObject> unitsOwnedCollider = new List<WorldObject>();
        for(int i = 0; i < hitcollider.Length; i++)
        {
            WorldObject wO = hitcollider[i].transform.root.GetComponent<WorldObject>();
            if (wO && wO.GetPlayerName() == player.username && wO is Unit && unitsOwnedCollider.Count < ResourceManager.MAXUNITINCHARGE && !unitsOwnedCollider.Contains(wO))
             unitsOwnedCollider.Add(wO);
        }
        return unitsOwnedCollider;
    }

    //Find position in the world under the mouse
    private Vector3 FindHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer)) 
        {
            return hit.point; 
        }
        return ResourceManager.InvalidPosition;
    }

    //Draw the selection Square in the game
    private void DrawDragBox()
    {
        Vector3 lowerLeft = new Vector3(
           Mathf.Min(firstPointBox.x, secondPointBox.x), firstPointBox.y,
           Mathf.Min(firstPointBox.z, secondPointBox.z)
           );
        Vector3 upperRight = new Vector3(
            Mathf.Max(firstPointBox.x, secondPointBox.x), firstPointBox.y,
            Mathf.Max(firstPointBox.z, secondPointBox.z)
            );
        selectionAreaTransform.position = lowerLeft + ((upperRight - lowerLeft) / 2);
        Vector3 localS = upperRight - lowerLeft;
        selectionAreaTransform.localScale = new Vector3(localS.x, localS.z, 1);
    }

}
