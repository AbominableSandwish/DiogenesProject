/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Private Data
    [SerializeField] private TextMeshProUGUI _elementText;
    [SerializeField] private List<Image> _images;
   
    private PlayerController _player;

    protected static UIManager _instance = null;
    #endregion

    #region Mono
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        _player = FindAnyObjectByType<PlayerController>();
    }
    #endregion

    #region Public Method
    public static UIManager Instance { get => _instance; protected set => _instance = value; }

    public static void SetTextElement(string nameElement)
    {
        Instance._elementText.text = nameElement;
    }
    
    //public void SelectStructure(int id)
    //{
    //    foreach(Image image in _images)
    //    {
    //        image.color = Color.grey;
    //    }

    //    bool isSelect = this._player.SelectStructure(id);
    //    if (isSelect)
    //    {
    //        _images[id].color = Color.green;
    //    }
    //    else
    //    {
    //        _images[id].color = Color.grey;
    //    }     
    //}
    #endregion
}
