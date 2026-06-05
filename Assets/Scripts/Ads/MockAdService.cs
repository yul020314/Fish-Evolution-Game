using System.Threading;
using Cysharp.Threading.Tasks;

namespace FishEvolution.Ads
{
    public sealed class MockAdService : IAdService
    {
        public UniTask<bool> ShowAsync(
            AdType adType,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return UniTask.FromResult(true);
        }
    }
}
