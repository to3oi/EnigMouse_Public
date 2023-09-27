using UnityEngine;

public class TestInputManager : MonoBehaviour
{
    [SerializeField] private SceneList destinationScene;
    [ContextMenu("OnStartInput")]
    private void OnStartInput()
    {
        InputManager.Instance.ChangeScene(destinationScene);
    }
}
