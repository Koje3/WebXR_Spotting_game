using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterShoot : MonoBehaviour
{
    public Transform raycastOrigin;
    public GameObject laser;

    private bool readyToShoot;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        readyToShoot = false;

        Ray ray = new Ray(raycastOrigin.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            GameObject laserClone = Instantiate(laser, raycastOrigin.position, raycastOrigin.rotation);

            Destroy(laserClone, 0.5f);
            Destroy(hit.transform.gameObject, 0.5f);
        }
        else
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");

            GameObject laserClone = Instantiate(laser, raycastOrigin.position, raycastOrigin.rotation);
            Destroy(laserClone, 0.5f);
        }

    }
}
