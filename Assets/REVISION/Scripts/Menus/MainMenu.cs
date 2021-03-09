using System.Collections;
using System.Collections.Generic;
using NavnoireCoding.Managers;
using NavnoireCoding.Utils;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private Animator _mainMenuAnimator;

    private int _fadeOutHash;
    private int _fadeInHash;

    private void Start()
    {
        _mainMenuAnimator = GetComponent<Animator>();

        _fadeInHash = Animator.StringToHash("BaseLayer.MainMenuFadeIn");
        _fadeOutHash = Animator.StringToHash("BaseLayer.MainMenuFadeOut");
    }

    public void FadeOut()
    {
        UIManager.Instance.SetUICameraActive(false);
        _mainMenuAnimator.Play(_fadeOutHash);
    }
    public void FadeIn() { _mainMenuAnimator.Play(_fadeInHash); }

    private void OnFadeOutComplete()
    {
        Events.Instance.OnMainMenuFadeComplete?.Invoke(true);
        gameObject.SetActive(false);
    }

    private void OnFadeInComplete()
    {
        Events.Instance.OnMainMenuFadeComplete?.Invoke(false);
        UIManager.Instance.SetUICameraActive(true);
    }
}
