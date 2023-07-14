using UnityEngine;

public class TestDynamicStageObject : MonoBehaviour
{
    public StageObjectType type;
    private DynamicStageObject _dynamicStageObject;

    void Start()
    {
        var t = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _dynamicStageObject = t.AddComponent<DynamicStageObject>();
        _dynamicStageObject.Setup(Vector2.zero, 0,type);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("isValidMove"))
        {
            Debug.Log($"isValidMove : {_dynamicStageObject.isValidMove()}");
        }

        if (GUILayout.Button("Rep"))
        {
            _dynamicStageObject.ReplaceBaseStageObject(type);
        }
        if (GUILayout.Button("Reset"))
        {
            _dynamicStageObject.Reset();
        }
    }
}