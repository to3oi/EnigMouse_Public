using System;
using UnityEngine;

public partial class GameManager
{
    public int Turn { get; private set; }
    private int _layerMask = 1 << 7;

    [SerializeField]
    private GameObject testParticle;
    /// <summary>
    /// 現在のターンを終了する
    /// </summary>
    public void TurnComplete()
    {
        Turn++;
    }

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    public void TestMagicPoint(UnityEngine.Vector2 pos, MagicType magicType)
    {

        Ray ray = _camera.ScreenPointToRay(pos);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        /*if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,LayerMask.GetMask("DynamicStageObject")))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent<DynamicStageObject>(out DynamicStageObject _))
            {
                Debug.Log(objectHit.transform.name);
            }
            else
            {
                Debug.Log("なし");
            }

            testParticle.transform.position = objectHit.position;
        }*/
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,LayerMask.GetMask("Ground")))
        {
            Transform objectHit = hit.transform;
            testParticle.transform.position = hit.point;
        }
    }
}