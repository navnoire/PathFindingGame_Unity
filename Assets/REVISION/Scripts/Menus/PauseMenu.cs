using System.Collections;
using System.Collections.Generic;
using NavnoireCoding.Managers;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Image _background;
    [SerializeField] Button _resumeBtn;
    [SerializeField] Button _mainMenuBtn;
    [SerializeField] Button _quitBtn;

    [SerializeField] Animator _pauseMenuAnimator;

    private void Awake()
    {
        _resumeBtn.onClick.AddListener(ResumeButtonClicked);
        _mainMenuBtn.onClick.AddListener(BackToMainMenuButtonClicked);
    }

    public void FadeIn()
    {
        _background.sprite = FXManager.Instance.BackgroundBlurSprite();
        _pauseMenuAnimator.Play("BaseLayer.PauseMenuFadeIn");
    }

    public void FadeOut()
    {
        Debug.Log("[UI Manager] Pause menu fade out is not implemented yet.");
    }

    private void ResumeButtonClicked()
    {
        GameManager.Instance.TogglePauseState();
    }

    private void BackToMainMenuButtonClicked()
    {
        GameManager.Instance.BackToMainMenu();
    }

}
