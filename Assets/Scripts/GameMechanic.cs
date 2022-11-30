using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebXR.Interactions
{
    public class GameMechanic : MonoBehaviour
    {
        public List<GameObject> objectsToFind;
        public Transform ghostObjectPosition;
        public Material ghostMaterial;
        public GameObject fuelBar;
        public float rotationSpeed = 1;

        [Space(4)]
        public GameObject teleportEffect;
        public GameObject longTeleportEffect;
        public AudioClip teleportSound;
        public AudioClip incorrectSound;
        public AudioClip correctSound;
        public AudioClip objectWarped;
        public AudioSource centerPlatformAudioSource;

        private GameObject currentGhostObject;
        private Scrollbar fuelScrollBar;
        private float objectsStartingCount;

        private WebXRState xrState = WebXRState.NORMAL;



        void Start()
        {
            objectsStartingCount = objectsToFind.Count;
            fuelScrollBar = fuelBar.GetComponent<Scrollbar>();

            SetNewObjectToFind();
            SetFuelLevel();
        }

        private void OnEnable()
        {
            WebXR.WebXRManager.OnXRChange += OnXRChange;
        }

        private void OnDisable()
        {
            WebXRManager.OnXRChange -= OnXRChange;
        }

        private void OnXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            xrState = state;
        }


        void Update()
        {
            if (currentGhostObject)
            {
                currentGhostObject.transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
            }

            if (xrState == WebXRState.NORMAL)
            {
                SetQualityLevel(1);
            }

            if (xrState == WebXRState.VR)
            {
                SetQualityLevel(0);
            }

        }

        public void SetNewObjectToFind()
        {
            if (objectsToFind.Count > 0)
            {
                Debug.Log("Find " + objectsToFind[0]);

                currentGhostObject = Instantiate(objectsToFind[0], ghostObjectPosition.position, ghostObjectPosition.rotation);

                currentGhostObject.transform.tag = "Untagged";
                currentGhostObject.layer = 7;
                MeshRenderer[] meshRenderers = currentGhostObject.GetComponents<MeshRenderer>();

                foreach (MeshRenderer item in meshRenderers)
                {
                    item.material = ghostMaterial;
                }
            }
            else
            {
                Debug.Log("Object list empty");
            }

        }

        public void SetFuelLevel()
        {
            float zeroToOne = (objectsStartingCount - objectsToFind.Count) / objectsStartingCount;

            fuelScrollBar.size = zeroToOne;

            ColorBlock cb = fuelScrollBar.colors;
            cb.normalColor = new Color(1 - (zeroToOne / 3), 0.5f + (zeroToOne / 2), 0.45f);
            fuelScrollBar.colors = cb;
        }

        public IEnumerator CorrectObjectFound(GameObject correctObject)
        {
            centerPlatformAudioSource.PlayOneShot(correctSound);

            yield return new WaitForSeconds(0.5f);



            GameObject teleportEffectClone = Instantiate(longTeleportEffect, correctObject.transform.position, Quaternion.identity);
            Destroy(teleportEffectClone, 5);

            yield return new WaitForSeconds(0.5f);
            centerPlatformAudioSource.PlayOneShot(objectWarped);

            yield return new WaitForSeconds(2.5f);

            Destroy(currentGhostObject);
            Destroy(correctObject);
            objectsToFind.RemoveAt(0);
            SetFuelLevel();

            yield return new WaitForSeconds(2f);

            if (objectsToFind.Count > 0)
            {
                SetNewObjectToFind();
            }
            else
            {
                Debug.Log("No more objects to find");
            }

        }

        public IEnumerator ObjectTeleportedToCenter(GameObject teleportedObject, Vector3 objectPosition, Quaternion objectRotation)
        {
            yield return new WaitForSeconds(2f);

            if (teleportedObject.name == objectsToFind[0].name)
            {

                StartCoroutine(CorrectObjectFound(teleportedObject));
            }
            else
            {
                centerPlatformAudioSource.PlayOneShot(incorrectSound);
                yield return new WaitForSeconds(1f);

                centerPlatformAudioSource.PlayOneShot(teleportSound);
                yield return new WaitForSeconds(0.5f);

                GameObject teleportEffectClone = Instantiate(teleportEffect, teleportedObject.transform.position, Quaternion.identity);
                Destroy(teleportEffectClone, 2);

                teleportedObject.transform.tag = "Transferable";
                Destroy(teleportedObject.GetComponent<Rigidbody>());
                teleportedObject.transform.rotation = objectRotation;
                teleportedObject.transform.position = objectPosition;

                GameObject teleportEffectClone2 = Instantiate(teleportEffect, teleportedObject.transform.position, Quaternion.identity);
                Destroy(teleportEffectClone2, 2);

            }
        }

        public void SetQualityLevel(int index)
        {
            QualitySettings.SetQualityLevel(index, true);
        }

    }
}




