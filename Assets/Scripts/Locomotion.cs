using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebXR.Interactions
{
    public class Locomotion : MonoBehaviour
    {
        public GameObject locomotionParent;
        public GameObject mainRotationCamera;
        public GameObject vrCameraR;
        public Transform objectSnapPoint;
        public GameObject blaster;

        public float moveSpeed = 0.5f;
        public float rotationSpeed = 1;
        private float snapRotationTimer;

        private CharacterController characterController;
        private WebXRController rightController;
        private WebXRController leftController;

        private WebXRState xrState = WebXRState.NORMAL;
        private Collider takeTrigger;
        private bool blasterInRange;
        private Outline blasterOutline;

        private bool blasterParented;

        private void Awake()
        {
            rightController = gameObject.transform.Find("handR").GetComponent<WebXRController>();
            leftController = gameObject.transform.Find("handL").GetComponent<WebXRController>();
            characterController = locomotionParent.GetComponent<CharacterController>();


        }

        private void OnEnable()
        {
            WebXRManager.OnXRChange += OnXRChange;
        }

        private void OnDisable()
        {
            WebXRManager.OnXRChange -= OnXRChange;
        }



        void Start()
        {
            snapRotationTimer = 0f;

            takeTrigger = GetComponent<SphereCollider>();
            blasterOutline = blaster.GetComponent<Outline>();
            blasterParented = false;
        }

        private void OnXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            xrState = state;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 movementInput = new Vector3(0,0,0);
            Vector3 movementDirection = new Vector3(0, 0, 0);

            if (xrState == WebXRState.VR)
            {
                Cursor.lockState = CursorLockMode.None;

                if (takeTrigger.enabled)
                {
                    takeTrigger.enabled = false;
                    blasterOutline.enabled = false;
                }

                if (blasterParented == true)
                {
                    blasterParented = false;

                    if (blaster.transform.parent != null)
                    {
                        blaster.transform.parent = null;
                    }

                    Rigidbody blasterRigidbody = blaster.GetComponent<Rigidbody>();
                    blasterRigidbody.isKinematic = false;
                    blasterRigidbody.useGravity = true;
                    
                }


                //SMOOTH LOCOMOTION
                Vector2 thumbstickValuesRight = rightController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);

                if (thumbstickValuesRight.y > 0.05f || thumbstickValuesRight.y < -0.05f)
                {
                    Quaternion headYaw = Quaternion.Euler(0, vrCameraR.transform.eulerAngles.y, 0);

                    movementDirection = headYaw * new Vector3(0, 0, thumbstickValuesRight.y);
                }

                Vector2 thumbstickValuesLeft = rightController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);

                if (thumbstickValuesLeft.y > 0.05f || thumbstickValuesLeft.y < -0.05f)
                {
                    Quaternion headYaw = Quaternion.Euler(0, vrCameraR.transform.eulerAngles.y, 0);

                    movementDirection = headYaw * new Vector3(0, 0, thumbstickValuesLeft.y);
                }


                //SMOOTH ROTATION
                //if (thumbstickValues.x > 0.05f || thumbstickValues.x < -0.05f)
                //{

                //    locomotionParent.transform.Rotate(new Vector3(0, thumbstickValues.x * rotationSpeed, 0));
                //}


                //SNAP ROTATION
                if (thumbstickValuesRight.x > 0.85f)
                {
                    
                    if (snapRotationTimer <= 0)
                    {
                        snapRotationTimer = 0.5f;

                        locomotionParent.transform.Rotate(new Vector3(0, 35f, 0));
                    }
                }

                if (thumbstickValuesRight.x < -0.85f)
                {

                    if (snapRotationTimer <= 0)
                    {
                        snapRotationTimer = 0.5f;

                        locomotionParent.transform.Rotate(new Vector3(0, -35f, 0));
                    }
                }

                if (snapRotationTimer > 0)
                {
                    snapRotationTimer -= Time.deltaTime;
                }


                //TOUCHPAD ROTATION
                Vector2 touchpadValues = rightController.GetAxis2D(WebXRController.Axis2DTypes.Touchpad);

                if (touchpadValues.x > 0.05f || touchpadValues.x < -0.05f)
                {

                    locomotionParent.transform.Rotate(new Vector3(0, touchpadValues.x * rotationSpeed, 0));
                }

            }


            if (xrState == WebXRState.NORMAL)
            {

                //Cursor locked and invisible
                if (Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                }


                //Blaster handling
                takeTrigger.enabled = true;

                if (Input.GetKeyDown("e"))
                {
                    if (blasterParented == true)
                    {
                        blasterParented = false;
                        if (blaster.transform.parent != null)
                        {
                            blaster.transform.parent = null;
                        }

                        Rigidbody blasterRigidbody = blaster.GetComponent<Rigidbody>();
                        blasterRigidbody.isKinematic = false;
                        blasterRigidbody.useGravity = true;

                        return;
                    }

                    if (blasterInRange && blaster != null && blasterParented == false)
                    {
                        blasterParented = true;
                        blasterOutline.enabled = false;

                        blaster.transform.parent = mainRotationCamera.transform;
                        blaster.transform.position = objectSnapPoint.position;
                        blaster.transform.localRotation = objectSnapPoint.transform.localRotation;

                        Rigidbody blasterRigidbody = blaster.GetComponent<Rigidbody>();
                        blasterRigidbody.isKinematic = true;
                        blasterRigidbody.useGravity = false;
                    }
                    
                }

                if (blasterParented == true)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0) && blaster != null)
                    {
                        blaster.GetComponent<BlasterShoot>().Shoot();
                    }

                }


                //WASD inputs
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                movementInput = new Vector3(horizontalInput, 0, verticalInput);



                //mouseinputs
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    if (Input.GetAxis("Mouse X") > 0.02f || Input.GetAxis("Mouse Y") > 0.02f || Input.GetAxis("Mouse Y") < -0.02f || Input.GetAxis("Mouse X") < -0.02f)
                    {

                        // locomotionParent.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0));

                        locomotionParent.transform.RotateAround(mainRotationCamera.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed);

                        mainRotationCamera.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -rotationSpeed, 0, 0));

                        // Debug.Log("X" + Input.GetAxis("Mouse X") + "Y" + Input.GetAxis("Mouse Y"));
                    }
                }


                movementDirection = locomotionParent.transform.TransformDirection(movementInput);
            }

            //APPLY MOVEMENT
 

            characterController.SimpleMove(movementDirection * moveSpeed);

        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Blaster" && blasterParented == false)
            {
                blasterInRange = true;
                blasterOutline.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == "Blaster" && blasterParented == false)
            {
                blasterInRange = false;
                blasterOutline.enabled = false;
            }
        }
    }
}

