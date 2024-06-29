using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    public bool HasCarEntered = false;
    public string EnteredCarName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            HasCarEntered = true;
            EnteredCarName = other.transform.parent.parent.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            HasCarEntered = false;
            EnteredCarName = "";
        }
    }
}
