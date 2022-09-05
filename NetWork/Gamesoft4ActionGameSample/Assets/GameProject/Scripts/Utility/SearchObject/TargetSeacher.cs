using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SeachTargetを検索する
/// </summary>
public class TargetSeacher : ObjectMonoBehaviour
{
    // 検索する相手のタイプ
    [SerializeField] SearchTarget.Types _targetType;
    public SearchTarget.Types TargetType
    {
        get => _targetType;
        set => _targetType = value;
    }

    [SerializeField] MainObjectParameter _mainObjParam;

    // 半径
    [SerializeField] float _radius = 1;
    public float Radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    [System.Flags]
    public enum ComparisonFlags
    {
        Node = 0,
        SameTeam = 0x00000001,
        OtherTeam = 0x00000002,
    }
    // 
    [SerializeField] ComparisonFlags _comparisonFlags;

    // レイヤーマスク
    [SerializeField] LayerMask _layerMasks;

    public LayerMask LayerMasks
    {
        get { return _layerMasks; }
        set { _layerMasks = value; }
    }


    // 最も近いターゲットに、このTransformを追従させる(近い順)
    [SerializeField] List<Transform> _followTargets;

    // 検索結果データ
    public struct Node
    {
        public float Distance;
        public SearchTarget Target;
    }
    List<Node> _nodes = new(50);
    public List<Node> Nodes => _nodes;

    // 検索用一時データ
    int _numColliders = 0;
    Collider[] _tempColliders = new Collider[100];

    // 比較関数の型
    public Node? GetClosestTarget()
    {
        if (_nodes.Count == 0) return null;
        return _nodes[0];
    }

    void Awake()
    {

    }

    void Update()
    {
        // 判定実行
        _numColliders = Physics.OverlapSphereNonAlloc(transform.position, _radius, _tempColliders, _layerMasks);

        // 対象物に絞り込む
        Vector3 pos = transform.position;
        _nodes.Clear();
        for(int i = 0;i < _numColliders; i++)
        {
            var collider = _tempColliders[i];

            // SearchTargetが存在するものが対象
            var searchTar = collider.attachedRigidbody.GetComponent<SearchTarget>();
            if (searchTar == null) continue;

            // 自キャラは無視
            if (_mainObjParam == searchTar.MainObjParam) continue;

            // 指定種類以外は無視
            if (searchTar.Type != _targetType) continue;

            // 同チームも判定する
            bool teamResult = false;
            if (_comparisonFlags.HasFlag(ComparisonFlags.SameTeam))
            {
                if (_mainObjParam.TeamID == searchTar.MainObjParam.TeamID) teamResult = true;
            }
            // 他チームも判定する
            if (_comparisonFlags.HasFlag(ComparisonFlags.OtherTeam))
            {
                if (_mainObjParam.TeamID != searchTar.MainObjParam.TeamID) teamResult = true;
            }
            if (teamResult == false) continue;

            // 距離を計算
            Vector3 v = collider.transform.position - pos;
            Node node = new();
            node.Distance = v.magnitude;
            node.Target = searchTar;
            // 追加
            _nodes.Add(node);
        }

        // ソート
        _nodes.Sort((a, b) => a.Distance.CompareTo(b.Distance));
    }

    void LateUpdate()
    {
        if(_followTargets != null)
        {
            for(int i = 0;i < _nodes.Count; i++)
            {
                if(i >= _followTargets.Count) break;

                if (_nodes[i].Target == null) continue;

                _followTargets[i].position = _nodes[i].Target.transform.position;
                _followTargets[i].rotation = _nodes[i].Target.transform.rotation;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f);

        Gizmos.DrawWireSphere(transform.position, _radius);

        /*
        if (_colliders != null)
        {
//            Gizmos.color = new Color(1, 1, 0, 0.5f);

            foreach (var collider in _colliders)
            {
            }
        }
        */
    }
#endif
}
