using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    public UnityAction UnlockAction;
    private void OnUnlock() => UnlockAction?.Invoke();

    [SerializeField] private XRButtonInteractable[] _comboButtons;
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private GameObject _correctCombinationText;
    [SerializeField] private GameObject _incorrectCombinationText;
    [SerializeField] private string _correctCombination = "1860";

    private string _userInput = "";

    void Start()
    {
        _displayText.text = "_" + _userInput;

        for (int i = 0; i < _comboButtons.Length; i++)
        {
            _comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs args)
    {
        for (int i = 0; i < _comboButtons.Length; i++)
        {
            if (args.interactableObject.transform.name == _comboButtons[i].transform.name)
            {
                if (_userInput.Length < _correctCombination.Length - 1)
                {
                    _userInput += i.ToString();
                    _displayText.text = "_" + _userInput;
                }
                else if (_userInput.Length == _correctCombination.Length - 1)
                {
                    _userInput += i.ToString();
                    _displayText.text = _userInput;

                    StartCoroutine(CheckCombination());
                }  
            }
            else
            {
                _comboButtons[i].ResetColor();
            }
        }
    }

    private void ResetCombination()
    {
        foreach (XRButtonInteractable button in _comboButtons)
        {
            button.ResetColor();
        }

        _displayText.enabled = true;
        _displayText.text = "_";
        _userInput = "";
        _incorrectCombinationText.SetActive(false);
        _correctCombinationText.SetActive(false);
    }

    private IEnumerator CheckCombination()
    {
        for (int i = 0; i < _comboButtons.Length; i++)
        {
            _comboButtons[i].enabled = false;
        }

        yield return new WaitForSeconds(1.0f);

        if (_userInput == _correctCombination)
        {
            _displayText.enabled = false;
            _correctCombinationText.SetActive(true);
            OnUnlock();
        }
        else
        {
            _displayText.enabled = false;
            _incorrectCombinationText.SetActive(true);

            yield return new WaitForSeconds(2.0f);

            for (int i = 0; i < _comboButtons.Length; i++)
            {
                _comboButtons[i].enabled = true;
            }

            ResetCombination();
        }
    }
}