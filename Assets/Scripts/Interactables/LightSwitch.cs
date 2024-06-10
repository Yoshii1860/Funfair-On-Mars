using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class LightSwitch : XRGrabInteractable
{
    public UnityEvent OnLightSwitchedOn;

    [Space(10)]
    [Header("References")]
    // Light components
    [SerializeField] private Light _light;
    [SerializeField] private float _lightMaxIntensity = 20f;
    private float _previousLightIntensity = 0f;

    [Space(10)]
    // Ray particle system components
    [SerializeField] private ParticleSystem _rayPS;
    private Color _rayColor;

    [Space(10)]
    // Light knob components
    [SerializeField] private Transform _lightKnob;
    [SerializeField] [Range (-90f, -178f)] private float _maxRotation = -170f;
    [SerializeField] [Range (-170f, -179f)] private float _maxRotationLimit = -179f;
    [SerializeField] [Range (-90f, -2f)] private float _minRotation = -10f;
    [SerializeField] [Range (-10f, -1f)] private float _minRotationLimit = -1f;

    [Space(10)]
    // Light surface components
    [SerializeField] private Renderer _lightSurface;
    private Material _lightSurfaceMaterial;
    private Color _surfaceEmissionColor;
    
    // Grabber components
    private IXRSelectInteractor _currentGrabber;
    private float _previousGrabberRotation = 0f;
    private bool _isGrabbed = false;
    private bool _isFirstFrame = true;

    // Layer masks
    private const string DEFAULT_LAYER = "Default";
    private const string GRAB_LAYER = "Grab";

    private void Start()
    {
        if (_lightKnob != null)
        {
            _lightKnob.localEulerAngles = new Vector3(0, 0, _minRotation);
        }

        if (_lightSurface != null)
        {
            _lightSurfaceMaterial = _lightSurface.materials[1];
        }

        if (_lightSurfaceMaterial != null)
        {
            _surfaceEmissionColor = _lightSurfaceMaterial.GetColor("_EmissionColor");
            _lightSurfaceMaterial.SetColor("_EmissionColor", new Color(0, 0, 0, 1));
        }

        if (_rayPS != null)
        {
            _rayColor = _rayPS.startColor;
            _rayPS.startColor = new Color(_rayColor.r, _rayColor.g, _rayColor.b, 0f);
            _rayPS.Clear();
        }
    }

    private void Update()
    {
        if (_isGrabbed)
    	{
            AdjustValues();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _isGrabbed = true;
        _isFirstFrame = true;
        _currentGrabber = args.interactor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _isGrabbed = false;
        ChangeLayerMask(GRAB_LAYER);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
    }

    private void AdjustValues()
    {
        float currentGrabberRotation = _currentGrabber.transform.localEulerAngles.z;
        float deltaRotation = NormalizeAngle(currentGrabberRotation - _previousGrabberRotation);
        _previousGrabberRotation = currentGrabberRotation;

        // avoid high delta rotation on first frame caused by the grabbers initial rotation
        if (_isFirstFrame)
        {
            deltaRotation = 0;
            _isFirstFrame = false;
        }

        float newZrotation = NormalizeAngle(_lightKnob.localEulerAngles.z + deltaRotation);
        bool isOutsideLimits = CheckLimits(newZrotation);

        if (!isOutsideLimits)
        {
            float lightIntensity = newZrotation / _maxRotation * _lightMaxIntensity;

            _lightKnob.localEulerAngles = new Vector3(0, 0, newZrotation);
            _light.intensity = lightIntensity;
            _lightSurfaceMaterial.SetColor("_EmissionColor", new Color(_surfaceEmissionColor.r * lightIntensity / _lightMaxIntensity, _surfaceEmissionColor.g * lightIntensity / _lightMaxIntensity, _surfaceEmissionColor.b * lightIntensity / _lightMaxIntensity, 1));
            _rayPS.startColor = new Color(_rayColor.r, _rayColor.g, _rayColor.b, lightIntensity / _lightMaxIntensity);

            _previousLightIntensity = lightIntensity;
        }
    }

    private bool CheckLimits(float zRotation)
    {
        // check if knob is at the min rotation limit
        if (zRotation >= _minRotationLimit)
        {
            // avoid delta overflow when knob is at max rotation limit to not snap to min rotation limit
            if (_previousLightIntensity > _lightMaxIntensity - 1f)
            {
                _lightKnob.localEulerAngles = new Vector3(0, 0, _maxRotation);
                _light.intensity = _lightMaxIntensity;
                _lightSurfaceMaterial.SetColor("_EmissionColor", _surfaceEmissionColor);
                _rayPS.startColor = _rayColor;
                ChangeLayerMask(DEFAULT_LAYER);
                OnLightSwitchedOn?.Invoke();
                return true;
            }
            _lightKnob.localEulerAngles = new Vector3(0, 0, _minRotation);
            _light.intensity = 0f   ;
            _lightSurfaceMaterial.SetColor("_EmissionColor", new Color(0, 0, 0, 1));
            _rayPS.startColor = new Color(_rayColor.r, _rayColor.g, _rayColor.b, 0f);
            _rayPS.Clear();
            ChangeLayerMask(DEFAULT_LAYER);
            return true;
        }
        // check if knob is at the max rotation limit
        else if (zRotation <= _maxRotationLimit)
        {
            _lightKnob.localEulerAngles = new Vector3(0, 0, _maxRotation);
            _light.intensity = _lightMaxIntensity;
            _lightSurfaceMaterial.SetColor("_EmissionColor", _surfaceEmissionColor);
            _rayPS.startColor = _rayColor;
            ChangeLayerMask(DEFAULT_LAYER);
            OnLightSwitchedOn?.Invoke();
            return true;
        }
        // knob is within limits
        else 
        {
            return false;
        }
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180)
        {
            return angle -= 360;
        }
        else if (angle < -180)
        {
            return angle += 360;
        }
        else
        {
            return angle;
        }
    }

    private void ChangeLayerMask(string layerName)
    {
        interactionLayers = InteractionLayerMask.GetMask(layerName);
    }
}
