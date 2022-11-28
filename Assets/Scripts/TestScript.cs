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
        if (other.gameObject.tag != "Interactable" && other.gameObject.tag != "SecondGrabPoint")
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

        if (currentRigidBody.tag == "SecondGrabPoint")
        {
            transform.position = currentRigidBody.position;
            currentRigidBody.GetComponent<FixedJoint>().connectedBody = transform.GetComponent<Rigidbody>();
            transform.GetComponent<Rigidbody>().isKinematic = false;
            return;
        }

        StartCoroutine(SmoothSnap(currentRigidBody));

    }

    IEnumerator SmoothSnap(Rigidbody snapRigidbody)
    {
        float waitTime = 0.2f;
        float elapsedTime = 0;
        bool snapPointFound = false;

        Vector3 startPosition = snapRigidbody.transform.position;
        Quaternion endRotation = Quaternion.Euler(new Vector3(0,0,0));

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

                //Calculate needed rotation and then apply it with slerp
                //First calculate what is the rotation between snapPoint and target (substraction)
                //Apply the rotation to the parent object

                //The way which which way the objects are multiplied matters. If the rotating object is first in the
                //multiplication then it rotates based on local axis. If the rotating object is second, it rotates based on
                //world axis.


                endRotation = transform.rotation * Quaternion.Inverse(snapPoint.transform.localRotation);

                snapRigidbody.transform.rotation = Quaternion.Slerp(snapRigidbody.transform.rotation,
                    endRotation, elapsedTime / waitTime);

            }


            elapsedTime += Time.deltaTime;

            yield return null;
        }

        absoluteMovement = transform.position - snapPoint.position;
        snapRigidbody.transform.position += absoluteMovement;
        snapRigidbody.transform.rotation = endRotation;

        attachJoint.connectedBody = snapRigidbody;


    }
}
