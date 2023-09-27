using UnityEngine;

public class BackGroundLoop : MonoBehaviour
{
    [SerializeField] private float ResetPosition;
    private float MoveSpeed = -0.01f;
    private Vector3 StartPosition;
    void Awake()
    {
        StartPosition = transform.position;
    }
    private void FixedUpdate()
    {
        transform.Translate(MoveSpeed, 0, 0, Space.World);
        if (transform.position.x < ResetPosition)
            transform.position = StartPosition;
    }
}