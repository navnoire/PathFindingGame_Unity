using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using NavnoireCoding.Utils.Enums;
using NavnoireCoding.Utils;

namespace NavnoireCoding.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private GameState _currentGameState = GameState.PREGAME;
        public GameState CurrentGameState
        {
            get { return _currentGameState; }
            private set { _currentGameState = value; }
        }

        private List<AsyncOperation> _loadOperations;
        private List<GameObject> _instantiatedSystemPrefabs = new List<GameObject>();

        public GameObject[] systemPrefabs;

        public LevelData_SO levelData;
        public GameProps_SO currentGameProps;

        //SafePlayerPrefs spp;

        [HideInInspector] public SerializationScript serializator;


        protected override void Awake()
        {
            base.Awake();

            //spp = new SafePlayerPrefs("TrailsGame", "PlayerID", "Progress");
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            _loadOperations = new List<AsyncOperation>();
            InstantiateSystemPrefabs();

            serializator = GetComponent<SerializationScript>();

            Events.Instance.OnMainMenuFadeComplete += HandleMainMenuFade;

            #region Player Prefs Integrity Check
            ////PlayerPrefs.DeleteAll();
            //// если игрока нет(первый запуск на устройстве) - создать(GameState ассет в памяти и прикрепить к ассет файлу в папке)
            //if (!PlayerPrefs.HasKey("PlayerID"))
            //{
            //    print("No playerID key found. Generating new");
            //    CreateNewPlayer();
            //}

            //// Если существует игрок - прочитать состояние из его файла с проверкой на целостность
            //else
            //{
            //    if (!spp.HasBeenEdited())
            //    {
            //        print("Prefs integrity is true");
            //        GameState_SO savedGame = serializator.LoadGameState(PlayerPrefs.GetString("PlayerID"));
            //        if (savedGame != null)
            //        {
            //            currentGamestate.Assign(savedGame);
            //            Destroy(savedGame);
            //        }

            //    }
            //    else
            //    {
            //        print("Prefs were edited");
            //        CreateNewPlayer();
            //    }
            //}
            #endregion

        }

        //void CreateNewPlayer()
        //{
        //    string newID = Guid.NewGuid().ToString("N");
        //    PlayerPrefs.SetString("PlayerID", newID);
        //    PlayerPrefs.SetString("Progress", Convert.ToString(0));
        //    spp.Save();

        //    currentGamestate.checksum = PlayerPrefs.GetString("CHECKSUM");
        //    currentGamestate.playerID = newID;
        //    currentGamestate.currentLevelIndex = 0;
        //    serializator.SaveGameState(currentGamestate);
        //}

        private void UpdateState(GameState state)
        {
            var prevGameState = _currentGameState;
            CurrentGameState = state;

            switch (CurrentGameState)
            {
                case GameState.PREGAME:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.RUNNING:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.PAUSED:
                    Time.timeScale = 0.0f;
                    break;
            }

            Events.Instance.OnGameStateChanged?.Invoke(prevGameState, _currentGameState);
        }

        private void InstantiateSystemPrefabs()
        {
            GameObject obj;
            for (int i = 0; i < systemPrefabs.Length; i++)
            {
                obj = Instantiate(systemPrefabs[i]);
                _instantiatedSystemPrefabs.Add(obj);
            }
        }

        private void ClearInstantiatedSystemPrefabs()
        {
            foreach (var obj in _instantiatedSystemPrefabs)
            {
                Destroy(obj);
            }
            _instantiatedSystemPrefabs.Clear();
        }

        private void OnApplicationQuit()
        {
            if (levelData != null)
            {
                levelData.ClearPuzzleStartData();

            }
            serializator.SaveGameState(currentGameProps);
            serializator.ClearLevelData();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearInstantiatedSystemPrefabs();

        }

        #region *----------Loading and unloading scenes----------*
        private void LoadScene(String sceneName)
        {
            if (levelData != null)
            {
                levelData.ClearPuzzleStartData();

            }
            serializator.ClearLevelData();

            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (ao == null)
            {
                Debug.LogError("[GameManager] Unable to load scene with name " + sceneName);
                return;
            }
            ao.completed += OnLoadOperationComplete;
            _loadOperations.Add(ao);
        }

        private void UnloadScene(string sceneName)
        {
            AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneName);
            if (ao == null)
            {
                Debug.LogError("[Game Manager] Unable to unload scene with name " + sceneName);
                return;
            }
            ao.completed += OnUnloadOperationComplete;
        }

        private void OnLoadOperationComplete(AsyncOperation ao)
        {
            if (_loadOperations.Contains(ao))
            {
                _loadOperations.Remove(ao);

                if (_loadOperations.Count == 0)
                {
                    UpdateState(GameState.RUNNING);
                }
            }
        }

        private void OnUnloadOperationComplete(AsyncOperation ao)
        {
            Debug.Log("[Game Manager] Unload complete.");

        }

        #endregion


        public void SetLevelData(LevelData_SO data)
        {
            levelData = data;
        }

        public void StartGame(string level)
        {
            LoadScene(level);
        }

        public void ReplayGame()
        {
            Debug.Log("[Game Manager] Replay mechanism is not implemented yet.");
        }

        public void BackToMainMenu()
        {
            UpdateState(GameState.PREGAME);
        }

        private void HandleMainMenuFade(bool isFadeout)
        {
            if (!isFadeout)
            {
                UnloadScene("Level");
            }
        }

        public void TogglePauseState()
        {
            UpdateState(CurrentGameState == GameState.PAUSED ? GameState.RUNNING : GameState.PAUSED);

        }

    }
}
