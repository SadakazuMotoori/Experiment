using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UniRx;
using UniRx.Triggers;

public class CharaUIPresenter : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    RectTransform _rectTransform;

    [SerializeField] MainObjectParameter _mainObjParam;

    // UI
    [SerializeField] TMPro.TextMeshProUGUI _textName;
//    [SerializeField] Slider _sliderHP;
    [SerializeField] GaugeController _gaugeCtrl;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        this.ObserveEveryValueChanged(text => _mainObjParam.Name)
            .Subscribe(text =>
            {
                _textName.text = text;
            }).AddTo(this);


        this.ObserveEveryValueChanged(hp => _mainObjParam.Hp)
            .Subscribe(hp =>
            {
                _gaugeCtrl.Value = (float)hp / (float)_mainObjParam.MaxHp;
            }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
//        _sliderHP.value = (float)_mainObjParam.Hp / (float)_mainObjParam.MaxHp;

        // ターゲットの3D座標からスクリーン座標へ変換し配置
        _rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _targetTransform.position);
    }
}
