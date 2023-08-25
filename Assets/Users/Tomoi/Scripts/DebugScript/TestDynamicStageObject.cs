using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestDynamicStageObject : MonoBehaviour
{
    public StageObjectType stageObjectType;
    public MagicType MagicType = MagicType.Ice;
    private DynamicStageObject _dynamicStageObject;

    void Start()
    {
        //var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var t = new GameObject();
        _dynamicStageObject = t.AddComponent<DynamicStageObject>();
        _dynamicStageObject.Setup(Vector2.zero, 0,stageObjectType);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("isValidMove"))
        {
            Debug.Log($"isValidMove : {_dynamicStageObject.isValidMove()}");
        }

        if (GUILayout.Button("HitMagic"))
        {
            _dynamicStageObject.HitMagic(MagicType,Vector2.up);
        }
        
        if (GUILayout.Button("Rep"))
        {
            _dynamicStageObject.ReplaceBaseStageObject(stageObjectType);
        }
        if (GUILayout.Button("Reset"))
        {
            _dynamicStageObject.Reset();
        }
        if (GUILayout.Button("EndTurn"))
        {
            _dynamicStageObject.EndTurn().Forget();
        }
    }
}