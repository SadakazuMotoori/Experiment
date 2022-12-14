using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

#if UNITY_EDITOR
using UnityEditor;
public partial class GaugeController : MonoBehaviour
{
    [MenuItem("GameObject/UI/[自作]Gauge", priority = 21)]
    public static void CreateGaugeSlider(MenuCommand menuCommand)
    {
        string path = AssetDatabase.GUIDToAssetPath("5e750a693ee3f654aa5df16bbed6f0ae");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return;

        // 作成
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        // Unpackしておく
        PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

        // 親の設定
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Undo登録
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        // 選択状態にする
        Selection.activeObject = go;
    }

}
#endif

public partial class GaugeController : MonoBehaviour
{
    [SerializeField] Image _fill;

    [SerializeField, Range(0.0f, 1.0f)] float _value = 0.5f;

    public float Value
    {
        get { return _value; }
        set { _value = Mathf.Clamp01(value); }
    }

    // ※Editor上のみ動作
    void OnValidate()
    {
        _fill.fillAmount = _value;
    }

    void Update()
    {
        if (_fill != null)
        {
            _fill.fillAmount = _value;
        }
    }
}

