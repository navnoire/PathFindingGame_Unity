//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using NavnoireCoding.Utils.Enums;
//using System;

//public class GameManager : Singleton<GameManager>
//{
//    // Знать текущий уровень, на котором мы находимся
//    // Загружать и выгружать уровни
//    // Следить за текущим статусом игры
//    // Генерировать другие персистент менеджеры


//    [SerializeField]
//    private int _currentLevelIndex;

//    // Учитывает все асинхронные операции загрузки левела, в этом списке - не завершена. по завершении загрузки удаляется
//    private List<AsyncOperation> _loadOperations;
//    private List<GameObject> _instancedSystemPrefabs = new List<GameObject>();

//    private GameState _currentGameState = GameState.PREGAME;
//    public GameState CurrentGameState
//    {
//        get { return _currentGameState; }
//        private set { UpdateState(value); }
//    }
//    public GameObject[] systemPrefabs;

//    private void Start()
//    {
//        DontDestroyOnLoad(gameObject);

//        _loadOperations = new List<AsyncOperation>();

//        InstantiateSystemPrefabs();

//        Events.Instance.OnMainMenuFadeComplete += HandleMainMenuFadeComplete;
//    }

//    private void Update()
//    {
//        if (_currentGameState == GameState.PREGAME) return;

//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            TogglePausestate();
//        }
//    }

//    private void LoadScene(int sceneIndex)
//    {
//        AsyncOperation ao = SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive);
//        if (ao == null)
//        {
//            Debug.LogError("[Game Manager] Unable load scene with index " + sceneIndex);
//            return;
//        }

//        ao.completed += OnLoadOperationComplete;
//        _loadOperations.Add(ao);
//        _currentLevelIndex = (int)sceneIndex;
//    }

//    private void UnloadScene(int sceneIndex)
//    {
//        AsyncOperation ao = SceneManager.UnloadSceneAsync((int)sceneIndex);
//        if (ao == null)
//        {
//            Debug.LogError("[Game Manager] Unable unload scene with index " + sceneIndex);
//            return;
//        }
//        ao.completed += OnUnloadOperationComplete;
//    }

//    void InstantiateSystemPrefabs()
//    {
//        GameObject obj;
//        for (int i = 0; i < systemPrefabs.Length; i++)
//        {
//            obj = Instantiate(systemPrefabs[i]);
//            _instancedSystemPrefabs.Add(obj);
//        }
//    }

//    void OnLoadOperationComplete(AsyncOperation ao)
//    {
//        if (_loadOperations.Contains(ao))
//        {
//            _loadOperations.Remove(ao);

//            if (_loadOperations.Count == 0)
//            {
//                UpdateState(GameState.RUNNING);
//            }
//        }
//        Debug.Log("Load complete.");
//    }

//    void OnUnloadOperationComplete(AsyncOperation ao)
//    {
//        Debug.Log("Unload complete.");
//    }

//    private void UpdateState(GameState state)
//    {
//        GameState prevGameState = _currentGameState;
//        _currentGameState = state;

//        switch (_currentGameState)
//        {
//            case GameState.PREGAME:
//                Time.timeScale = 1.0f;
//                break;
//            case GameState.RUNNING:
//                Time.timeScale = 1.0f;
//                break;
//            case GameState.PAUSED:
//                Time.timeScale = 0.0f;
//                break;
//            default:
//                break;
//        }

//        Events.Instance.OnGameStateChanged?.Invoke(_currentGameState, prevGameState);
//    }

//    public void StartGame()
//    {
//        LoadScene(SceneIndexEnum.Game);
//    }

//    public void RestartGame()
//    {
//        UpdateState(GameState.PREGAME);
//    }

//    public void QuitGame()
//    {
//        //autosave
//        //cleaning
//        //other features for quiting

//        Application.Quit();
//    }

//    public void TogglePausestate()
//    {
//        UpdateState(_currentGameState == GameState.PAUSED ? GameState.RUNNING : GameState.PAUSED);
//    }

//    private void HandleMainMenuFadeComplete(bool isFadeOut)
//    {
//        if (!isFadeOut)
//        {
//            UnloadScene((SceneIndexEnum)_currentLevelIndex);
//        }
//    }

//    protected override void OnDestroy()
//    {
//        base.OnDestroy();

//        Events.Instance.OnMainMenuFadeComplete -= HandleMainMenuFadeComplete;

//        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
//        {
//            Destroy(_instancedSystemPrefabs[i]);
//        }
//        _instancedSystemPrefabs.Clear();
//    }
//}
