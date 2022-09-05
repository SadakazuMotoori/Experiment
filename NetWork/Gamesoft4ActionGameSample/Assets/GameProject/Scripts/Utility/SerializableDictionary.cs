using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // �V���A���C�Y�p��List�m�[�h
    [System.Serializable]
    public struct Node
    {
        public TKey Key;
        public TValue Value;
    }
    [SerializeField] List<Node> _list;

    // Dictionary���X�V�ς�
    bool _updatedDictionary = false;

    public new KeyCollection Keys
    {
        get
        {
            ListToDic();
            return base.Keys;
        }
    }

    public new ValueCollection Values
    {
        get
        {
            ListToDic();
            return base.Values;
        }
    }
    // �A�C�e����ǉ�
    public new void Add(TKey key, TValue value)
    {
        ListToDic();
        base.Add(key, value);
    }

    // �A�C�e�����폜
    public new bool Remove(TKey key)
    {
        ListToDic();
        return base.Remove(key);
    }

    // �擾�E�Z�b�g
    // �擾���A���݂��Ȃ��ꍇ�͗�O����
    public new TValue this[TKey key]
    {
        get
        {
            ListToDic();
            return base[key];
        }
        set
        {
            ListToDic();
            base[key] = value;
        }
    }

    // �擾�@���݂��Ȃ��ꍇ�͉������Ȃ�
    // �߂�l�Ftrue...���݂����@false...���݂��Ȃ�
    public new bool TryGetValue(TKey key, out TValue value)
    {
        ListToDic();
        return base.TryGetValue(key, out value);
    }

    // List�̓��e����Dictionary�փR�s�[
    void ListToDic()
    {
        // Dictionary���X�V�ς݂Ȃ�A���X�g����R�s�[�͂��Ȃ�
        if (_updatedDictionary) return;
        // �X�V�ς݂Ƃ���
        _updatedDictionary = true;

        Clear();

        if (_list == null) return;
        // ���X�g�̓��e��Dictionary�փR�s�[
        foreach (var node in _list)
        {
            Add(node.Key, node.Value);
        }
    }

    // �f�V���A���C�Y��Ɏ��s�����
    // Inspector�ł́A�Ȃɂ�����ύX���Ɏ��s�����
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }

    // �V���A���C�Y�O�Ɏ��s�����
    // Inspector�ł́A�\�����͏�Ɏ��s����Ă���
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // ���V���A���C�Y�\�Ȃ̂�List�݂̂Ȃ̂�
        // ����Dictionary���ύX����Ă�����AList�փf�[�^���R�s�[����

        // Dictionary���ύX����Ă��Ȃ��Ȃ�A�X�L�b�v
        if (_updatedDictionary == false) return;
        _updatedDictionary = false;

        // Dictionary�̓��e��List�փR�s�[
        if (_list == null) _list = new();

        _list.Clear();
        foreach (var pair in this)
        {
            Node node = new();
            node.Key = pair.Key;
            node.Value = pair.Value;
            _list.Add(node);
        }
    }

    /*
    // Inspector�ŕҏW���Ă��Ă΂��
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // List -> Dictionary

        Clear();

        foreach (var node in _list)
        {
            if (ContainsKey(node.Key))
            {
                continue;
            }
            Add(node.Key, node.Value);
        }
    }

    // Inspector�ŕ\�����ɂ������ƌĂ΂��
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Dictionary -> List
        if (_list == null) _list = new();
        _list.Clear();
        foreach (var pair in this)
        {
            Node node = new();
            node.Key = pair.Key;
            node.Value = pair.Value;
            _list.Add(node);
        }
    }
    */
}
