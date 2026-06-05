using System.Threading;
using Cysharp.Threading.Tasks;

namespace FishEvolution.Ads
{
    public interface IAdService
    {
        UniTask<bool> ShowAsync(
            AdType adType,
            CancellationToken cancellationToken);
    }
}
