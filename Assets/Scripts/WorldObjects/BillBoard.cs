using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
   
    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward); 
    }
}
