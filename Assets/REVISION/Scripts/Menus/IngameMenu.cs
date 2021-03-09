using System.Collections;
using System.Collections.Generic;
using NavnoireCoding.Managers;
using NavnoireCoding.Utils;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private Button _pauseBtn;
    [SerializeField] private Button _rebornBtn;
    [SerializeField] private Button _restartLevelBtn;
    [SerializeField] private Button _tryAnotherSeedBtn;
    [SerializeField] private Button _takeScreenshotBtn;

    [SerializeField] private Text _scoreTxt;

    private void Awake()
    {
        _pauseBtn.onClick.AddListener(PauseBtnClicked);
    }

    private void OnEnable()
    {
        Events.Instance.OnItemCollected += UpdateScoreText;
    }


    private void PauseBtnClicked()
    {
        GameManager.Instance.TogglePauseState();
    }

    private void UpdateScoreText(int newScore)
    {
        _scoreTxt.text = string.Format("{0} / {1}", newScore, GameManager.Instance.levelData.levelGoal);
    }


    private void OnDisable()
    {
        if (Events.IsInitialized) Events.Instance.OnItemCollected -= UpdateScoreText;
    }
}
