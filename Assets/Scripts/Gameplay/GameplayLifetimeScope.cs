using FishEvolution.Audio;
using FishEvolution.Ads;
using FishEvolution.Boss;
using FishEvolution.Combat;
using FishEvolution.Config;
using FishEvolution.FishBook;
using FishEvolution.Map;
using FishEvolution.Performance;
using FishEvolution.Quest;
using FishEvolution.Ranking;
using FishEvolution.Save;
using FishEvolution.Shop;
using FishEvolution.Tutorial;
using FishEvolution.UI;
using FishEvolution.VFX;
using System;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FishEvolution.Gameplay
{
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        [UnityEngine.SerializeField] private HudDisplayDataSO _hudDisplayData;
        [UnityEngine.SerializeField] private AudioSettingsSO _audioSettings;
        [UnityEngine.SerializeField] private VFXSettingsSO _vfxSettings;
        [UnityEngine.SerializeField] private AudioSource _bgmSource;
        [UnityEngine.SerializeField] private AudioSource _sfxSource;
        [UnityEngine.SerializeField] private Transform _vfxPoolRoot;
        [UnityEngine.SerializeField] private SkillDataSO[] _skillData = new SkillDataSO[0];
        [UnityEngine.SerializeField] private QuestDataSO[] _questData = new QuestDataSO[0];
        [UnityEngine.SerializeField] private ShopItemDataSO[] _shopItems = new ShopItemDataSO[0];
        [UnityEngine.SerializeField] private FishDataSO[] _fishBookData = new FishDataSO[0];
        [UnityEngine.SerializeField] private ScoreSettings _scoreSettings = new ScoreSettings();
        [UnityEngine.SerializeField] private AdSettings _adSettings = new AdSettings();
        [UnityEngine.SerializeField] private PerformanceSettings _performanceSettings = new PerformanceSettings();
        [UnityEngine.SerializeField] private GuideStep[] _guideSteps = new GuideStep[0];
        [UnityEngine.SerializeField] private int _maxRankRecords = 20;
        [UnityEngine.SerializeField] private string _saveFileName = "player_progress.json";
        [UnityEngine.SerializeField] private float _autoSaveInterval = 10f;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            RegisterSceneComponent<PlayerController>(builder);
            RegisterSceneComponent<PlayerGrowthController>(builder);
            RegisterSceneComponent<FishSpawner>(builder);
            RegisterSceneComponent<BossController>(builder);
            RegisterSceneComponent<MapManager>(builder);
            RegisterSceneComponent<FogSystem>(builder);
            builder.RegisterMessageBroker<FoodConsumedEvent>(options);
            builder.RegisterMessageBroker<PlayerLevelUpEvent>(options);
            builder.RegisterMessageBroker<PlayerProgressChangedEvent>(options);
            builder.RegisterMessageBroker<DamageRequest>(options);
            builder.RegisterMessageBroker<DamageAppliedEvent>(options);
            builder.RegisterMessageBroker<EntityDeathEvent>(options);
            builder.RegisterMessageBroker<BossDeadEvent>(options);
            builder.RegisterMessageBroker<MapChangeRequest>(options);
            builder.RegisterMessageBroker<MapChangedEvent>(options);
            builder.RegisterMessageBroker<MapUnlockedEvent>(options);
            builder.RegisterMessageBroker<MapLockedEvent>(options);
            builder.RegisterMessageBroker<QuestProgressChangedEvent>(options);
            builder.RegisterMessageBroker<QuestCompletedEvent>(options);
            builder.RegisterMessageBroker<QuestRewardClaimRequest>(options);
            builder.RegisterMessageBroker<RewardGrantRequest>(options);
            builder.RegisterMessageBroker<RewardGrantedEvent>(options);
            builder.RegisterMessageBroker<ShopPurchaseRequest>(options);
            builder.RegisterMessageBroker<ShopPurchaseCompletedEvent>(options);
            builder.RegisterMessageBroker<ShopPurchaseFailedEvent>(options);
            builder.RegisterMessageBroker<FishBookChangedEvent>(options);
            builder.RegisterMessageBroker<FishBookSelectionRequest>(options);
            builder.RegisterMessageBroker<FishBookSelectionChangedEvent>(options);
            builder.RegisterMessageBroker<ScoreChangedEvent>(options);
            builder.RegisterMessageBroker<ScoreSubmitRequest>(options);
            builder.RegisterMessageBroker<RankUpdatedEvent>(options);
            builder.RegisterMessageBroker<AdRequest>(options);
            builder.RegisterMessageBroker<AdCompletedEvent>(options);
            builder.RegisterMessageBroker<AdFailedEvent>(options);
            builder.RegisterMessageBroker<PlayerMovedEvent>(options);
            builder.RegisterMessageBroker<TutorialStartedEvent>(options);
            builder.RegisterMessageBroker<TutorialStepChangedEvent>(options);
            builder.RegisterMessageBroker<TutorialCompletedEvent>(options);
            builder.RegisterMessageBroker<PerformanceSnapshotEvent>(options);
            builder.RegisterMessageBroker<PerformanceWarningEvent>(options);
            builder.RegisterMessageBroker<EatRequest>(options);
            builder.RegisterMessageBroker<EatCompletedEvent>(options);
            builder.RegisterMessageBroker<SkillUseRequest>(options);
            builder.RegisterMessageBroker<SkillUsedEvent>(options);
            builder.RegisterMessageBroker<BuffRequest>(options);
            builder.RegisterMessageBroker<BuffAppliedEvent>(options);
            builder.RegisterMessageBroker<AudioVolumeRequest>(options);
            builder.RegisterMessageBroker<AudioVolumeChangedEvent>(options);

            builder.Register<SizeCheck>(Lifetime.Singleton)
                .WithParameter(1.1f);
            RegisterAudio(builder);
            builder.RegisterInstance(new SkillCatalog(_skillData));
            builder.RegisterInstance(_questData);
            builder.RegisterInstance(new ShopCatalog(_shopItems));
            builder.RegisterInstance(_fishBookData);
            builder.RegisterInstance(_scoreSettings);
            builder.RegisterInstance(_adSettings);
            builder.RegisterInstance(_performanceSettings);
            builder.RegisterInstance(_guideSteps);
            builder.RegisterInstance(Mathf.Max(1, _maxRankRecords));
            builder.Register<IAdService, MockAdService>(Lifetime.Singleton);
            builder.Register<UnlockInventory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ShopWallet>(Lifetime.Singleton)
                .AsSelf();
            builder.Register<SpeedBuff>(Lifetime.Singleton);
            builder.Register<ShieldBuff>(Lifetime.Singleton);
            builder.Register<ExpBuff>(Lifetime.Singleton);
            builder.Register<BuffManager>(Lifetime.Singleton);
            builder.RegisterInstance(new SaveSettings(_saveFileName, _autoSaveInterval));
            builder.Register<JsonSave>(Lifetime.Singleton);
            builder.Register<SaveManager>(Lifetime.Singleton);
            if (_hudDisplayData != null)
            {
                builder.RegisterInstance(HudDisplaySettings.FromData(_hudDisplayData));
                builder.RegisterEntryPoint<GameplayHudController>(Lifetime.Singleton);
            }

            RegisterVFX(builder);
            builder.RegisterEntryPoint<DamageSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DeathSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EatSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<BuffSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SkillSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<QuestManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<RewardSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ShopManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<FishBookManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<FishPreviewUI>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ScoreSystem>(Lifetime.Singleton)
                .AsSelf();
            builder.RegisterEntryPoint<RankSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AdManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AdRewardSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<TutorialSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<HighlightUI>(Lifetime.Singleton);
            builder.RegisterEntryPoint<PerformanceMonitor>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AutoSave>(Lifetime.Singleton);
        }

        private void RegisterSceneComponent<T>(IContainerBuilder builder)
            where T : Component
        {
            var component = GetSceneComponent<T>();
            if (component != null)
            {
                builder.RegisterComponent(component);
            }
        }

        private T GetSceneComponent<T>()
            where T : Component
        {
            var roots = gameObject.scene.GetRootGameObjects();
            for (var index = 0; index < roots.Length; index++)
            {
                var component = roots[index].GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private void RegisterAudio(IContainerBuilder builder)
        {
            var settings = _audioSettings != null
                ? _audioSettings.CreatePlaybackSettings()
                : AudioPlaybackSettings.CreateDefault();
            builder.RegisterInstance(settings);
            builder.RegisterInstance(
                new AudioMixerController(settings.VolumeSettings));
            builder.RegisterInstance(
                new AudioSourceBinding(
                    GetAudioSource(ref _bgmSource),
                    GetAudioSource(ref _sfxSource)));
            builder.RegisterEntryPoint<AudioManager>(Lifetime.Singleton);
        }

        private void RegisterVFX(IContainerBuilder builder)
        {
            var settings = _vfxSettings != null
                ? _vfxSettings.CreatePlaybackSettings()
                : new VFXPlaybackSettings(new VFXCue[0]);
            builder.RegisterInstance(settings);
            builder.RegisterInstance(new VFXPoolRoot(GetVFXPoolRoot()));
            builder.RegisterEntryPoint<VFXManager>(Lifetime.Singleton);
        }

        private Transform GetVFXPoolRoot()
        {
            if (_vfxPoolRoot != null)
            {
                return _vfxPoolRoot;
            }

            var root = new GameObject("VFXPoolRoot");
            root.transform.SetParent(transform, false);
            _vfxPoolRoot = root.transform;
            return _vfxPoolRoot;
        }

        private AudioSource GetAudioSource(ref AudioSource audioSource)
        {
            if (audioSource != null)
            {
                return audioSource;
            }

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            return audioSource;
        }
    }

    public sealed class SkillCatalog
    {
        private readonly SkillDataSO[] _skills;

        public SkillCatalog(SkillDataSO[] skills)
        {
            _skills = skills ?? new SkillDataSO[0];
        }

        public bool TryGet(
            SkillType skillType,
            out SkillDataSO skillData)
        {
            for (var index = 0; index < _skills.Length; index++)
            {
                skillData = _skills[index];
                if (skillData != null && skillData.SkillType == skillType)
                {
                    return true;
                }
            }

            skillData = null;
            return false;
        }
    }

    public sealed class SkillSystem : IStartable, ITickable, IDisposable
    {
        private readonly PlayerController _player;
        private readonly SkillCatalog _skillCatalog;
        private readonly ISubscriber<SkillUseRequest> _skillSubscriber;
        private readonly IPublisher<SkillUsedEvent> _skillUsedPublisher;
        private readonly ISkillEffect[] _effects;
        private readonly SkillCooldownSlot[] _cooldowns;
        private IDisposable _skillSubscription;

        public SkillSystem(
            PlayerController player,
            SkillCatalog skillCatalog,
            ISubscriber<SkillUseRequest> skillSubscriber,
            IPublisher<SkillUsedEvent> skillUsedPublisher,
            IPublisher<DamageRequest> damagePublisher,
            IPublisher<BuffRequest> buffPublisher)
        {
            _player = player;
            _skillCatalog = skillCatalog;
            _skillSubscriber = skillSubscriber;
            _skillUsedPublisher = skillUsedPublisher;
            _effects = CreateEffects(damagePublisher, buffPublisher);
            _cooldowns = CreateCooldownSlots();
        }

        public void Start()
        {
            _skillSubscription = _skillSubscriber.Subscribe(HandleSkillUse);
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
            _skillSubscription?.Dispose();
            _skillSubscription = null;
        }

        private void HandleSkillUse(SkillUseRequest request)
        {
            if (!CanHandle(request, out var skillType, out var skillData))
            {
                return;
            }

            var effect = GetEffect(skillType);
            if (effect == null || !effect.TryUse(_player, skillData, 1))
            {
                return;
            }

            var cooldown = skillData.GetCooldown(1);
            SetCooldown(skillType, Time.time + cooldown);
            _skillUsedPublisher.Publish(
                new SkillUsedEvent(_player, skillType, cooldown));
        }

        private bool CanHandle(
            SkillUseRequest request,
            out SkillType skillType,
            out SkillDataSO skillData)
        {
            skillType = GetPlayerSkillType(request);
            skillData = null;
            return request.Player == _player &&
                skillType != SkillType.None &&
                _skillCatalog.TryGet(skillType, out skillData) &&
                IsCooldownReady(skillType, Time.time);
        }

        private SkillType GetPlayerSkillType(SkillUseRequest request)
        {
            if (!request.IsValid || request.Player.FishData == null)
            {
                return SkillType.None;
            }

            return request.Player.FishData.Skill;
        }

        private ISkillEffect GetEffect(SkillType skillType)
        {
            for (var index = 0; index < _effects.Length; index++)
            {
                if (_effects[index].SkillType == skillType)
                {
                    return _effects[index];
                }
            }

            return null;
        }

        private bool IsCooldownReady(
            SkillType skillType,
            float time)
        {
            var slot = GetCooldownSlot(skillType);
            return slot == null || slot.ReadyAt <= time;
        }

        private void SetCooldown(
            SkillType skillType,
            float readyAt)
        {
            var slot = GetCooldownSlot(skillType);
            if (slot != null)
            {
                slot.ReadyAt = readyAt;
            }
        }

        private SkillCooldownSlot GetCooldownSlot(SkillType skillType)
        {
            for (var index = 0; index < _cooldowns.Length; index++)
            {
                if (_cooldowns[index].SkillType == skillType)
                {
                    return _cooldowns[index];
                }
            }

            return null;
        }

        private static ISkillEffect[] CreateEffects(
            IPublisher<DamageRequest> damagePublisher,
            IPublisher<BuffRequest> buffPublisher)
        {
            return new ISkillEffect[]
            {
                new DashSkillEffect(),
                new SonarSkillEffect(damagePublisher),
                new ShieldSkillEffect(buffPublisher),
                new FrenzySkillEffect(buffPublisher)
            };
        }

        private static SkillCooldownSlot[] CreateCooldownSlots()
        {
            return new SkillCooldownSlot[]
            {
                new SkillCooldownSlot(SkillType.Dash),
                new SkillCooldownSlot(SkillType.Sonar),
                new SkillCooldownSlot(SkillType.Shield),
                new SkillCooldownSlot(SkillType.Frenzy)
            };
        }
    }

    public interface ISkillEffect
    {
        SkillType SkillType { get; }

        bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level);
    }

    public sealed class SkillCooldownSlot
    {
        public SkillCooldownSlot(SkillType skillType)
        {
            SkillType = skillType;
        }

        public SkillType SkillType { get; }
        public float ReadyAt { get; set; }
    }

    public sealed class DashSkillEffect : ISkillEffect
    {
        public SkillType SkillType => SkillType.Dash;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            var distance = skillData.GetEffectValue(level);
            if (player == null || distance <= 0f)
            {
                return false;
            }

            player.Dash(distance);
            return true;
        }
    }

    public sealed class SonarSkillEffect : ISkillEffect
    {
        private const int MaxHits = 32;

        private readonly IPublisher<DamageRequest> _damagePublisher;
        private readonly Collider2D[] _hits = new Collider2D[MaxHits];

        public SonarSkillEffect(IPublisher<DamageRequest> damagePublisher)
        {
            _damagePublisher = damagePublisher;
        }

        public SkillType SkillType => SkillType.Sonar;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (!CanUse(player, skillData))
            {
                return false;
            }

            var damage = GetDamage(player, skillData, level);
            var filter = ContactFilter2D.noFilter;
            var count = Physics2D.OverlapCircle(
                player.transform.position,
                skillData.Range,
                filter,
                _hits);
            PublishHits(player, damage, count);
            return true;
        }

        private bool CanUse(
            PlayerController player,
            SkillDataSO skillData)
        {
            return player != null &&
                skillData.Range > 0f &&
                _damagePublisher != null;
        }

        private int GetDamage(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            var baseDamage = GetBaseDamage(player);
            var value = baseDamage * skillData.GetEffectValue(level);
            return Mathf.Max(1, Mathf.RoundToInt(value));
        }

        private int GetBaseDamage(PlayerController player)
        {
            return player.TryGetComponent<AttackComponent>(out var attack)
                ? attack.AttackDamage
                : 1;
        }

        private void PublishHits(
            PlayerController player,
            int damage,
            int count)
        {
            for (var index = 0; index < count; index++)
            {
                TryPublishHit(player, damage, _hits[index]);
                _hits[index] = null;
            }
        }

        private void TryPublishHit(
            PlayerController player,
            int damage,
            Collider2D hit)
        {
            if (hit == null ||
                hit.gameObject == player.gameObject ||
                !hit.TryGetComponent<HealthComponent>(out var health))
            {
                return;
            }

            _damagePublisher.Publish(
                new DamageRequest(player.gameObject, health, damage));
        }
    }

    public sealed class ShieldSkillEffect : ISkillEffect
    {
        private readonly IPublisher<BuffRequest> _buffPublisher;

        public ShieldSkillEffect(IPublisher<BuffRequest> buffPublisher)
        {
            _buffPublisher = buffPublisher;
        }

        public SkillType SkillType => SkillType.Shield;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (player == null)
            {
                return false;
            }

            _buffPublisher.Publish(
                new BuffRequest(
                    player,
                    BuffType.Shield,
                    skillData.GetEffectValue(level),
                    skillData.Duration));
            return true;
        }
    }

    public sealed class FrenzySkillEffect : ISkillEffect
    {
        private readonly IPublisher<BuffRequest> _buffPublisher;

        public FrenzySkillEffect(IPublisher<BuffRequest> buffPublisher)
        {
            _buffPublisher = buffPublisher;
        }

        public SkillType SkillType => SkillType.Frenzy;

        public bool TryUse(
            PlayerController player,
            SkillDataSO skillData,
            int level)
        {
            if (player == null)
            {
                return false;
            }

            _buffPublisher.Publish(
                new BuffRequest(
                    player,
                    BuffType.Frenzy,
                    skillData.GetEffectValue(level) - 1f,
                    skillData.Duration));
            return true;
        }
    }
}
