using System.Collections;
using System.Collections.Generic;
using NavnoireCoding.Utils;
using NavnoireCoding.Utils.Enums;
using UnityEngine;

namespace NavnoireCoding.Managers
{

    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private IngameMenu _ingameMenu;
        private MainMenu _mainMenu;
        private Camera _uiCamera;
        void Start()
        {
            Events.Instance.OnGameStateChanged += HandleGameStateChanged;
            _mainMenu = GetComponentInChildren<MainMenu>();
            _uiCamera = GetComponentInChildren<Camera>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUICameraActive(bool isActive)
        {
            _uiCamera.gameObject.SetActive(isActive);
        }

        private void HandleGameStateChanged(GameState prevState, GameState newState)
        {
            _ingameMenu.gameObject.SetActive(newState == GameState.RUNNING);
            _pauseMenu.gameObject.SetActive(newState == GameState.PAUSED);

            switch (newState)
            {
                case GameState.PAUSED:
                    _pauseMenu.FadeIn();
                    break;
                case GameState.RUNNING:
                    if (prevState == GameState.PAUSED) { _pauseMenu.FadeOut(); }
                    if (prevState == GameState.PREGAME) { _mainMenu.FadeOut(); }
                    break;
                case GameState.PREGAME:
                    _mainMenu.gameObject.SetActive(true);
                    _mainMenu.FadeIn();
                    break;
            }

            //if (prevState != GameState.PREGAME && newState == GameState.PREGAME)
            //{
            //    _mainMenu.gameObject.SetActive(true);
            //    _mainMenu.FadeIn();
            //}

            //if (prevState == GameState.PREGAME && newState == GameState.RUNNING)
            //{
            //    _mainMenu.FadeOut();
            //}
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //TODO: проверить, точно ли надо отписываться от этого события в этом методе?
            if (Events.IsInitialized) { Events.Instance.OnGameStateChanged -= HandleGameStateChanged; }
        }
    }
}

//public class UIManager : Singleton<UIManager>
//{
//    private MainMenu _mainMenu;
//    [SerializeField] private IngameMenu _ingameMenu;
//    [SerializeField] private PauseMenu _pauseMenu;
//    [SerializeField] private Camera _dummyCamera;

//    private void Start()
//    {
//        _mainMenu = GetComponentInChildren<MainMenu>();
//        Events.Instance.OnGameStateChanged += HandleGameStateChanged;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0) && GameManager.Instance.CurrentGameState == GameState.PREGAME)
//        {
//            GameManager.Instance.StartGame();
//        }

//        //TODO: Удалить, это блок для тестирования входных данных от мыши
//        if (Input.GetMouseButtonDown(0) && GameManager.Instance.CurrentGameState == GameState.RUNNING)
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit))
//            {
//                Renderer r = hit.transform.GetComponent<Renderer>();
//                if (r != null)
//                {
//                    r.material.color = Random.ColorHSV();
//                }
//            }
//        }
//    }

//    public void SetUICameraActive(bool active)
//    {
//        _dummyCamera.gameObject.SetActive(active);
//    }

//    private void HandleGameStateChanged(GameState incomingState, GameState prevState)
//    {
//        _pauseMenu.gameObject.SetActive(incomingState == GameState.PAUSED);
//        _ingameMenu.gameObject.SetActive(incomingState == GameState.RUNNING);

//        if (prevState == GameState.PREGAME && incomingState == GameState.RUNNING)
//        {
//            _mainMenu.FadeOut();
//        }

//        if (prevState != GameState.PREGAME && incomingState == GameState.PREGAME)
//        {
//            _mainMenu.gameObject.SetActive(true);
//            _mainMenu.FadeIn();
//        }
//    }

//    private void OnDisable()
//    {
//        Events.Instance.OnGameStateChanged -= HandleGameStateChanged;
//    }

//}
