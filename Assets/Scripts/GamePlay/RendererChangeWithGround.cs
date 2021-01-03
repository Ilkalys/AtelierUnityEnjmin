using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererChangeWithGround : MonoBehaviour
{
    public GameObject rabbit, boat;

    // Check the ground Type and adapt the Renderer of the Unit
    // Currently : changement only on Water
    void Update()
    {
        boat.SetActive(false);
        rabbit.SetActive(true);

        float dist = 10;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, dist))
        {
            //Ground = Sea
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("See"))
            {
                rabbit.SetActive(false);
                boat.SetActive(true);
            }
        }
    }
}
