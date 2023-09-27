using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoudManagerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SoundManager.Instance.PlaySE(SEType.MouseSummon);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SoundManager.Instance.PlaySE(SEType.SE3);
        }
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    SoundManager.Instance.PlaySE(SEType.fire);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    SoundManager.Instance.PlaySE(SEType.ice);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    SoundManager.Instance.PlayBGM(BGMType.gameplay);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    SoundManager.Instance.StopSE(SEType.charge);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    SoundManager.Instance.StopSE(SEType.wind);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    SoundManager.Instance.StopSE(SEType.fire);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    SoundManager.Instance.StopSE(SEType.ice);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    SoundManager.Instance.StopBGM(BGMType.gameplay);
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    SoundManager.Instance.SetMaxVol(0.5f);
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    SoundManager.Instance.SetMaxVol(1.0f);
        //}
    }
}
