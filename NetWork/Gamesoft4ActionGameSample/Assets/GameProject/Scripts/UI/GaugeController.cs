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
    [MenuItem("GameObject/UI/[����]Gauge", priority = 21)]
    public static void CreateGaugeSlider(MenuCommand menuCommand)
    {
        string path = AssetDatabase.GUIDToAssetPath("5e750a693ee3f654aa5df16bbed6f0ae");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return;

        // �쐬
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        // Unpack���Ă���
        PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

        // �e�̐ݒ�
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Undo�o�^
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        // �I����Ԃɂ���
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

    // ��Editor��̂ݓ���
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

