/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class TMP_Menu : MonoBehaviour
{
    public void Loading()
    {
        SceneManager.LoadScene("LoadingTest");
    }
}
