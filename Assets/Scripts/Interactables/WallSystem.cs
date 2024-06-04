using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.Events;

[ExecuteAlways]
public class WallSystem : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField] private GameObject _originalWallCubePrefab;
    [SerializeField] private GameObject _originalSocketWallPrefab;
    [SerializeField] private GameObject _wallCubePrefab;
    [SerializeField] private GameObject _socketWallPrefab;
    [SerializeField] Dynamite _dynamite;
    [SerializeField] private AudioClip _explodeWallClip;
    [SerializeField] private AudioClip _socketClip;
    [Space(10)]
    [Header("Build Wall Settings")]
    [SerializeField] [Range(0.126f, 1f)] private float _cubeSize = 0.5f;
    [SerializeField] private int _wallHeight = 5;
    [SerializeField] private int _wallWidth = 5;
    [SerializeField] private float _cubeSpacing = 0.005f;
    [SerializeField] private int _socketedWallHeight = 1;
    [SerializeField] private int _explosionPower = 2000;
    [Space(10)]
    [Header("Wall Operations")]
    [SerializeField] private bool _buildWall = false;
    [SerializeField] private bool _deleteWall = false;
    [SerializeField] private bool _explodeWall = false;
    [Space(10)]
    [Header("Generated Wall Columns")]
    [SerializeField] private List<GeneratedColumn> _generatedColumns;

    private Vector3 _cubeSizeVector;
    private Vector3 _spawnPosition = new Vector3(0, 0, 0);
    private Vector3 _originalSocketSize;
    
    private XRSocketInteractor _socketInteractor;
    private GameObject[] _wallCubes;

    private int _wallWidthCounter = 0;

    public XRSocketInteractor GetSocketInteractor() => _socketInteractor;
    public AudioClip GetExplodeWallClip() => _explodeWallClip;
    public AudioClip GetSocketedClip() => _socketClip;

    private void Start()
    {
        if (_originalSocketWallPrefab != null)
        {
            float socketSize = _originalSocketWallPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x * _originalSocketWallPrefab.transform.localScale.x;
            _originalSocketSize = new Vector3(socketSize, socketSize, socketSize);
        }
        
        ResizePrefabs();

        if (_socketInteractor != null)
        {
            Debug.Log("Adding Socket Interactor Listeners");
            _socketInteractor.selectEntered.AddListener(OnSelectEntered);
            _socketInteractor.selectExited.AddListener(OnSelectExited);
        }
        else
        {
            _socketInteractor = GetComponentInChildren<XRSocketInteractor>();
            if (_socketInteractor != null)
            {
                Debug.Log("Adding Socket Interactor Listeners");
                _socketInteractor.selectEntered.AddListener(OnSelectEntered);
                _socketInteractor.selectExited.AddListener(OnSelectExited);
            }
            else Debug.LogWarning("Socket Interactor is still null on Start()");
        }

        if (_dynamite != null)
        {
            _dynamite.OnExplode.AddListener(OnDestroyWall);
        }
    }

    private void Update()
    {
        if (_buildWall)
        {
            _buildWall = false;
            BuildWall();
        }

        if (_deleteWall)
        {
            _deleteWall = false;

            for (int i = 0; i < _generatedColumns.Count; i++)
            {
                _generatedColumns[i].DeleteColumn();
            }

            _wallWidthCounter = 0;
            _generatedColumns.Clear();
        }
    }

    private void ResizePrefabs()
    {
        if (_wallCubePrefab == null)
        {
            return;
        }

        _wallCubePrefab.transform.localScale = new Vector3(_cubeSize, _cubeSize, _cubeSize);
        _socketWallPrefab.transform.localScale = new Vector3(_cubeSize, _cubeSize, _cubeSize);
        _socketWallPrefab.transform.GetChild(0).localScale = (_originalSocketSize / _cubeSize) * 2f;

        _cubeSizeVector = _wallCubePrefab.GetComponent<Renderer>().bounds.size;
    }

    private void BuildWall()
    {
        if (_originalSocketWallPrefab != null)
        {
            float socketSize = _originalSocketWallPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x * _originalSocketWallPrefab.transform.localScale.x;
            _originalSocketSize = new Vector3(socketSize, socketSize, socketSize);
        }
        
        ResizePrefabs();

        _spawnPosition = transform.position;

        int socketedWallHeight = Random.Range(0, _wallHeight);

        for (int i = 0; i < _wallWidth; i++)
        {
            if (i == socketedWallHeight)
            {
                GenerateColumn(i, _wallHeight, true);
            }
            else
            {
                GenerateColumn(i, _wallHeight, false);
            }

            _spawnPosition.x += _cubeSizeVector.x + _cubeSpacing;
            _wallWidthCounter++;
        }
    }

    private void GenerateColumn(int index, int height, bool socketed)
    {
        GameObject wallColumn = new GameObject("Wall Column " + _wallWidthCounter);
        wallColumn.transform.parent = transform;

        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializeColumn(wallColumn.transform, index, height, socketed);

        _spawnPosition.y = transform.position.y;
        _wallCubes = new GameObject[height];

        for (int i = 0; i < height; i++)
        {
            if (_wallCubePrefab != null)
            {
                _wallCubes[i] = Instantiate(_wallCubePrefab, _spawnPosition, transform.rotation);
                _wallCubes[i].name = "Wall Cube " + i;
                tempColumn.SetCube(_wallCubes[i]);
            }
            _spawnPosition.y += _cubeSizeVector.y + _cubeSpacing;
        }

        if (socketed && _socketWallPrefab != null)
        {
            if (_socketedWallHeight < 0 || _socketedWallHeight >= height)
            {
                _socketedWallHeight = 1;
            }

            AddSocketWall(tempColumn, wallColumn);
        }

        for (int i = 0; i < height; i++)
        {
            if (_wallCubes[i] != null)
            {
                _wallCubes[i].transform.SetParent(wallColumn.transform);
            }
        }

        _generatedColumns.Add(tempColumn);
    }

    private void AddSocketWall(GeneratedColumn socketedColumn, GameObject parent)
    {
        if (_wallCubes[_socketedWallHeight] != null)
        {
            Vector3 socketedCubePos = _wallCubes[_socketedWallHeight].transform.position;
            DestroyImmediate(_wallCubes[_socketedWallHeight]);

            _wallCubes[_socketedWallHeight] = Instantiate(_socketWallPrefab, socketedCubePos, transform.rotation);
            socketedColumn.SetCube(_wallCubes[_socketedWallHeight]);
            _wallCubes[_socketedWallHeight].name = "Socketed Wall Cube " + _socketedWallHeight;
            _socketInteractor = _wallCubes[_socketedWallHeight].GetComponentInChildren<XRSocketInteractor>();

            parent.name = parent.name + " (Socketed)";

            if (_socketInteractor != null)
            {
                Debug.Log("Adding Socket Interactor Listeners");
                _socketInteractor.selectEntered.AddListener(OnSelectEntered);
                _socketInteractor.selectExited.AddListener(OnSelectExited);
            }
            else Debug.Log("Socket Interactor is null on AddSocketWall()");
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Socketed Wall Selected");

        if (_generatedColumns.Count >= 1)
        {
            for (int i = 0; i < _generatedColumns.Count; i++)
            {
                _generatedColumns[i].ActivateColumn();
            }
        }
    }

    private void OnDestroyWall()
    {
        if (_generatedColumns.Count >= 1)
        {
            for (int i = 0; i < _generatedColumns.Count; i++)
            {
                int randomPower = Random.Range(_explosionPower / 2, _explosionPower);
                _generatedColumns[i].ExplodeColumn(randomPower);
                StartCoroutine(DestroyAfterExplosion(_generatedColumns[i], 10f));
            }
        }

        OnDestroy?.Invoke();   
    }

    private IEnumerator DestroyAfterExplosion(GeneratedColumn column, float delay)
    {
        yield return new WaitForSeconds(delay);
        column.DeleteColumn();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("Socketed Wall Reset");

        if (_generatedColumns.Count >= 1)
        {
            for (int i = 0; i < _generatedColumns.Count; i++)
            {
                _generatedColumns[i].ResetColumn();
            }
        }
    }
}

