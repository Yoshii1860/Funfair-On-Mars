using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] XRButtonInteractable _startButton;
    [SerializeField] GameObject _keyLight;
    [SerializeField] string[] _msgStrings;
    [SerializeField] TextMeshProUGUI[] _msgTexts;

    private bool _isPressed = false;

    void Start()
    {
        if (_startButton != null)
        {
            _startButton.selectEntered.AddListener(OnStartButtonPressed);
        }
    }

    private void OnStartButtonPressed(SelectEnterEventArgs args)
    {
        if (!_isPressed)
        {
            SetText(_msgStrings[2], 0);
            SetText(_msgStrings[3], 1);
            if (_keyLight != null)
            {
                _keyLight.SetActive(true);
            }
            _isPressed = true;
        }
        else
        {
            SetText(_msgStrings[0], 0);
            SetText(_msgStrings[1], 1);
            if (_keyLight != null)
            {
                _keyLight.SetActive(false);
            }
            _isPressed = false;
        }
    }

    public void SetText(string msg, int index)
    {
        _msgTexts[index].text = msg;
    }
}
