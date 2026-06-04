# Unity6 技术架构文档

# 架构原则

遵循：

SOLID

模块化

事件驱动

数据驱动

禁止强耦合

---

# 技术基础设施

项目必须使用以下框架：

## Dependency Injection

VContainer

用途：

- Manager注册
- Service注册
- 生命周期管理
- 场景依赖注入

禁止：

new Manager()

FindObjectOfType()

Singleton泛滥

---

## Async Framework

UniTask

用途：

- 场景加载
- Addressables加载
- 网络请求
- UI动画等待

禁止：

Task

Coroutine滥用

优先使用：

UniTask

UniTaskVoid

CancellationToken

---

## Event System

MessagePipe

用途：

- UI通信
- Gameplay事件
- 系统解耦

事件示例：

PlayerLevelUpEvent

FishEatEvent

BossDeadEvent

QuestCompletedEvent

禁止：

直接引用Manager通信

---

## Tween System

DOTween

用途：

- UI动画
- 相机动画
- 特效动画
- 数值跳动

禁止：

手写Lerp动画

优先：

DOMove

DOScale

DOFade

DOShakePosition

---

## Resource Management

Addressables

所有运行时资源必须通过Addressables加载。

禁止：

Resources.Load

---

# 固定技术栈

AI生成代码时不得替换。

DI：
VContainer

Async：
UniTask

EventBus：
MessagePipe

Animation：
DOTween

Config：
ScriptableObject

Resource：
Addressables

Input：
Input System

Serialization：
Newtonsoft Json

---

# 项目结构

Scripts

├── Core
├── Gameplay
├── AI
├── Combat
├── UI
├── Managers
├── Audio
├── Save
├── Pool
├── Config
└── Utilities

---

# Core层

GameEntry

GameBootstrap

SceneLoader

EventBus

GameStateMachine

---

# Manager层

GameManager

UIManager

AudioManager

SaveManager

PoolManager

ConfigManager

AddressableManager

---

# Gameplay层

PlayerController

FishController

FishSpawner

MapManager

QuestManager

SkillManager

---

# AI层

FishAIController

StateMachine

PatrolState

ChaseState

EscapeState

AttackState

DeadState

---

# Combat层

CombatSystem

DamageSystem

BuffSystem

HealthComponent

AttackComponent

---

# UI层

MVVM模式

View

ViewModel

Model

禁止业务逻辑写UI

---

# 数据层

全部采用ScriptableObject

FishDataSO

SkillDataSO

MapDataSO

BossDataSO

ItemDataSO

---

# Addressables

必须管理：

Prefab

Audio

UI

VFX

Sprite

禁止Resources加载核心资源

---

# 对象池

FishPool

FoodPool

EffectPool

BossSkillPool

必须支持预热

必须支持动态扩容

---

# 存档

Json

AES加密预留

云存档接口预留

---

# 性能目标

300 AI

1000 Food

60 FPS

GC Alloc 接近0

禁止频繁Instantiate
