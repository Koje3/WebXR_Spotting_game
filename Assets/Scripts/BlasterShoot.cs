using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterShoot : MonoBehaviour
{
    public Transform raycastOrigin;
    public Transform TransferPoint;
    public float laserShowTime = 1;
    [Space(4)]
    public AudioSource centerPlatformAudiosource;
    public AudioClip clickSound;
    public AudioClip laserSound;
    public AudioClip transferSound;
    public GameObject particleHit;
    public GameObject transferEffect;

    private LineRenderer laserLine;
    private AudioSource audioSource;

    private float laserTimer;
    private float transferTimer;
    private GameObject lastObject;
    private bool readyToShoot;
    private Vector3 currentHitPoint;
    private float currentHitDistance;
    private Vector3 lineStartPosition;
    private bool transferInProgress;
    private bool playClickSound;
    private float clickTimer;



    // Start is called before the first frame update
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        laserTimer = 0;
        laserLine.enabled = false;
        transferInProgress = false;


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

            lineStartPosition = Vector3.MoveTowards(lineStartPosition, currentHitPoint, 40 * Time.deltaTime);
            laserLine.SetPosition(0, lineStartPosition);

        }
        else
        {
            laserLine.enabled = false;

            if (transferInProgress == false)
            {
                readyToShoot = true;
            }
         
        }

        if (transferTimer > 0)
        {
            transferTimer = transferTimer - Time.deltaTime;
        }

        if (clickTimer > 0)
        {
            clickTimer -= Time.deltaTime;
            playClickSound = false;
        }
        else
        {
            playClickSound = true;
        }



    }/// <summary>
     /// </summary>

    public void Shoot()
    {
        
        if (readyToShoot)
        {
            readyToShoot = false;
            lineStartPosition = raycastOrigin.position;
            audioSource.PlayOneShot(laserSound);

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

                currentHitPoint = hit.point;
                currentHitDistance = hit.distance;

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

                currentHitPoint = raycastOrigin.TransformDirection(Vector3.forward) * 1000;
                currentHitDistance = 1000;
            }
        }
        else
        {
            if (playClickSound)
            {
                clickTimer = 0.3f;
                audioSource.PlayOneShot(clickSound);
            }

        }
    }

    public IEnumerator TransferObject(GameObject transferObject)
    {
        transferInProgress = true;

        centerPlatformAudiosource.PlayOneShot(transferSound);
        GameObject particleExplosion = Instantiate(particleHit, transferObject.transform.position, Quaternion.identity);
        Destroy(particleExplosion, 1.4f);

        yield return new WaitForSeconds(1f);

        transferObject.transform.gameObject.AddComponent<Rigidbody>();
        transferObject.transform.position = TransferPoint.position;

        GameObject transferParticleEffect = Instantiate(transferEffect, TransferPoint.position, Quaternion.identity);
        Destroy(transferParticleEffect, 2f);

        if (transferObject.transform.gameObject.GetComponent<Outline>() != null)
        {
            transferObject.GetComponent<Outline>().enabled = false;
        }

        transferInProgress = false;

    }
}
