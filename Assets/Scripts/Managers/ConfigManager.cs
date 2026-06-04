using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace FishEvolution.Managers
{
    public interface IConfigManager
    {
        bool IsInitialized { get; }

        UniTask InitializeAsync(CancellationToken cancellationToken);

        void RegisterConfig<TConfig>(TConfig config) where TConfig : class;

        bool TryGetConfig<TConfig>(out TConfig config) where TConfig : class;
    }

    public sealed class ConfigManager : IConfigManager
    {
        private readonly Dictionary<Type, object> _configs = new Dictionary<Type, object>();

        public bool IsInitialized { get; private set; }

        public UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsInitialized = true;
            return UniTask.CompletedTask;
        }

        public void RegisterConfig<TConfig>(TConfig config) where TConfig : class
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _configs[typeof(TConfig)] = config;
        }

        public bool TryGetConfig<TConfig>(out TConfig config) where TConfig : class
        {
            if (_configs.TryGetValue(typeof(TConfig), out var value))
            {
                config = (TConfig)value;
                return true;
            }

            config = null;
            return false;
        }
    }
}
