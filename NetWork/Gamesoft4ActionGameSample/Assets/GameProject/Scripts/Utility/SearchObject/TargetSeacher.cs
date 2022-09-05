using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SeachTarget����������
/// </summary>
public class TargetSeacher : ObjectMonoBehaviour
{
    // �������鑊��̃^�C�v
    [SerializeField] SearchTarget.Types _targetType;
    public SearchTarget.Types TargetType
    {
        get => _targetType;
        set => _targetType = value;
    }

    [SerializeField] MainObjectParameter _mainObjParam;

    // ���a
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

    // ���C���[�}�X�N
    [SerializeField] LayerMask _layerMasks;

    public LayerMask LayerMasks
    {
        get { return _layerMasks; }
        set { _layerMasks = value; }
    }


    // �ł��߂��^�[�Q�b�g�ɁA����Transform��Ǐ]������(�߂���)
    [SerializeField] List<Transform> _followTargets;

    // �������ʃf�[�^
    public struct Node
    {
        public float Distance;
        public SearchTarget Target;
    }
    List<Node> _nodes = new(50);
    public List<Node> Nodes => _nodes;

    // �����p�ꎞ�f�[�^
    int _numColliders = 0;
    Collider[] _tempColliders = new Collider[100];

    // ��r�֐��̌^
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
        // ������s
        _numColliders = Physics.OverlapSphereNonAlloc(transform.position, _radius, _tempColliders, _layerMasks);

        // �Ώە��ɍi�荞��
        Vector3 pos = transform.position;
        _nodes.Clear();
        for(int i = 0;i < _numColliders; i++)
        {
            var collider = _tempColliders[i];

            // SearchTarget�����݂�����̂��Ώ�
            var searchTar = collider.attachedRigidbody.GetComponent<SearchTarget>();
            if (searchTar == null) continue;

            // ���L�����͖���
            if (_mainObjParam == searchTar.MainObjParam) continue;

            // �w���ވȊO�͖���
            if (searchTar.Type != _targetType) continue;

            // ���`�[�������肷��
            bool teamResult = false;
            if (_comparisonFlags.HasFlag(ComparisonFlags.SameTeam))
            {
                if (_mainObjParam.TeamID == searchTar.MainObjParam.TeamID) teamResult = true;
            }
            // ���`�[�������肷��
            if (_comparisonFlags.HasFlag(ComparisonFlags.OtherTeam))
            {
                if (_mainObjParam.TeamID != searchTar.MainObjParam.TeamID) teamResult = true;
            }
            if (teamResult == false) continue;

            // �������v�Z
            Vector3 v = collider.transform.position - pos;
            Node node = new();
            node.Distance = v.magnitude;
            node.Target = searchTar;
            // �ǉ�
            _nodes.Add(node);
        }

        // �\�[�g
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
