using UnityEngine;

public class TestWaterShaderUVScroll : MonoBehaviour
{
    private Material _material;


    [SerializeField] private float IceRatio = 0;
    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }


    void Update()
    {
        if (IceRatio <= 1)
        {
            _material.SetFloat("_UVTime", _material.GetFloat("_UVTime") + Time.deltaTime);
        }
        _material.SetFloat("_IceRatio",IceRatio );
    }
}