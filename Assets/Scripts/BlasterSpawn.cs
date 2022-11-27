using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterSpawn : MonoBehaviour
{
    public GameObject blaster;
    public GameObject blasterSpawnPoint;
    public AudioClip transferSound;
    public float blasterSpawnTimeSeconds = 5f;
    public float blasterSpawnTimer;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = blaster.GetComponent<AudioSource>();
        blasterSpawnTimer = blasterSpawnTimeSeconds;
    }


    void Update()
    {
        if (blasterSpawnTimer <= 0)
        {
            blaster.transform.position = blasterSpawnPoint.transform.position;
            audioSource.PlayOneShot(transferSound);
            blasterSpawnTimer = blasterSpawnTimeSeconds;
        }


        if (blaster.transform.position.y < transform.position.y)
        {
            blasterSpawnTimer -= Time.deltaTime;

            if (blaster.transform.position.y < transform.position.y - 5)
            {
                blasterSpawnTimer = blasterSpawnTimer - Time.deltaTime * 2;
            }
        }
        else
        {
            blasterSpawnTimer = blasterSpawnTimeSeconds;
        }

    }

}
