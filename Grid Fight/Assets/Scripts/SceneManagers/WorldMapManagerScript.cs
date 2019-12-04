using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldMapManagerScript : MonoBehaviour
{
    public static WorldMapManagerScript Instance;

    public MatchType MatchInfoType;
    public List<WorldMapArenaClass> Arenas = new List<WorldMapArenaClass>();
    public WorldMapSaveClass WorldMapSave;

    #region SwitchVariables
#if UNITY_SWITCH && !UNITY_EDITOR
    private nn.account.Uid userId;
#endif
    private const string Switch_MountName = "MySave";
    private const string Switch_FileName = "WorldMapProgress.xml";
    private const string PlayerPref_Name = "WorldMapProgress";
    private string filePath;
#if UNITY_SWITCH && !UNITY_EDITOR
    private nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
#endif
    private const int saveDataSize = 8;
    private int saveData = 0;
    private int loadData = 0;
#if UNITY_SWITCH && !UNITY_EDITOR
    private nn.Result result;
#endif

    #endregion

    private void Awake()
    {
        Instance = this;
    }
   
    public void SetupWorldMap()
    {
        WorldMapSave = null;
        
        if (WorldMapSave == null)
        {
            WorldMapSave = new WorldMapSaveClass();
            Debug.Log("---------------------- empty");
            for (int i = 0; i < 10; i++)
            {
                WorldMapSave.arenas.Add(new WorldMapArenaSaveClass(i, i == 0 ? true : false));
            }

#if UNITY_SWITCH && !UNITY_EDITOR
        SaveSwitch();
#elif UNITY_EDITOR
        Save();
#endif

        }

#if UNITY_SWITCH && !UNITY_EDITOR
        LoadSwitch();

#elif UNITY_EDITOR
        Load();
#endif
        
        for (int i = 0; i < Arenas.Count; i++)
        {
            Arenas[i].Arena.ArenaBtn.interactable = WorldMapSave.arenas.Where(r => r.Id == Arenas[i].Id).First().isArenaCompleted;
        }

        LoaderManagerScript.Instance.MainCanvasGroup.alpha = 0;
    }

    public void GoToArena(int id)
    {
        LoaderManagerScript.Instance.PlayerBattleInfo = Arenas.Where(r=> r.Id == id).First().PlayerBattleInfo;
        LoaderManagerScript.Instance.MatchInfoType = MatchInfoType;
        LoaderManagerScript.Instance.LoadNewSceneWithLoading("BattleScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void Start()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        nn.account.Account.Initialize();
        result = nn.fs.SaveData.Mount(Switch_MountName, userId);
#endif
        filePath = string.Format("{0}:/{1}", Switch_MountName, Switch_FileName);
        SetupWorldMap();

    }

    void OnDestroy()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        nn.fs.FileSystem.Unmount(Switch_MountName);
#endif
    }

    public void Save()
    {
        PlayerPrefs.SetString(PlayerPref_Name, PlaytraGamesLtd.Utils.SerializeToString<WorldMapSaveClass>(WorldMapSave));
    }

    public void Load()
    {
        WorldMapSave = PlaytraGamesLtd.Utils.DeserializeFromString<WorldMapSaveClass>(PlayerPrefs.GetString(PlayerPref_Name));
    }

    private void SaveSwitch()
    {

#if UNITY_SWITCH
        // Nintendo Switch Guideline 0080
        UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

#if UNITY_SWITCH && !UNITY_EDITOR
        // Convert the text to UTF-8-encoded bytes.
        byte[] data = Encoding.UTF8.GetBytes(PlaytraGamesLtd.Utils.SerializeToString<WorldMapSaveClass>(WorldMapSave));

        
        while (true)
        {
            // Attempt to open the file in write mode.
            result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);
            // Check if file was opened successfully.
            if (result.IsSuccess())
            {
                // Exit the loop because the file was successfully opened.
                break;
            }
            else
            {
                if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
                {
                    // Create a file with the size of the encoded data if no entry exists.
                    result = nn.fs.File.Create(filePath, data.LongLength);

                    // Check if the file was successfully created.
                    if (!result.IsSuccess())
                    {
                        Debug.LogErrorFormat("Failed to create {0}: {1}", filePath, result.ToString());
                    }
                }
                else
                {
                    // Generic fallback error handling for debugging purposes.
                    Debug.LogErrorFormat("Failed to open {0}: {1}", filePath, result.ToString());
                }
            }
        }

        // Set the file to the size of the binary data.
        result = nn.fs.File.SetSize(fileHandle, data.LongLength);

        // You do not need to handle this error if you are sure there will be enough space.
        if (nn.fs.FileSystem.ResultUsableSpaceNotEnough.Includes(result))
        {
            Debug.LogErrorFormat("Insufficient space to write {0} bytes to {1}", data.LongLength, filePath);
        }

        // NOTE: Calling File.Write() with WriteOption.Flush incurs two write operations.
        result = nn.fs.File.Write(fileHandle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);

        // The file must be closed before committing.
        nn.fs.File.Close(fileHandle);

        // Verify that the write operation was successful before committing.
        if (!result.IsSuccess())
        {
            Debug.LogErrorFormat("Failed to write {0}: {1}", filePath, result.ToString());
        }
        // This method moves the data from the journaling area to the main storage area.
        // If you do not call this method, all changes will be lost when the application closes.
        // Only call this when you are sure that all previous operations succeeded.
        nn.fs.FileSystem.Commit(Switch_MountName);
#endif
#if UNITY_SWITCH && !UNITY_EDITOR
// Stop preventing the system from terminating the game while saving.
UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif

    }

    private void LoadSwitch()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        WorldMapSave = new WorldMapSaveClass();
        // Attempt to open the file in read-only mode.
        result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
        if (!result.IsSuccess())
        {
            if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
            {
                Debug.LogFormat("File not found: {0}", filePath);
            }
            else
            {
                Debug.LogErrorFormat("Unable to open {0}: {1}", filePath, result.ToString());
            }
        }

        // Get the file size.
        long fileSize = 0;
        nn.fs.File.GetSize(ref fileSize, fileHandle);
        // Allocate a buffer that matches the file size.
        byte[] data = new byte[fileSize];
        // Read the save data into the buffer.
        nn.fs.File.Read(fileHandle, 0, data, fileSize);
        // Close the file.
        nn.fs.File.Close(fileHandle);
        // Decode the UTF8-encoded data and store it in the string buffer.
        WorldMapSave = PlaytraGamesLtd.Utils.DeserializeFromString<WorldMapSaveClass>(Encoding.UTF8.GetString(data));
#endif
    }
}

[System.Serializable]
public class WorldMapArenaClass
{
    public int Id;
    public WorldMapArenaScript Arena;
    public List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
}

public class WorldMapArenaSaveClass
{
    public int Id;
    public bool isArenaCompleted;

    public WorldMapArenaSaveClass()
    {

    }

    public WorldMapArenaSaveClass(int id, bool isarenaCompleted)
    {
        Id = id;
        isArenaCompleted = isarenaCompleted;
    }
}

public class WorldMapSaveClass
{
    public List<WorldMapArenaSaveClass> arenas = new List<WorldMapArenaSaveClass>();

    public WorldMapSaveClass()
    {

    }
}