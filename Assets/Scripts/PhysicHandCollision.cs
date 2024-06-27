using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicHandCollision : MonoBehaviour
{ 
    [SerializeField] private PhysicMaterial _highFrictionMat;
    [SerializeField] private PhysicMaterial _standardMat;
    private Coroutine _ignoreGripCoroutine;
    
    /*
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<XRHoldInteractable>() != null)
        {
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.material = _highFrictionMat;
            }
        }
/*
        _ignoreGripCoroutine = StartCoroutine(IgnoreGrip());
*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<XRHoldInteractable>() != null)
        {
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.material = _standardMat;
            }
        }

        if (_ignoreGripCoroutine != null)
        {
            StopCoroutine(_ignoreGripCoroutine);
        }
    }

/*
    private IEnumerator IgnoreGrip()
    {
        while (true)
        {
            _anim.SetFloat("Grip", 0f);
            yield return null;
        }
    }
*/
}
