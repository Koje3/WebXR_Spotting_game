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

        public float moveSpeed = 0.5f;
        public float rotationSpeed = 1;
        private float snapRotationTimer;

        private CharacterController characterController;
        private WebXRController rightController;
        private WebXRController leftController;

        private WebXRState xrState = WebXRState.NORMAL;

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

                //SMOOTH LOCOMOTION
                Vector2 thumbstickValues = rightController.GetAxis2D(WebXRController.Axis2DTypes.Thumbstick);

                if (thumbstickValues.y > 0.05f || thumbstickValues.y < -0.05f)
                {
                    Quaternion headYaw = Quaternion.Euler(0, vrCameraR.transform.eulerAngles.y, 0);

                    movementDirection = headYaw * new Vector3(0, 0, thumbstickValues.y);
                }


                //SMOOTH ROTATION
                //if (thumbstickValues.x > 0.05f || thumbstickValues.x < -0.05f)
                //{

                //    locomotionParent.transform.Rotate(new Vector3(0, thumbstickValues.x * rotationSpeed, 0));
                //}


                //SNAP ROTATION
                if (thumbstickValues.x > 0.85f)
                {
                    
                    if (snapRotationTimer <= 0)
                    {
                        snapRotationTimer = 0.5f;

                        locomotionParent.transform.Rotate(new Vector3(0, 35f, 0));
                    }
                }

                if (thumbstickValues.x < -0.85f)
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
                //Cursor.lockState = CursorLockMode.Locked;

                //WASD inputs
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                movementInput = new Vector3(horizontalInput, 0, verticalInput);



                //mouseinputs
                if (Input.GetAxis("Mouse X") > 0.02f || Input.GetAxis("Mouse Y") > 0.02f || Input.GetAxis("Mouse Y") < -0.02f || Input.GetAxis("Mouse X") < -0.02f)
                {

                    // locomotionParent.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0));

                    locomotionParent.transform.RotateAround(mainRotationCamera.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed);

                    mainRotationCamera.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * -rotationSpeed, 0, 0));

                    // Debug.Log("X" + Input.GetAxis("Mouse X") + "Y" + Input.GetAxis("Mouse Y"));
                }

                movementDirection = locomotionParent.transform.TransformDirection(movementInput);
            }

            //APPLY MOVEMENT
 

            characterController.SimpleMove(movementDirection * moveSpeed);

        }
    }
}

