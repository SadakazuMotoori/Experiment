using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-100)]
public class TimeScaleManager : MonoBehaviour
{
    public static TimeScaleManager Instance { get; private set; }

    class Node
    {
        public float ElapsedTime = 0;
        // Time Scale
        public float TimeScale = 1.0f;
    }

    LinkedList<Node> _nodes = new();

    List<Node> _reserveDeleteNodes = new();

    public async void AddTimeScale(float timeScale, float duration, float startDelayTime = 0)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(startDelayTime));

        Node node = new();
        node.ElapsedTime = duration;
        node.TimeScale = timeScale;
        _nodes.AddLast(node);

        /*
        Observable.Timer(System.TimeSpan.FromSeconds(duration), Scheduler.MainThreadIgnoreTimeScale)
            .Subscribe(_ =>
            {
                _nodes.Remove(node);
            });
        */
    }

    // VFXFixedTimeStep�o�b�N�A�b�v(���s�I�����Ɏ����Ŗ߂�Ȃ�����)
    float _defaultVFXFixedTimeStep = 0;

    void Awake()
    {
        if (Instance != null) return;
        Instance = this;

        // VFX�̍X�V�p�x���L�����Ă���
        _defaultVFXFixedTimeStep = UnityEngine.VFX.VFXManager.fixedTimeStep;
    }

    void OnDestroy()
    {
        // �I������VFX�̍X�V�p�x��߂�
        UnityEngine.VFX.VFXManager.fixedTimeStep = _defaultVFXFixedTimeStep;
    }

    void Update()
    {
//        _reserveDeleteNodes.Clear();

        float minTimeScale = 1.0f;
        foreach (var node in _nodes)
        {
            node.ElapsedTime -= Time.unscaledDeltaTime;
            if(node.ElapsedTime <= 0)
            {
                _reserveDeleteNodes.Add(node);
            }
            // �ŏ���TimeScale��I��
            minTimeScale = Mathf.Min(minTimeScale, node.TimeScale);
        }

        foreach(var node in _reserveDeleteNodes)
        {
            _nodes.Remove(node);
        }
        _reserveDeleteNodes.Clear();

        // TimeScale�Z�b�g
        Time.timeScale = minTimeScale;

        // TimeScale�����ɁAVFX�̍X�V�^�C�~���O���ω�������
        UnityEngine.VFX.VFXManager.fixedTimeStep = _defaultVFXFixedTimeStep * Time.timeScale;
    }
}
