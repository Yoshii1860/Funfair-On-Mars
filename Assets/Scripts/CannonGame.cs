using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

class Cannon
{
    public string Name;
    public CannonBehaviour CannonScript;
    public int DamageReceived;
}

public class CannonGame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _rightGateDoor;
    [SerializeField] private Transform _leftGateDoor;

    [SerializeField] private Renderer _lightRenderer;
    private Material[] _lightMaterials;
    private Dictionary<string, Material> _lightMaterialsList = new Dictionary<string, Material>();

    [SerializeField] private CannonBehaviour[] _cannons;
    private List<Cannon> _cannonData = new List<Cannon>();
    private GameObject _particleCollider;
    [Space(10)]

    [Header("Game Settings")]
    [SerializeField] private int _maxCannonDamage = 15; 
    [SerializeField] private float _gameDuration = 60f;
    private float _timePlayed = 0f;
    public bool IsGameRunning = false;
    public int PlayerDamageReceived = 0;

    private void Start()
    {
        _lightMaterials = Resources.LoadAll<Material>("LightMaterials");
        foreach (Material material in _lightMaterials)
        {
            _lightMaterialsList.Add(material.name, material);
        }

        for (int i = 0; i < GameManager.Instance.XRRig.childCount; i++)
        {
            if (GameManager.Instance.XRRig.GetChild(i).CompareTag("ParticleCollider"))
            {
                _particleCollider = GameManager.Instance.XRRig.GetChild(i).gameObject;
            }
        }

        ResetCannonData();
    }

    private void Update()
    {
        if (IsGameRunning)
        {
            _timePlayed += Time.deltaTime;
            CheckProgress();
        }
    }

    public void StartGame()
    {
        IsGameRunning = true;
        StartCoroutine(StartGameCoroutine());
    }

    public void StopGame()
    {
        IsGameRunning = false;
        StartCoroutine(CloseGate());
    }

    private void StartCannons(bool isRunning)
    {
        foreach (CannonBehaviour cannon in _cannons)
        {
            cannon.IsRunning = isRunning;
            cannon.Agent.isStopped = !isRunning;
        }
    }

    private IEnumerator StartGameCoroutine()
    {
        IsGameRunning = true;
        ResetCannonData();
        StartCoroutine(CloseGate());
        StartCoroutine(StartLight());
        _particleCollider.SetActive(true);
        yield return new WaitForSeconds(3f);
        StartCannons(true);
    }

    private void CheckProgress()
    {
        if (PlayerDamageReceived >= 3)
        {
            _lightRenderer.material = _lightMaterialsList["yellow"];
        }
        else if (PlayerDamageReceived >= 7)
        {
            _lightRenderer.material = _lightMaterialsList["orange"];
        }
        else if (PlayerDamageReceived > 9)
        {
            _lightRenderer.material = _lightMaterialsList["red"];
        }

        if (PlayerDamageReceived >= 10 || !IsGameRunning || _timePlayed >= _gameDuration)
        {
            StartCoroutine(FinishGame(false));
        }
        else if (_cannonData.Count == 0)
        {
            StartCoroutine(FinishGame(true));
        }
    }

    private IEnumerator FinishGame(bool hasWon)
    {
        IsGameRunning = false;
        _particleCollider.SetActive(false);
        StartCannons(false);

        yield return new WaitForSeconds(1f);

        if (!hasWon)
        {
            Debug.Log("Game Over");
            _lightRenderer.material = _lightMaterialsList["red"];
        }
        else if (hasWon)
        {
            Debug.Log("Game Won");
            _lightRenderer.material = _lightMaterialsList["green"];
        }

        yield return new WaitForSeconds(2f);

        foreach (CannonBehaviour cannon in _cannons)
        {
            cannon.ResetCannon();
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine(OpenGate());
    }

    private IEnumerator StartLight()
    {
        yield return new WaitForSeconds(1f);
        _lightRenderer.material = _lightMaterialsList["green"];
    }

    private IEnumerator OpenGate()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            _rightGateDoor.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, -70, 0), t);
            _leftGateDoor.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 70, 0), t);
            yield return null;
        }
    }

    private IEnumerator CloseGate()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            _rightGateDoor.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0), t);
            _leftGateDoor.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0), t);
            yield return null;
        }
    }

    public void HitCannon(string cannonName)
    {
        Cannon cannon = _cannonData.Find(x => x.Name == cannonName);
        if (cannon != null)
        {
            cannon.DamageReceived++;
            Debug.Log(cannonName + " has been hit. Damage: " + cannon.DamageReceived);
            if (cannon.DamageReceived == _maxCannonDamage)
            {
                DeactivateCannon(cannonName);
            }
        }
    }

    private void DeactivateCannon(string cannonName)
    {
        Cannon cannon = _cannonData.Find(x => x.Name == cannonName);
        if (cannon != null)
        {
            cannon.CannonScript.Agent.isStopped = true;
            cannon.CannonScript.IsRunning = false;
            Debug.Log($"Cannon {cannonName} deactivated.");
            _cannonData.Remove(cannon);
        }
    }

    private void ResetCannonData()
    {
        _cannonData.Clear();
        _cannonData = new List<Cannon>();

        foreach (CannonBehaviour cannon in _cannons)
        {
            Cannon newCannon = new Cannon();
            newCannon.Name = cannon.name;
            newCannon.CannonScript = cannon;
            newCannon.DamageReceived = 0;
            _cannonData.Add(newCannon);
        }
    }
}
