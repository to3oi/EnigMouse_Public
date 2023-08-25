using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class TestScript : MonoBehaviour
{
    [SerializeField] GameObject obj;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            var mouse = Mouse.Instance;
            //mouse.KeyGet();
            //mouse.ClearChack();
            StartCoroutine(mouse.Death());
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }
}
