using System.Collections.Generic;
using Map;
using UnityEditor;
using UnityEngine;


/// <summary>
/// ゲーム内のマップを作成するエディター拡張
/// </summary>
public class StageMapEditor : EditorWindow
{
    private Editor _editor;
    public StageMaps StageMaps;

    private int index = 0;
    private Vector2 boxSizeOffset = new Vector2(100, 350);

    private Vector2 _scrollPosition = Vector2.zero;

    [MenuItem("CustomEditor/Map")]
    static void Open()
    {
        var window = GetWindow<StageMapEditor>();
        window.titleContent = new GUIContent("MapEditor");
    }

    void ResetList(int _index = -1)
    {
        if (_index == -1)
        {
            _index = index;
        }

        StageMaps._stageMapList[_index].y.Clear();

        for (int y = 0; y < 6; y++)
        {
            X mapX = new X();
            for (int x = 0; x < 6; x++)
            {
                mapX.x.Add(StageObjectType.None);
            }

            StageMaps._stageMapList[_index].y.Add(mapX);
        }
    }

    /// <Summary>
    /// ウィンドウのパーツを表示する
    /// </Summary>
    void OnGUI()
    {
        //ScriptableObjectの取得
        var guids = UnityEditor.AssetDatabase.FindAssets("t:StageMaps");
        if (guids.Length == 0)
        {
            throw new System.IO.FileNotFoundException("StageMaps does not found");
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        StageMaps = AssetDatabase.LoadAssetAtPath<StageMaps>(path);


        //Listを初期化する処理
        if (GUILayout.Button("現在のListを初期化する"))
        {
            if (StageMaps != null)
            {
                ResetList();
            }
        }


        //Editorの要素が更新されたかチェックする
        if (EditorGUI.EndChangeCheck())
        {
            if (StageMaps != null)
            {
                //要素を更新
                _editor = Editor.CreateEditor(StageMaps);
                EditorUtility.SetDirty(StageMaps);
            }
        }

        EditorGUILayout.Space();
        List<string> mapList = new List<string>();
        for (int i = 0; i < StageMaps._stageMapList.Count; i++)
        {
            mapList.Add(i.ToString());
        }


        //ポップアップのGUIを作成
        index = EditorGUILayout.Popup("ステージセレクト", index, mapList.ToArray());

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(4));

        if (_editor != null)
        {
            //StageMapsの要素の表示・変更処理
            var database = (StageMaps)_editor.target;
            //スクロールの処理
            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            //StageMapsの要素を表示
            //縦揃え
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < StageMaps._stageMapList[index].y.Count; y++)
            {
                //横揃え
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < StageMaps._stageMapList[index].y[y].x.Count; x++)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    var objectType = StageMaps._stageMapList[index].y[y].x[x];
                    objectType = (StageObjectType)EditorGUILayout.EnumPopup(objectType);
                    StageMaps._stageMapList[index].y[y].x[x] = objectType;
                    //中央揃え
                    GUILayout.FlexibleSpace();
                    // TODO:オブジェクトのデータが揃ったら`objectType.ToString()`をオブジェクトの画像に差し替え
                    GUILayout.Box(objectType.ToString(), GUILayout.Width((position.size.x - boxSizeOffset.x) / 6),
                        GUILayout.Height((position.size.y - boxSizeOffset.y) / 6));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            Undo.RecordObject(database, "");

            //スクロールの処理
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();


            //要素の追加と削除
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存", GUILayout.Height(30), GUILayout.Width(60)))
            {
                // ScriptableObjectのパスは 'Assets/' から始まる相対パス
                ScriptableObject obj =
                    AssetDatabase.LoadAssetAtPath("Assets/Resources/StageMaps.asset", typeof(ScriptableObject)) as
                        ScriptableObject;
                // 変更を通知
                EditorUtility.SetDirty(obj);

                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("+", GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (StageMaps != null)
                {
                    database._stageMapList.Add(new Map.Map(new List<X>(6)));
                    ResetList(database._stageMapList.Count - 1);
                    index = database._stageMapList.Count - 1;
                    EditorUtility.SetDirty(StageMaps);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("-", GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (StageMaps != null)
                {
                    if (database._stageMapList.Count <= 1)
                    {
                        Debug.Log($"Listを空にすることはできません");
                    }
                    else
                    {
                        database._stageMapList.RemoveAt(index);
                        if (database._stageMapList.Count - 1 < index)
                        {
                            index = database._stageMapList.Count - 1;
                        }

                        EditorUtility.SetDirty(StageMaps);
                    }
                }
            }

            EditorUtility.SetDirty(StageMaps);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}