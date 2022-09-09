using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class UICursorController : MonoBehaviour
{
    [SerializeField] List<RectTransform> _targets;

    [SerializeField] RectTransform _image;

    RectTransform _rectTransform;

    int _selectIndex = 0;

    public RectTransform GetNowSelected()
    {
        if (_selectIndex >= _targets.Count) return null;
        return _targets[_selectIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        var sizeDelta = _image.sizeDelta;
        var seq = DOTween.Sequence();
        seq.Append(_image.DOSizeDelta(sizeDelta + new Vector2(5,5), 0.5f));
        seq.Append(_image.DOSizeDelta(sizeDelta, 0.5f));
        seq.SetLoops(-1).SetLink(gameObject);
    }

    private void Update()
    {
        if (_targets.Count >= 1)
        {
            if(_rectTransform.position != _targets[_selectIndex].position)
            {
//                _rectTransform.position = _targets[_selectIndex].position;
                _rectTransform.DOMove(_targets[_selectIndex].position, 0.1f).SetLink(gameObject);
            }

            if (_rectTransform.sizeDelta != _targets[_selectIndex].sizeDelta)
            {
//                _rectTransform.sizeDelta = _targets[_selectIndex].sizeDelta;
                _rectTransform.DOSizeDelta(_targets[_selectIndex].sizeDelta, 0.1f).SetLink(gameObject);
            }
        }
    }

    public void MoveSelection(int value)
    {
        _selectIndex += value;
        if (_selectIndex >= _targets.Count) _selectIndex = _targets.Count - 1;
        if (_selectIndex < 0) _selectIndex = 0;
    }

}
