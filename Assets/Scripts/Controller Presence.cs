using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerPresence : MonoBehaviour
{
    [SerializeField] private GameObject handModelPrefab;
    [SerializeField] private InputDeviceCharacteristics controllerCharacteristics;

    private InputDevice targetDevice;
    private List<InputDevice> devices;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    private bool controllerFound = false;
    private float checkInterval = 1.0f;

    void Start()
    {
        StartCoroutine(CheckForControllers());   
    }

    IEnumerator CheckForControllers()
    {
        Debug.Log("Checking for devices... " + "Time: " + Time.time + " seconds.");

        devices = new List<InputDevice>();
        
        while (!controllerFound)
        {
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            if (devices.Count > 0)
            {
                controllerFound = true;
                Debug.Log("Controllers found!");

                LogDevices(devices);

                targetDevice = devices[0];

                Debug.Log("Target device: " + targetDevice.name);

                if (handModelPrefab != null)
                {
                    spawnedHandModel = Instantiate(handModelPrefab, transform);
                }
                else 
                {
                    Debug.LogError("Controller prefab is not set.");
                }

                handAnimator = spawnedHandModel.GetComponent<Animator>();

                if (handAnimator == null)
                {
                    Debug.LogError("Hand animator is not found.");
                }
            }
            else
            {
                Debug.Log("Waiting for controllers...");
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    void UpdateHandAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            Debug.Log("Trigger value: " + triggerValue);
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            Debug.Log("Grip value: " + gripValue);
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void LogDevices(List<InputDevice> devices)
    {
        foreach (var device in devices)
        {
            Debug.Log($"Device name: {device.name}, Device characteristics: {device.characteristics}");
        }
    }

    void Update()
    {
        if (controllerFound)
        {
            UpdateHandAnimation();
        }
    }
}