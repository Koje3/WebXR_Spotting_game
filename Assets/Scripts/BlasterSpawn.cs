using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterSpawn : MonoBehaviour
{
    public GameObject blaster;
    public GameObject blasterSpawnPoint;
    public AudioClip transferSound;
    public GameObject particleEffect;
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
            blasterSpawnTimer = blasterSpawnTimeSeconds;

            StartCoroutine(SpawnBlaster());
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

    public IEnumerator SpawnBlaster()
    {
        audioSource.PlayOneShot(transferSound);

        yield return new WaitForSeconds(0.4f);

        GameObject transferParticleEffect = Instantiate(particleEffect, blaster.transform.position, Quaternion.identity);
        Destroy(transferParticleEffect, 2f);

        blaster.transform.position = blasterSpawnPoint.transform.position;

        GameObject transferParticleEffect2 = Instantiate(particleEffect, blaster.transform.position, Quaternion.identity);
        Destroy(transferParticleEffect2, 2f);
    }

}
