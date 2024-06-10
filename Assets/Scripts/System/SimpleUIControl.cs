using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] ProgressControl _progressControl;
    [SerializeField] TextMeshProUGUI[] _msgTexts;

    private const int TEXT_FIELD = 0;
    private const int BTN_FIELD = 1;

    private void OnEnable()
    {
        if (_progressControl != null)
        {
            _progressControl.OnStartGame.AddListener(StartGame);
            _progressControl.OnChallangeComplete.AddListener(ChallengeCompleted);
        }
    }

    private void StartGame(string txtMsg, string btnMsg, bool isButton)
    {
        SetText(txtMsg, btnMsg, isButton);
    }

    private void ChallengeCompleted(string txtMsg, string btnMsg, bool isButton)
    {
        SetText(txtMsg, btnMsg, isButton);
    }

    public void SetText(string txtMsg, string btnMsg, bool isButton)
    {
        _msgTexts[TEXT_FIELD].text = txtMsg;
        _msgTexts[BTN_FIELD].text = btnMsg;
        if (isButton)
        {
            _msgTexts[BTN_FIELD].transform.parent.gameObject.SetActive(true);
        }
        else
        {
            _msgTexts[BTN_FIELD].transform.parent.gameObject.SetActive(false);
        }
    }
}
