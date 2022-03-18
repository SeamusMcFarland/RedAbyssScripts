using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    GameObject[] computers;
    List<ComputerScript> computersS = new List<ComputerScript>();
    GameObject player;
    Rigidbody playerRB;
    int savedAccess;
    bool[] tankAccess;
    GameObject[] gasTanks;
    List<GasTankScript> gasTanksS = new List<GasTankScript>();
    int compAquire; //used to make sure computer scripts are ordered
    int tankAquire;
    bool originalManager;
    bool setup;
    public int levelNum; // 0 is main menu, -50 is music credits
    int lastLevel;
    bool newScene;

    int difficulty;
    public DifficultyScript difficultyS;
    int musicVolume;
    VolumeScript musicVolumeS;

    AudioSource musicSource;

    public TextAsset[] allTextFiles; // technically only all text files used in the current scene

    bool fromNewGame;
    UIManagerScript uimanagerS;
    bool volumeSetup;

    const bool CHEATS_ENABLED = true;

    Text loadingText;

    private void Start()
    {
        tankAccess = new bool[]{false, false, false, false, false, false};
        Setup();
        //Debug.developerConsoleVisible = true;
        //Debug.LogError("...");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (CHEATS_ENABLED)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                for (int i = 0; i < tankAccess.Length; i++)
                    tankAccess[i] = true;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Setup();
    }

    public void Setup()
    {
        //Debug.LogError("setup originally called");
        if (!setup) // in case if called multiple times
        {
            if (levelNum != -50) // doesn't run if on music credits scene
            {
                loadingText = GameObject.FindGameObjectWithTag("loadingtext").GetComponent<Text>();
                musicSource = GameObject.Find("MusicManager").GetComponent<AudioSource>();
                musicSource.ignoreListenerPause = true;
            }
            //Debug.LogError("doing setup");



            if (levelNum != 0 && levelNum != -50) // doesn't run if on main menu or music credits scene
            {
                musicSource.volume = ((float)musicVolume) / 10f; // can't be on main menu since will otherwise set to 0 since start hasn't been initiated on volume script

                //Debug.LogError("not on main menu");
                uimanagerS = GameObject.FindWithTag("uimanager").GetComponent<UIManagerScript>();
                if (fromNewGame)
                {
                    //Debug.LogError("introfade method call");
                    uimanagerS.IntroFade();
                }
                else
                    uimanagerS.RemoveFade();

                if (levelNum == 100 && fromNewGame) // if tutorial and from new game
                {
                    GameObject.Find("TutorialExit").GetComponent<SceneTriggerScript>().SetToNewGame(GetReadString("SaveFile"));
                }
                fromNewGame = false;

                if (lastLevel != levelNum) // in case if a new level is loaded, saved access is wiped
                {
                    savedAccess = 0;
                    for (int i = 0; i < tankAccess.Length; i++)
                    {
                        tankAccess[i] = false;
                    }
                }

                if (difficulty < 1 || difficulty > 6) // checks if difficulty has not been set and defaults to 3
                    difficulty = 3;

                setup = true;
                computers = null;
                player = null;
                computersS.Clear();

                player = GameObject.FindGameObjectWithTag("player");
                playerRB = player.GetComponent<Rigidbody>();
                player.GetComponent<PlayerScript>().SetDifficulty(difficulty);
                computers = GameObject.FindGameObjectsWithTag("computer");
                compAquire = 0;
                for (int i = 0; i < computers.Length; i++) // orders and gets computer scripts
                {
                    for (int j = 0; j < computers.Length; j++)
                    {
                        if (computers[j].GetComponent<ComputerScript>().GetCompNum() == i + 1)
                        {
                            computersS.Add(computers[j].GetComponent<ComputerScript>());
                            break;
                        }
                    }
                }
                for (int i = 0; i < savedAccess; i++)
                {
                    computersS[i].PreAccess();
                }

                gasTanksS.Clear();
                gasTanks = GameObject.FindGameObjectsWithTag("gastank");
                tankAquire = 0;
                for (int i = 0; i < gasTanks.Length; i++) // orders and gets gastank scripts
                {
                    for (int j = 0; j < gasTanks.Length; j++)
                    {
                        if (gasTanks[j].GetComponent<GasTankScript>().GetGasTankNum() == i + 1)
                        {
                            gasTanksS.Add(gasTanks[j].GetComponent<GasTankScript>());
                            break;
                        }
                    }
                }
                for (int i = 0; i < tankAccess.Length; i++)
                {
                    if (tankAccess[i])
                        gasTanksS[i].PreAccess();
                }


                //starting player position
                if (levelNum == 1)
                {
                    if (savedAccess > 3) // when player has reached the boss (four doors)
                    {
                        playerRB.MovePosition(new Vector3(50.7f, player.transform.position.y, -13.1f));
                    }
                    else if (savedAccess > 1) // when player has gotten access to two doors
                    {
                        playerRB.MovePosition(new Vector3(16f, player.transform.position.y, -11f));
                    }
                    else if (savedAccess == 1) // when player has gotten access to one door
                    {
                        playerRB.MovePosition(new Vector3(-5.8f, player.transform.position.y, 9.5f));
                    }
                    else
                    {
                        playerRB.MovePosition(new Vector3(-3.9f, player.transform.position.y, -13.3f));
                    }
                }
                else if (levelNum == 2)
                {
                    if(tankAccess[1] && tankAccess[2] && tankAccess[3])
                        playerRB.MovePosition(new Vector3(-43f, player.transform.position.y, -13f));
                    else if(tankAccess[1] && tankAccess[2])
                        playerRB.MovePosition(new Vector3(-8.11f, player.transform.position.y, -46.35f));
                }
                else if (levelNum == 3)
                {
                    if (savedAccess > 3) // when player has reached the boss (four doors)
                    {
                        playerRB.MovePosition(new Vector3(50f, player.transform.position.y, 20f));
                    }
                    else if (savedAccess > 1) // when player has gotten access to two doors
                    {
                        playerRB.MovePosition(new Vector3(35.5f, player.transform.position.y, -34.7f));
                    }
                    else if (savedAccess > 0) // when player has gotten access to one door
                    {
                        playerRB.MovePosition(new Vector3(11.5f, player.transform.position.y, -43.5f));
                    }
                    else
                    {
                        playerRB.MovePosition(new Vector3(-3.9f, player.transform.position.y, -12.5f));
                    }
                }
                else if (levelNum == 4)
                {
                    if(!newScene)
                        playerRB.MovePosition(new Vector3(-9.7f, player.transform.position.y, 102.9f));
                }

            }
            else if(levelNum == 0)
            {
                difficultyS = GameObject.Find("DifficultyText").GetComponent<DifficultyScript>();
                musicVolumeS = GameObject.Find("MusicVolumeText").GetComponent<VolumeScript>();
                if (!volumeSetup) // need this since will retrieve 0 instead of 10 on startup
                {
                    musicVolume = 10;
                    musicVolumeS.SetVolume(10);
                    volumeSetup = true;
                }
                else
                    musicVolumeS.SetVolume(musicVolume);
                print("aquired volume: " + musicVolumeS.GetVolume());
                musicSource.volume = ((float)musicVolume) / 10f;
                setup = true;
            }
        }
    }


    public string GetReadString(string name)
    {
        return ReadString(name);
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (originalManager || GameObject.FindGameObjectsWithTag("scenemanager").Length < 2)
            originalManager = true;
        else
            Destroy(this.gameObject);
    }

    public void LoadNewScene(string s)
    {
        if (GetScene() != -50)
            loadingText.enabled = true;
        setup = false;
        Time.timeScale = 1f;

        if (levelNum == 0) // checks if the main menu is open, and if so gets the updated difficulty and volumes
        {
            difficulty = difficultyS.GetDifficulty();
            musicVolume = musicVolumeS.GetVolume();
        }
        lastLevel = levelNum;

        if (!(s.Equals("Reload")))
            fromNewGame = true;

        if (s.Equals("Level1"))
            levelNum = 1;
        else if (s.Equals("Level2"))
            levelNum = 2;
        else if (s.Equals("Level3"))
            levelNum = 3;
        else if (s.Equals("Level4"))
            levelNum = 4;
        else if (s.Equals("Cutscene1"))
            levelNum = -1;
        else if (s.Equals("Cutscene2"))
            levelNum = -2;
        else if (s.Equals("Cutscene3"))
            levelNum = -3;
        else if (s.Equals("MainMenu"))
            levelNum = 0;
        else if (s.Equals("Tutorial"))
            levelNum = 100;
        else if (!(s.Equals("Reload")))
            print("ERROR! MISSING INSTRUCTIONS FOR REQUESTED SCENE");

        if (s.Equals("Reload"))
        {
            newScene = false;
            SceneManager.LoadScene(GetSceneName());
        }
        else
        {
            newScene = true;
            /*if (s.Equals("Level2"))
            {
                if (ReadString("SaveFile") == "Level1")
                    SaveLevel("Level2");
            }
            else if (s.Equals("Level3"))
            {
                if (ReadString("SaveFile") == "Level1" || ReadString("SaveFile") == "Level2")
                    SaveLevel("Level3");
            }
            else if (s.Equals("Level4"))
            {
                if (ReadString("SaveFile") == "Level1" || ReadString("SaveFile") == "Level2" || ReadString("SaveFile") == "Level3")
                    SaveLevel("Level4");
            }*/
            SceneManager.LoadScene(s);
        }
    }

    static string textString;

    public void SaveLevel(string s)
    {
        textString = s;
        //WriteTextFile("SaveFile", s);
    }

    public void SaveProgress(int n)
    {
        if (savedAccess < n)
        {
            savedAccess = n;
        }
    }

    public void TankSaveProgress(int n)
    {
        //for(int i = 0; i < tankAccess.Length; i++) // for testing
            //tankAccess[i] = true;
        tankAccess[n - 1] = true;
    }

    public bool CheckTankFinished() // checks if all tanks have been accessed
    {
        bool finished = true;
        for (int i = 0; i < tankAccess.Length; i++)
        {
            if (!tankAccess[i])
                finished = false;
        }
        return finished;
    }

    public int GetProgress()
    {
        return savedAccess;
    }

    public int GetTankSaveProgress()
    {
        int temp = 0;
        foreach (bool b in tankAccess)
        {
            if (b)
                temp++;
        }
        return temp;
    }

    public bool GetSpecificTankSaveProgress(int n)
    {
        return tankAccess[n - 1]; // has to be -1 since 0 is noneffective default
    }

    public int GetScene()
    {
        return levelNum;
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool GetNewScene()
    {
        return newScene;
    }

    /*static void WriteString(string textFileName)
    {
        string path = "Assets/TextFiles/" + textFileName + ".txt";

        StreamWriter writer = new StreamWriter(path, true);
        File.WriteAllText(@"Assets/TextFiles/" + textFileName, string.Empty); // clears all in text file before writing
        writer.WriteLine(textString); // writes the entered text to the text file
        writer.Close();

        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset)Resources.Load(textFileName);

        Debug.Log(asset.text);
    }*/

    /*private void WriteTextFile(string writeFile, string text)
    {
        File.WriteAllText(Application.persistentDataPath + "/" + writeFile + ".txt", text);
    }*/

    /*[MenuItem("Tools/Write file")]
    static void WriteString(string textFileName)
    {
        string path = "Assets/TextFiles/" + textFileName + ".txt";

        StreamWriter writer = new StreamWriter(path, true);
        File.WriteAllText(@"Assets/TextFiles/" + textFileName, string.Empty); // clears all in text file before writing
        writer.WriteLine(textString); // writes the entered text to the text file
        writer.Close();

        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset)Resources.Load(textFileName);

        Debug.Log(asset.text);
    }

    [MenuItem("Tools/Read file")]
    static void ReadString(string textFileName)
    {
        string path = "Assets/TextFiles/" + textFileName + ".txt";

        StreamReader reader = new StreamReader(path);
        textString = reader.ReadToEnd();

        reader.Close();
    }*/

    /*public void WriteString(string textFileName, string toWrite)
    {
        if (File.Exists(textFileName))
        {
            string path = "Assets/TextFiles/" + textFileName + ".txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(toWrite);
        }
        else
            print("ERROR! CAN'T FIND TEXT FILE!");
    }*/

    public string ReadString(string textFileName)
    {
        print("read string using textfilename BEFORE: " + textFileName);
        TextAsset txt = (TextAsset)Resources.Load(textFileName, typeof(TextAsset));
        print("read string using textfilename AFTER: " + textFileName);
        return txt.text;
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    /*
    public string ReadString(string textFileName)
    {
        foreach (TextAsset t in allTextFiles) // searches for the requested text file and sets it to selected file
        {
            if (t.name == textFileName)
            {
                selectedFile = t;
                break;
            }
        }
        return selectedFile.text;
    }
    */
}
