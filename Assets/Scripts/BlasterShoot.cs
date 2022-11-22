using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterShoot : MonoBehaviour
{
    public Transform raycastOrigin;
    public Transform TransferPoint;
    public float laserShowTime = 1;

    private LineRenderer laserLine;

    private float laserTimer;
    private float transferTimer;
    private GameObject lastObject;
    private bool readyToShoot;

    

    // Start is called before the first frame update
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        laserTimer = 0;
        laserLine.enabled = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            Shoot();
        }

        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {

            if (lastObject != null && lastObject != hit.transform.gameObject)
            {
                if (lastObject.transform.gameObject.GetComponent<Outline>() != null)
                {
                    lastObject.transform.gameObject.GetComponent<Outline>().enabled = false;
                }
            }

            if (hit.transform.tag == "Transferable" && hit.transform.gameObject.GetComponent<Outline>() != null)
            {
                hit.transform.gameObject.GetComponent<Outline>().enabled = true;
            }

            lastObject = hit.transform.gameObject;

        }



        if (laserTimer > 0)
        {
            laserTimer = laserTimer - Time.deltaTime;
        }
        else
        {
            laserLine.enabled = false;
        }

        if (transferTimer > 0)
        {
            transferTimer = transferTimer - Time.deltaTime;
        }



    }/// <summary>
     /// </summary>

    public void Shoot()
    {
        readyToShoot = false;

        Ray ray = new Ray(raycastOrigin.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            laserTimer = laserShowTime;
            laserLine.enabled = true;
            laserLine.SetPosition(0, raycastOrigin.position);
            laserLine.SetPosition(1, hit.point);

            if (hit.transform.tag == "Transferable")
            {
                hit.transform.tag = "Interactable";
                StartCoroutine(TransferObject(hit.transform.gameObject));

            }


        }
        else
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");

            laserTimer = laserShowTime;
            laserLine.enabled = true;
            laserLine.SetPosition(0, raycastOrigin.position);
            laserLine.SetPosition(1, raycastOrigin.TransformDirection(Vector3.forward) * 1000);

        }

    }

    public IEnumerator TransferObject(GameObject transferObject)
    {
        yield return new WaitForSeconds(0.5f);


        transferObject.transform.gameObject.AddComponent<Rigidbody>();
        transferObject.transform.position = TransferPoint.position;

        if (transferObject.transform.gameObject.GetComponent<Outline>() != null)
        {
            transferObject.GetComponent<Outline>().enabled = false;
        }

    }
}
