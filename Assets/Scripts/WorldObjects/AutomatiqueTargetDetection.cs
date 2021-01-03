using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomatiqueTargetDetection : MonoBehaviour
{
    public float timeBeforeDetection;
    public bool enemyDetection = true;
    public WorldObject linkedObject;

    private SphereCollider sphereCollider;
    private Player controller;
    private bool detectionInProgress;

    // Start is called before the first frame update
    void Start()
    {
        controller = linkedObject.player;
        sphereCollider = this.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        if (linkedObject as Unit)
        {
            Unit tmp = (Unit)linkedObject;
            if (tmp.IsMoving())
            {
                detectionInProgress = false;
                sphereCollider.enabled = false;
            }
            else 
            { 
                sphereCollider.enabled = true; 
            }
        }
    }


    void OnTriggerEnter(Collider collision)
    {
        WorldObject objectTouched = collision.gameObject.GetComponentInParent<WorldObject>();
        if (objectTouched && objectTouched.gameObject != linkedObject.gameObject)
        {
            if ((enemyDetection && controller.username != objectTouched.GetPlayerName()) ||
               (!enemyDetection && controller.username == objectTouched.GetPlayerName()))
            {
                StartCoroutine("DetectionDelay", objectTouched.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        WorldObject objectTouched = collision.gameObject.GetComponentInParent<WorldObject>();
        if (objectTouched && objectTouched.gameObject != linkedObject.gameObject)
        {
            if ((enemyDetection && controller.username != objectTouched.GetPlayerName()) ||
               (!enemyDetection && controller.username == objectTouched.GetPlayerName()))
            {
                StartCoroutine("StopDetectionDelay", objectTouched.gameObject);
            }
        }
    }

    IEnumerator DetectionDelay(GameObject targ)
    {
        detectionInProgress = true;
        yield return new WaitForSeconds(timeBeforeDetection);
        if (linkedObject.target == null)
        {
            if (detectionInProgress && targ && (targ.transform.position - this.transform.position).magnitude <= sphereCollider.radius)
            {
                linkedObject.target = targ;
            }
        }
    }

    IEnumerator StopDetectionDelay(GameObject targ)
    {
        detectionInProgress = false;
        yield return new WaitForSeconds(timeBeforeDetection);
        if (linkedObject.target == targ)
        {
            if (!detectionInProgress && targ && (targ.transform.position - this.transform.position).magnitude <= sphereCollider.radius)
            {
                linkedObject.target = null;
            }
        }
    }
}
