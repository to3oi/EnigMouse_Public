using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageObjectTest : MonoBehaviour
{
    [SerializeField]
    private BaseStageObject _stageObject;

    private void Start()
    {
        _stageObject.InitAnimation().Forget();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Fire",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.HitMagic(MagicType.Fire,Vector2.up,out _);
        }
        if(GUILayout.Button("Water",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.HitMagic(MagicType.Water,Vector2.up,out _);
        }
        if(GUILayout.Button("Ice",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.HitMagic(MagicType.Ice,Vector2.up,out _);
        }
        if(GUILayout.Button("Wind",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.HitMagic(MagicType.Wind,Vector2.up,out _);
        }
        
        if(GUILayout.Button("InitAnimation",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.InitAnimation().Forget();
        }
        if(GUILayout.Button("EndAnimation",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.EndAnimation().Forget();
        }
        
        if(GUILayout.Button("MoveToCell",GUILayout.Width(100),GUILayout.Height(20)))
        {
            _stageObject.MoveToCell().Forget();
        }
    }
}
