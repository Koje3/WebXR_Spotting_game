using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Rigidbody currentRigidBody;
    public FixedJoint attachJoint;
    public float rotationSmooth;
    public bool activateSlerp;

    // Start is called before the first frame update
    void Start()
    {
        activateSlerp = false;
        attachJoint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("b"))
        {
            Snap();
        }

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Interactable")
            return;

        currentRigidBody = other.gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Interactable")
            return;

        currentRigidBody = null;
    }

    void Snap()
    {

        if (!currentRigidBody)
            return;

        StartCoroutine(SmoothSnap(currentRigidBody));

    }

    IEnumerator SmoothSnap(Rigidbody snapRigidbody)
    {
        float waitTime = 0.2f;
        float elapsedTime = 0;
        bool snapPointFound = false;

        Vector3 startPosition = snapRigidbody.transform.position;


        Transform snapPoint = snapRigidbody.transform.Find("SnapPoint");

        if (snapPoint == null)
        {
            snapPoint = snapRigidbody.transform;
            snapPointFound = false;
        }
        else
        {
            snapPointFound = true;
        }


        Vector3 absoluteMovement = transform.position - snapPoint.position;

        while (elapsedTime <= waitTime)
        {
            snapRigidbody.transform.position = Vector3.Lerp(startPosition, snapRigidbody.transform.position + (transform.position - snapPoint.position), elapsedTime / waitTime);

            if (snapPointFound == true)
            {
                snapRigidbody.transform.rotation = Quaternion.Slerp(snapRigidbody.transform.rotation,
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90,
                    transform.rotation.eulerAngles.z), elapsedTime / waitTime);
            }


            elapsedTime += Time.deltaTime;

            yield return null;
        }

        absoluteMovement = transform.position - snapPoint.position;
        snapRigidbody.transform.position += absoluteMovement;

        attachJoint.connectedBody = snapRigidbody;


    }
}
