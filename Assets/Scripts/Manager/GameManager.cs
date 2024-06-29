using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Timers;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton Pattern

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion




    #region Enums

    public enum GameState
    {
        MENU,
        GAMEPLAY,
        PAUSE
    }

    /*
    public enum SubGameState
    {
        DEFAULT,
        EVENT,
        PICKUP,
        PAINT
    }
    */

    #endregion




    #region Fields


    public GameState CurrentGameState { get; private set; }
    //public SubGameState CurrentSubGameState { get; private set; }
    [Header("Game State")]
    public GameState LastGameState;
    //public SubGameState LastSubGameState;
    [Space(10)]

    // Player and related components
    public Transform PlayerRig;
    public Transform XRRig;
    public Transform LeftController;
    public Transform RightController;

    [Header("Flags")]
    public bool IsGamePaused = false;
    [Tooltip("When the game is loading.")]
    public bool IsLoading = false;

    #endregion




    #region Reference Management

    void UpdateReferences()
    {

    }

    #endregion




    #region Start and Initialization

    private void Start()
    {
        UpdateReferences();
    }

    #endregion




    #region Start Management

    public void StartGame()
    {
        
    }

    #endregion




    #region Game State Management

    public void OpenMenu()
    {
        SetGameState(GameState.MENU);
    }

    public void BackToMainMenu()
    {
        SetGameState(GameState.MENU);
    }

    public void PauseGame()
    {
        SetGameState(GameState.PAUSE);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.GAMEPLAY);
    }

    //private void SetGameState(GameState newState, SubGameState newSubGameState = SubGameState.DEFAULT)
    private void SetGameState(GameState newState)
    {
        LastGameState = CurrentGameState;
        //LastSubGameState = CurrentSubGameState;
        CurrentGameState = newState;
        //CurrentSubGameState = newSubGameState;

        switch (CurrentGameState)
        {
            case GameState.MENU:
                break;

            case GameState.GAMEPLAY:
                /*
                switch (CurrentSubGameState)
                {
                    case SubGameState.DEFAULT:
                        break;
                }
                */
                break;

            case GameState.PAUSE:
                break;
        }
    }
    #endregion



    /*
    #region Save and Load

    public void LoadNewGame()
    {
        IsLoading = true;
    }

    public void SaveData(string filename = "Asylum-autosave")
    {
        SaveSystem.SaveGameFile(filename, Player, InventoryManager.Instance.Weapons.transform, EnemyPool, InteractableObjectPool, _interactStaticObjectPool, _doorObjectPool, _textObjectPool, _summonObjectPool, _autoSavePool);
    }

    public void LoadData(string filename = "Asylum-autosave")
    {
        SaveData data = SaveSystem.LoadGameFile(filename);
        if (data != null)
        {
            LoadPlayerStats(data);
            LoadInventory(data);
            LoadWeapons(data);
            LoadAmmo(data);
            LoadEnemies(data);
            LoadInteractableObjects(data);
            LoadInteractableStaticObjects(data);
            LoadDoorObjects(data);
            LoadTextObjects(data);
            LoadSummonObjects(data);
            LoadEventData(data);
            LoadVariousData(data);
            LoadAutoSaveData(data);
        }
        else Debug.LogError("GameManager.cs: No save data found!");
    }

    public void LoadNewScene(string filename, bool newGame = false)
    {
        IsLoading = true;

        string sceneNameToLoad = filename.Split('-')[0].Trim();

        StartCoroutine(LoadSceneAsync(sceneNameToLoad, filename, newGame));
    }

    IEnumerator LoadSceneAsync(string sceneName, string filename, bool newGame = false)
    {
        Blackscreen.SetActive(true);

        AsyncOperation asyncLoadLoadingScreen = SceneManager.LoadSceneAsync("LoadingScreen");

        while (!asyncLoadLoadingScreen.isDone)
        {
            yield return null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float targetFillAmount = Mathf.Lerp(_progressBar.fillAmount, asyncLoad.progress, Time.deltaTime * 5f);
            _progressBar.fillAmount = targetFillAmount;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        asyncLoad.allowSceneActivation = true;
        
        yield return new WaitForSeconds(1f);

        UpdateReferences();

        yield return new WaitForSeconds(1f);

        if (!newGame)
        {
            LoadData(filename);
        }
        else
        {
            if (_debugMode) Debug.Log("GameManager - Start - Gameplay: " + SceneManager.GetActiveScene().name);
            StartGame();
            StartCoroutine(StartWithUpdatedReferences());
        }

        yield return new WaitForSeconds(1f);
        
        if (!newGame)
        {
            if (_debugMode) Debug.Log("GameManager - Start - LoadGame: " + SceneManager.GetActiveScene().name);
            StartCoroutine(LoadGameWithBlackScreen());
        }
        else if (newGame)
        {
            foreach (Enemy enemy in EnemyPool.GetComponentsInChildren<Enemy>())
            {
                enemy.gameObject.SetActive(false);
            }
        }

        if (_debugMode) Debug.Log("Scene " + sceneName + " is fully loaded!");
        IsLoading = false;
    }

    private void LoadPlayerStats(SaveData data)
    {
        Player.Health = data.Health;
        Vector3 position = new Vector3(data.Position[0], data.Position[1], data.Position[2]);
        Player.transform.position = position;
    }

    private void LoadInventory(SaveData data)
    {
        InventoryManager.Instance.Items.Clear();

        foreach (ItemData itemData in data.Items)
        {
            Item item = CreateItemFromData(itemData);
            InventoryManager.Instance.AddItem(item);
        }
    }

    private void LoadWeapons(SaveData data)
    {
        foreach (WeaponData weaponData in data.Weapons)
        {
            Weapon weapon = InventoryManager.Instance.Weapons.transform.GetChild(weaponData.Index).GetComponent<Weapon>();
            weapon.CurrentAmmoInClip = weaponData.MagazineCount;
            weapon.IsAvailable = weaponData.IsAvailable;
            if (weaponData.IsEquipped) weapon.gameObject.SetActive(true);
            else weapon.gameObject.SetActive(false);
        }
    }

    private void LoadAmmo(SaveData data)
    {
        Ammo ammoSlot = Player.GetComponent<Ammo>();
        ammoSlot.ResetAmmo();

        foreach (Item item in InventoryManager.Instance.Items)
        {
            if (item.Type == ItemType.Ammo)
            {
                ammoSlot.IncreaseCurrentAmmo(item.AmmoType, item.Quantity);
            }
        }

        foreach (Weapon weapon in InventoryManager.Instance.Weapons.transform.GetComponentsInChildren<Weapon>(true))
        {
            if (weapon.IsAvailable)
            {
                ammoSlot.IncreaseCurrentAmmo(weapon.AmmoType, weapon.CurrentAmmoInClip);
            }

            if (weapon.IsEquipped)
            {
                int inventoryAmmo = InventoryManager.Instance.GetInventoryAmmo(weapon.AmmoType);
                Player.SetBulletsUI(weapon.CurrentAmmoInClip, inventoryAmmo);
            }
        }
    }

    private void LoadEnemies(SaveData data)
    {
        Enemy[] enemiesPool = EnemyPool.GetComponentsInChildren<Enemy>(true);
        Mannequin[] mannequinsPool = EnemyPool.GetComponentsInChildren<Mannequin>(true);

        foreach (EnemyData enemyData in data.Enemies)
        {
            foreach (Enemy enemy in enemiesPool)
            {
                if (enemy.GetComponent<UniqueIDComponent>().UniqueID == enemyData.UniqueID)
                {
                    enemy.HealthPoints = enemyData.Health;
                    enemy.IsDead = enemyData.IsDead;
                    enemy.transform.position = new Vector3(enemyData.Position[0], enemyData.Position[1], enemyData.Position[2]);
                    enemy.transform.rotation = Quaternion.Euler(enemyData.Rotation[0], enemyData.Rotation[1], enemyData.Rotation[2]);
                    if (enemy.IsDead)
                    {
                        enemy.gameObject.SetActive(false);
                    }
                    else
                    {
                        enemy.gameObject.SetActive(true);
                    }
                    if (!enemyData.IsActive) enemy.gameObject.SetActive(false);
                }
            }

            foreach (Mannequin mannequin in mannequinsPool)
            {
                if (mannequin.GetComponent<UniqueIDComponent>().UniqueID == enemyData.UniqueID)
                {
                    mannequin.IsDead = enemyData.IsDead;
                    mannequin.transform.position = new Vector3(enemyData.Position[0], enemyData.Position[1], enemyData.Position[2]);
                    mannequin.transform.rotation = Quaternion.Euler(enemyData.Rotation[0], enemyData.Rotation[1], enemyData.Rotation[2]);
                    if (mannequin.IsDead)
                    {
                        mannequin.gameObject.SetActive(false);
                    }
                    else if (enemyData.IsActive)
                    {
                        mannequin.gameObject.SetActive(true);
                    }
                    else mannequin.gameObject.SetActive(false);
                }
            }
        }
    }

    
    private void LoadInteractableObjects(SaveData data)
    {
        InteractableObject[] intObjectsPool = InteractableObjectPool.GetComponentsInChildren<InteractableObject>();
        Duplicate[] duplicatesPool = InteractableObjectPool.GetComponentsInChildren<Duplicate>();

        foreach (InteractableObjectData intObjectsData in data.InteractableObjects)
        {
            foreach (InteractableObject pickupObject in intObjectsPool)
            {
                if (pickupObject.GetComponent<UniqueIDComponent>().UniqueID == intObjectsData.UniqueID)
                {
                    pickupObject.IsActive = intObjectsData.IsActive;
                    if (!pickupObject.IsActive)
                    {
                        // Move the object to a position far away
                        pickupObject.transform.position = new Vector3(0, -1000f, 0);
                    }
                }
            }

            int count = 0;

            foreach (InteractableObject obj in intObjectsPool)
            {                    
                if (obj is HorrorDollCode horrorDoll)
                {
                    if (!horrorDoll.IsActive)
                    {
                        count++;
                    }
                }
            }

            InventoryManager.Instance.HorrorDollCount.text = count.ToString();

            foreach (Duplicate duplicate in duplicatesPool)
            {
                if (duplicate.DuplicateObject.GetComponent<UniqueIDComponent>().UniqueID == intObjectsData.UniqueID)
                {
                    duplicate.DuplicateObject.GetComponent<InteractableObject>().IsActive = intObjectsData.IsActive;
                    if (!duplicate.DuplicateObject.GetComponent<InteractableObject>().IsActive)
                    {
                        // Move the object to a position far away
                        duplicate.DuplicateObject.transform.position = new Vector3(0, -1000f, 0);
                    }
                }
            }
        }
    }

    private void LoadInteractableStaticObjects(SaveData data)
    {
        Drawer[] intObjPool;

        foreach (Transform staticObject in _interactStaticObjectPool.GetComponentsInChildren<Transform>())
        {
            intObjPool = staticObject.GetComponentsInChildren<Drawer>();

            foreach (InteractStaticObjectData intObjData in data.InteractStaticObjects)
            {
                foreach (Drawer drawer in intObjPool)
                {
                    if (drawer.GetComponent<UniqueIDComponent>().UniqueID == intObjData.UniqueID)
                    {
                        drawer.IsOpen = intObjData.IsOpen;
                        if (drawer.IsDrawer && intObjData.IsOpen) drawer.transform.position = new Vector3(intObjData.Position[0], intObjData.Position[1], intObjData.Position[2]);
                        else if (intObjData.IsOpen) drawer.transform.rotation = Quaternion.Euler(intObjData.Rotation[0], intObjData.Rotation[1], intObjData.Rotation[2]);
                    }
                }
            }
        }
    }

    private void LoadDoorObjects(SaveData data)
    {
        foreach (DoorObjectData doorData in data.Doors)
        {
            foreach (Duplicate door in _doorObjectPool.GetComponentsInChildren<Duplicate>())
            {
                if (door.DuplicateObject.GetComponent<UniqueIDComponent>().UniqueID == doorData.UniqueID)
                {
                    Door doorScript = door.DuplicateObject.GetComponent<Door>();
                    doorScript.IsLocked = doorData.IsLocked;
                    if (_debugMode) Debug.Log("GameManager.cs: Door " + door.DuplicateObject.name + " is " + (doorData.IsLocked ? "locked" : "unlocked") + " and " + (doorData.IsOpen ? "open" : "closed"));
                    if (doorData.IsOpen) doorScript.OpenDoor(false);
                }
            }
        }
    }

    private void LoadTextObjects(SaveData data)
    {
        foreach (TextObjectData textData in data.TextObjects)
        {
            foreach (TextCode textCode in _textObjectPool.GetComponentsInChildren<TextCode>())
            {
                if (textCode.GetComponent<UniqueIDComponent>().UniqueID == textData.UniqueID)
                {
                    textCode.IsAudioActive = textData.IsActive;
                }
            }
        }

        foreach (Duplicate duplicate in _textObjectPool.GetComponentsInChildren<Duplicate>())
        {
            foreach (TextObjectData textData in data.TextObjects)
            {
                if (duplicate.DuplicateObject.GetComponent<UniqueIDComponent>().UniqueID == textData.UniqueID)
                {
                    TextCode textCode = duplicate.DuplicateObject.GetComponent<TextCode>();
                    textCode.IsAudioActive = textData.IsActive;
                }
            }
        }
    }

    private void LoadSummonObjects(SaveData data)
    {
        foreach (SummonObjectData summonData in data.SummonObjects)
        {
            foreach (SummonObject summonObject in _summonObjectPool.GetComponentsInChildren<SummonObject>())
            {
                if (summonObject.GetComponent<UniqueIDComponent>().UniqueID == summonData.UniqueID)
                {
                    summonObject.IsObjectPlaced = summonData.IsObjectPlaced;
                    if (summonData.IsObjectPlaced) summonObject.LoadPlacedObject();
                }
            }
        }
    }

    private void LoadEventData(SaveData data)
    {
        foreach (SavedEventData savedEventData in data.Events)
        {
            foreach (EventDataEntry eventDataEntry in EventData.eventDataEntries)
            {
                if (eventDataEntry.EventName == savedEventData.EventName)
                {
                    if (savedEventData.Active)
                    {
                        eventDataEntry.Active = true;
                        EventData.TriggerEvent(eventDataEntry.EventName);
                    }
                }
            }
        }
    }

    private void LoadVariousData(SaveData data)
    {
        AudioManager.Instance.SetAudioClip(AudioManager.Instance.Environment, data.VariousData.EnvironmentMusicClip);
        AudioManager.Instance.PlayAudio(AudioManager.Instance.Environment, 0.1f, 1f, true);
        Player.IsLightAvailable = data.VariousData.IsFlashlightEnabled;
        if (Player.IsLightAvailable) Player.LightSwitch();
    }

    private void LoadAutoSaveData(SaveData data)
    {
        foreach (AutoSaveData autoSaveData in data.AutoSaveData)
        {
            foreach (AutoSave autoSave in _autoSavePool.GetComponentsInChildren<AutoSave>())
            {
                if (autoSave.GetComponent<UniqueIDComponent>().UniqueID == autoSaveData.UniqueID)
                {
                    if (!autoSaveData.IsActive)
                    {
                        autoSave.IsActive = false;
                        autoSave.GetComponent<Collider>().enabled = false;
                    }
                }
            }
        }
    }

    #endregion
    */




    #region Helper Methods

    public void ToggleTeleport(bool turnOnOrOff)
    {
        foreach (Transform controller in LeftController.GetComponentsInChildren<Transform>())
        {
            if (controller.CompareTag("Teleport"))
            {
                controller.gameObject.SetActive(turnOnOrOff);
            }
        }

        foreach (Transform controller in RightController.GetComponentsInChildren<Transform>())
        {
            if (controller.CompareTag("Teleport"))
            {
                controller.gameObject.SetActive(turnOnOrOff);
            }
        }
    }
    /*
    private Item CreateItemFromData(ItemData itemData)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.DisplayName = itemData.DisplayName;
        newItem.Description = itemData.Description;
        newItem.IsUnique = itemData.IsUnique;
        newItem.Quantity = itemData.Quantity;
        newItem.Type = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.Type);
        newItem.AmmoType = (Ammo.AmmoType)System.Enum.Parse(typeof(Ammo.AmmoType), itemData.AmmoType);
        newItem.PotionType = (Potion.PotionType)System.Enum.Parse(typeof(Potion.PotionType), itemData.PotionType);
        newItem.IconPath = itemData.IconPath;
        newItem.Icon = Resources.Load<Sprite>(itemData.IconPath);
        newItem.PrefabPath = itemData.PrefabPath;
        newItem.Prefab = Resources.Load<GameObject>(itemData.PrefabPath);

        return newItem;
    }
    */
    #endregion
}