using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObject : MonoBehaviour
{
    private GameMechanic gameMechanic;


    // Start is called before the first frame update
    void Start()
    {
        gameMechanic = transform.GetComponentInParent<GameMechanic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Transferable")
        {
            //gameMechanic.ObjectTeleportedToCenter(other.transform.gameObject);
        }
    }
}
