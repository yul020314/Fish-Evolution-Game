using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace FishEvolution.Core
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(GameScene scene, CancellationToken cancellationToken);
    }

    public sealed class SceneLoader : ISceneLoader
    {
        public async UniTask LoadSceneAsync(GameScene scene, CancellationToken cancellationToken)
        {
            var sceneBuildIndex = (int)scene;

            if (SceneManager.GetActiveScene().buildIndex == sceneBuildIndex)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
