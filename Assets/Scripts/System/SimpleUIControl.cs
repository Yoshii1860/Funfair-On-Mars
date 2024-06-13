using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] private ProgressControl _progressControl;
    [SerializeField] private GameObject _msgCanvas;
    [SerializeField] private XRButtonInteractable _msgButton;
    [SerializeField] private TextMeshProUGUI[] _msgTexts;

    private const int TEXT_FIELD = 0;
    private const int BTN_FIELD = 1;

    private void OnEnable()
    {
        if (_progressControl != null)
        {
            _progressControl.OnChallangeComplete.AddListener(ChallengeCompleted);
        }
    }

    private void ChallengeCompleted(string txtMsg, string btnMsg, bool isDisableable = false)
    {
        SetText(txtMsg, btnMsg, isDisableable);
    }

    public void SetText(string txtMsg, string btnMsg, bool isDisableable = false)
    {
        _msgTexts[TEXT_FIELD].text = txtMsg;
        _msgTexts[BTN_FIELD].text = btnMsg;

        if (isDisableable)
        {
            _msgCanvas.SetActive(true);
            _msgButton.selectEntered.AddListener(OnDisableCanvas);
        }
        else
        {
            _msgButton.selectEntered.RemoveListener(OnDisableCanvas);
        }
    }

    private void OnDisableCanvas(SelectEnterEventArgs args)
    {
        _msgCanvas.SetActive(false);
    }
}
