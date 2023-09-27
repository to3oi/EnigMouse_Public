using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    [ContextMenu("SceneChange")]
    private void SceneChange()
    {
        SceneManager.Instance.SceneChange(SceneList.MainGame,true,true);
    }
}
