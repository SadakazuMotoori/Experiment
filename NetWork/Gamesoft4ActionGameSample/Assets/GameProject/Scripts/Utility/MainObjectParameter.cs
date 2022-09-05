using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectParameter : MonoBehaviour
{
    // チームID
    [SerializeField] int _teamID = 0;
    public int TeamID
    {
        get { return _teamID; }
        set { _teamID = value; }
    }

    // 名前
    [SerializeField] string _name;
    public string Name => _name;


    // 体力
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

    // 腕力
    [SerializeField] int _str = 5;
    public int Str => _str;


    // 死亡フラグ
    bool _isDied = false;
    public bool IsDied
    {
        get { return _isDied; }
        set { _isDied = value; }
    }

    // 無敵フラグ
    bool _isInvincible = false;
    public bool IsInvincible
    {
        get { return _isInvincible; }
        set { _isInvincible = value; }
    }

    // ヒットストップ時間
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

    // キャラクターの場合に使用
    Character.CharacterBrain CharaBrain { get; set; }
    // ステージオブジェクトの場合に使用
    StageObjectBrain StageObjBrain { get; set; }
}
