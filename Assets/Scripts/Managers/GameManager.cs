using System.Threading;
using Cysharp.Threading.Tasks;
using FishEvolution.Core;
using UnityEngine;
using VContainer.Unity;

namespace FishEvolution.Managers
{
    public sealed class GameManager : IAsyncStartable
    {
        private readonly IConfigManager _configManager;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISceneLoader _sceneLoader;

        public GameManager(
            IConfigManager configManager,
            IGameStateMachine gameStateMachine,
            ISceneLoader sceneLoader)
        {
            _configManager = configManager;
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            Debug.Log("Fish Evolution core framework started.");

            await _configManager.InitializeAsync(cancellationToken);
            await _gameStateMachine.ChangeStateAsync(GameState.Bootstrap, cancellationToken);
            await _sceneLoader.LoadSceneAsync(GameScene.MainMenu, cancellationToken);
            await _gameStateMachine.ChangeStateAsync(GameState.MainMenu, cancellationToken);
        }
    }
}
