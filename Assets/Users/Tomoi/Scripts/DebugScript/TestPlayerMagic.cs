using UnityEngine;

public class TestPlayerMagic : MonoBehaviour
{
    [SerializeField] private BasePlayerMagic _playerMagic;
    private Camera _camera;

    private void Start()
    {
        //var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //_playerMagic = g.AddComponent<PlayerMagicSample>();
        _playerMagic = Instantiate(_playerMagic);
        _camera = Camera.main;
    }

    private void Update()
    {
        var pos = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,LayerMask.GetMask("Ground")))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _playerMagic.Init(hit.point);
            }

            if (Input.GetMouseButton(0))
            {
                _playerMagic.Move(hit.point);
            }            
            if (Input.GetMouseButton(1))
            {
                _playerMagic.Ignite();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _playerMagic.Release();
            }
        }
    }
}