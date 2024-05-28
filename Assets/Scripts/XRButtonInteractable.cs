using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRButtonInteractable : XRSimpleInteractable
{
    [Space(10)]
    [Header("Button Settings")]
    [SerializeField] private Color[] _buttonColors = new Color[4];
    [SerializeField] private Image _buttonImage;

    private Color _normalColor;
    private Color _highlightedColor;
    private Color _pressedColor;
    private Color _selectedColor;

    private bool _isPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        _normalColor = _buttonColors[0];
        _highlightedColor = _buttonColors[1];
        _pressedColor = _buttonColors[2];
        _selectedColor = _buttonColors[3];

        _buttonImage.color = _normalColor;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        _buttonImage.color = _highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if (!_isPressed)
        {
            _buttonImage.color = _normalColor;
        }
        else
        {
            _buttonImage.color = _pressedColor;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!_isPressed)
        {
            _buttonImage.color = _pressedColor;
        }
        else
        {
            _buttonImage.color = _highlightedColor;
        }

        _isPressed = !_isPressed;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!_isPressed)
        {
            _buttonImage.color = _normalColor;
        }
        else
        {
            _buttonImage.color = _pressedColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