[System.Serializable]
public class GeneratedColumn
{
    [SerializeField] private int _columnIndex;

    public GameObject[] WallCubes;
    public bool IsSocketed;

    private Transform _parentObject;

    public void InitializeColumn(Transform parent, int index, int width, bool isSocketed)
    {
        _columnIndex = index;
        _parentObject = parent;
        WallCubes = new GameObject[width];
        IsSocketed = isSocketed;
    }

    public void SetCube(GameObject cube)
    {
        for (int i = 0; i < WallCubes.Length; i++)
        {
            if (WallCubes[i] == null)
            {
                WallCubes[i] = cube;
                break;
            }
        }
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < WallCubes.Length; i++)
        {
            if (WallCubes[i] != null)
            {
                Object.DestroyImmediate(WallCubes[i]);
            }
        }

        if (_parentObject != null)
        {
            Object.DestroyImmediate(_parentObject.gameObject);
        }
    }

    public void ExplodeColumn(int power)
    {
        Debug.Log("Exploding Wall Column");

        for (int i = 0; i < WallCubes.Length; i++)
        {
            if (WallCubes[i] != null)
            {
                Rigidbody rb = WallCubes[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.AddRelativeForce(Random.onUnitSphere * power);
                }
            }
        }
    }

    public void ActivateColumn()
    {
        for (int i = 0; i < WallCubes.Length; i++)
        {
            if (WallCubes[i] != null)
            {
                Rigidbody rb = WallCubes[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }
    }

    public void ResetColumn()
    {
        for (int i = 0; i < WallCubes.Length; i++)
        {
            if (WallCubes[i] != null)
            {
                Rigidbody rb = WallCubes[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
    }
}
