using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private string _sceneName; 

    private TileBootstrap_Addressables _tileBootstrap;

    [SerializeField]
    private Slider m_LoadingSlider;

    [SerializeField]
    private GameObject m_PlayButton, m_LoadingText;

    private void Start()
    {
        _tileBootstrap = FindAnyObjectByType<TileBootstrap_Addressables>();
        StartCoroutine(loadNextLevel());
    }

    private IEnumerator loadNextLevel()
    {
        while (!_tileBootstrap.LoadHandle.IsDone)
        {
            m_LoadingSlider.value = _tileBootstrap.LoadHandle.PercentComplete;

            if (_tileBootstrap.LoadHandle.PercentComplete >= 0.9f && !m_PlayButton.activeInHierarchy)
                m_PlayButton.SetActive(true);

            yield return null;
        }

        m_LoadingSlider.value = _tileBootstrap.LoadHandle.PercentComplete;
        m_PlayButton.SetActive(true);
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
        Debug.Log($"Loaded Level ");
    }
}
