using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectParameter : MonoBehaviour
{
    // �`�[��ID
    [SerializeField] int _teamID = 0;
    public int TeamID
    {
        get { return _teamID; }
        set { _teamID = value; }
    }

    // ���O
    [SerializeField] string _name;
    public string Name => _name;


    // �̗�
    [SerializeField] int _hp = 0;
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hp <= 0) _hp = 0;
        }
    }

    int _maxHp;
    public int MaxHp => _maxHp;

    // �r��
    [SerializeField] int _str = 5;
    public int Str => _str;


    // ���S�t���O
    bool _isDied = false;
    public bool IsDied
    {
        get { return _isDied; }
        set { _isDied = value; }
    }

    // ���G�t���O
    bool _isInvincible = false;
    public bool IsInvincible
    {
        get { return _isInvincible; }
        set { _isInvincible = value; }
    }

    // �q�b�g�X�g�b�v����
    public float HitStopTimer { get; set; } = 0;

    void Awake()
    {
        CharaBrain = GetComponent<Character.CharacterBrain>();
        StageObjBrain = GetComponent<StageObjectBrain>();

        _maxHp = _hp;
    }

    void Update()
    {
        HitStopTimer -= Time.deltaTime;
        if (HitStopTimer <= 0)
        {
            HitStopTimer = 0;
        }
    }

    // �L�����N�^�[�̏ꍇ�Ɏg�p
    Character.CharacterBrain CharaBrain { get; set; }
    // �X�e�[�W�I�u�W�F�N�g�̏ꍇ�Ɏg�p
    StageObjectBrain StageObjBrain { get; set; }
}
