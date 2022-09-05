using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // シリアライズ用のListノード
    [System.Serializable]
    public struct Node
    {
        public TKey Key;
        public TValue Value;
    }
    [SerializeField] List<Node> _list;

    // Dictionaryが更新済み
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
    // アイテムを追加
    public new void Add(TKey key, TValue value)
    {
        ListToDic();
        base.Add(key, value);
    }

    // アイテムを削除
    public new bool Remove(TKey key)
    {
        ListToDic();
        return base.Remove(key);
    }

    // 取得・セット
    // 取得時、存在しない場合は例外発生
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

    // 取得　存在しない場合は何もしない
    // 戻り値：true...存在した　false...存在しない
    public new bool TryGetValue(TKey key, out TValue value)
    {
        ListToDic();
        return base.TryGetValue(key, out value);
    }

    // Listの内容からDictionaryへコピー
    void ListToDic()
    {
        // Dictionaryが更新済みなら、リストからコピーはしない
        if (_updatedDictionary) return;
        // 更新済みとする
        _updatedDictionary = true;

        Clear();

        if (_list == null) return;
        // リストの内容をDictionaryへコピー
        foreach (var node in _list)
        {
            Add(node.Key, node.Value);
        }
    }

    // デシリアライズ後に実行される
    // Inspectorでは、なにかしら変更時に実行される
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {

    }

    // シリアライズ前に実行される
    // Inspectorでは、表示中は常に実行されている
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // ※シリアライズ可能なのはListのみなので
        // もしDictionaryが変更されていたら、Listへデータをコピーする

        // Dictionaryが変更されていないなら、スキップ
        if (_updatedDictionary == false) return;
        _updatedDictionary = false;

        // Dictionaryの内容をListへコピー
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
    // Inspectorで編集しても呼ばれる
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

    // Inspectorで表示中にもずっと呼ばれる
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
