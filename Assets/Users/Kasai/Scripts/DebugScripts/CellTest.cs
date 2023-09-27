using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTest : MonoBehaviour
{
    private StageManager stageManager;
    private DynamicStageObject dynamicStageObject;
    public Vector2 TestPos = Vector2.zero;
    private bool fadebool = true;
    //public Vector2 MagicVec = Vector2.zero;
    public enum Cell
    {
        HitMagic,
        EndAnim,
        EndTurn
    }
    public enum Magic
    {
        None,
        Fire,
        Water,
        Ice,
        Wind
    }
    public Cell celltype;
    public Magic magictype;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = StageManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            dynamicStageObject = stageManager.GetDynamicStageObject((int)TestPos.x, (int)TestPos.y);
            //Debug.Log(dynamicStageObject + "\n" + stageManager.GetStageObjectType((int)TestPos.x, (int)Mathf.Abs(TestPos.y - 5)));
            Check(magictype,celltype);

        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.Instance.SceneChange(SceneList.MainGame, true, true);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.Instance.FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.Instance.FadeOut();
        }
        /*if (Input.GetKeyDown(KeyCode.Tab))
        {
            fadebool = !fadebool;
            Debug.Log(fadebool);
            
            SceneManager.Instance.FadeImageColor(fadebool);
        }*/

    }
    private void Check( Magic type, Cell cell)
    {
        var vec = Vector2.zero;
        
        switch (cell)
        {
            case Cell.EndAnim:
                dynamicStageObject.ReplaceBaseStageObject(StageObjectType.None);
                break;
            case Cell.EndTurn:
                dynamicStageObject.EndTurn();
                break;
            case Cell.HitMagic:
                switch(type)
                {
                    case Magic.Fire:
                        //Debug.Log("MagicFire");
                        dynamicStageObject.HitMagic(MagicType.Fire, vec);
                        break;
                    case Magic.Water:
                        //Debug.Log("MagicWater");
                        dynamicStageObject.HitMagic(MagicType.Water, vec);
                        break;
                    case Magic.Ice:
                        //Debug.Log("MagicIce");
                        dynamicStageObject.HitMagic(MagicType.Ice, vec);
                        break;
                    case Magic.Wind:
                        //Debug.Log("MagicWind");
                        dynamicStageObject.HitMagic(MagicType.Wind, vec);
                        break;
                    default:
                        
                        break;
                }
                break;


            default:
                Debug.LogError("種類とれてない");
                break;
        }
    }
}
