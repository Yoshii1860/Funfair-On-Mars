using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRButtonInteractable : XRSimpleInteractable
{
    [Space(10)]
    [Header("Button Settings")]
    [SerializeField] private Image _buttonImage;

    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlightedColor;
    [SerializeField] private Color _pressedColor;
    [SerializeField] private Color _selectedColor;

    private bool _isPressed = false;

    // Start is called before the first frame update
    void Start()
    {
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
        Debug.Log("Button Pressed");
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
            _buttonImage.color = _selectedColor;
        }
    }

    public void ResetColor()
    {
        _buttonImage.color = _normalColor;
    }
}
