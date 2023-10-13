using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Image = UnityEngine.UI.Image;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        SceneManager.Instance.SceneChange(SceneList.MainGame, true, true);
    }
    private void Update()
    {

    }

}
