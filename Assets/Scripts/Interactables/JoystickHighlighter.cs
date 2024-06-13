using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class JoystickHighlighter : MonoBehaviour
{
    [SerializeField] XRBaseInteractable _interactable;
    [SerializeField] private Material _powerOffMat;
    [SerializeField] private Material _powerOnMat;
    [SerializeField] private GameObject _powerLight;
    [SerializeField] private Renderer _highlightRenderer;

    private void OnEnable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.AddListener(PowerOnLight);
            _interactable.selectExited.AddListener(PowerOffLight);
        }

        if (_highlightRenderer != null && _powerOffMat != null)
        {
            _highlightRenderer.material = _powerOffMat;
            if (_powerLight != null)
            {
                _powerLight.SetActive(false);
            }
        }
    }

    private void OnDisable() 
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(PowerOnLight);
            _interactable.selectExited.RemoveListener(PowerOffLight);
        }
    }

    private void PowerOnLight(SelectEnterEventArgs args)
    {
        if (_highlightRenderer != null && _powerOnMat != null)
        {
            _highlightRenderer.material = _powerOnMat;
            if (_powerLight != null)
            {
                _powerLight.SetActive(true);
            }
        }
    }

    private void PowerOffLight(SelectExitEventArgs args)
    {
        if (_highlightRenderer != null && _powerOffMat != null)
        {
            _highlightRenderer.material = _powerOffMat;
            if (_powerLight != null)
            {
                _powerLight.SetActive(false);
            }
        }
    }
}
