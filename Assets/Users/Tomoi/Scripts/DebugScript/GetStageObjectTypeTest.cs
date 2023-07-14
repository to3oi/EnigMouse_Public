using UnityEngine;

public class GetStageObjectTypeTest : MonoBehaviour
{
    [SerializeField] private Vector2 _vector2 = Vector2.zero;

    
    private void OnGUI()
    {
        if(GUILayout.Button("Check",GUILayout.Width(100),GUILayout.Height(20)))
        {
            var res =StageManager.Instance.GetStageObjectType((int)_vector2.x, (int)_vector2.y);
            Debug.Log(res != null ? res.ToString() : "null");
        }
    }
}
