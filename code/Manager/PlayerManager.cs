using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    [Inject] CameraManager _cameraManager;
    [Inject] InGameView _inGameView;
    [SerializeField] PlayerCore _playerPrefab;
    private List<IHackable> hackObjects = new List<IHackable>();
    private PlayerCore _currentPlayerInstance;
    public event Action NextStageCallBack;
    public event Action HackCallBack;
    public event Action DeathCallBack;

    /// <summary>
    /// プレイヤーを生成する
    /// </summary>
    /// <param name="genPos"></param>
    public void CreatePlayer(Vector2 genPos)
    {
        if (_currentPlayerInstance != null)
            Destroy(_currentPlayerInstance.gameObject);
        _currentPlayerInstance = Instantiate(_playerPrefab,new Vector3(genPos.x,genPos.y),Quaternion.identity);
        _cameraManager.SetTarget(_currentPlayerInstance.transform);
        _currentPlayerInstance.Initialaize();
        //コールバック関数を登録する
        _currentPlayerInstance.NextStageCallBack += NextStageCallBack;
        _currentPlayerInstance.DeathCallBack += DeathCallBack;
        Bind();
    }

    public void AddHackObject(IHackable hackable)
    {
        hackObjects.Add(hackable);
    }

    public async UniTask DestroyPlayer()
    {
        await _currentPlayerInstance.DeathPlayer();
    }

    public async void GameClearPlayer()
    {
        await _currentPlayerInstance.DeathPlayer();
    }

    private void Bind()
    {
        _currentPlayerInstance.HackStateProp.Subscribe(state =>
        {
            foreach (var hackObject in hackObjects)
            {
                hackObject.SetHackMode(state);
            }
        }).AddTo(_currentPlayerInstance.gameObject);

        _currentPlayerInstance.BulletCount.Subscribe(count =>
        {
            _inGameView.SetBulletCount(count);
        }).AddTo(_currentPlayerInstance.gameObject);
    }

    public PlayerCore PlayerCore => _currentPlayerInstance;
}
