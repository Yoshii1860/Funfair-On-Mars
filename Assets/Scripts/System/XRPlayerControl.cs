using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerControl : MonoBehaviour
{
    [SerializeField] private GrabMoveProvider[] _grabMoveProviders;
    [SerializeField] private Collider[] _grabColliders;

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < _grabColliders.Length; i++)
        {
            if (other == _grabColliders[i])
            {
                SetGrabMovers(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < _grabMoveProviders.Length; i++)
        {
            if (other == _grabColliders[i])
            {
                SetGrabMovers(false);
            }
        }
    }

    private void SetGrabMovers(bool isActive)
    {
        for (int i = 0; i < _grabMoveProviders.Length; i++)
        {
            _grabMoveProviders[i].enabled = isActive;
        }
    }
}
