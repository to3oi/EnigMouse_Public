using UnityEngine;

public class InitScript : MonoBehaviour
{
    [SerializeField] private SceneList destinationScene;
    private void Start()
    {
        SceneManager.Instance.SceneChange(destinationScene,true,true);
    }
}
