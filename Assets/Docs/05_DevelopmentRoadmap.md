# DevelopmentRoadmap.md

# Fish Evolution 开发路线图

版本：

V1.0

目标：

完成可上线的单机版大鱼吃小鱼。

开发原则：

每个阶段必须：

* 可编译
* 可运行
* 可测试
* 可提交Git

禁止跨阶段开发。

---

# Phase 0 项目初始化

目标：

创建基础工程。

任务：

* Unity6项目
* URP配置
* Input System
* Addressables
* Git仓库
* GitIgnore
* 文件夹结构

验收：

项目可正常启动。

---

# Phase 1 Core框架

目标：

搭建基础架构。

开发内容：

GameBootstrap

GameManager

SceneLoader

EventBus

GameStateMachine

ConfigManager

验收：

游戏可以进入主菜单。

---

# Phase 2 数据层

目标：

建立数据驱动架构。

开发内容：

FishDataSO

SkillDataSO

BossDataSO

MapDataSO

ItemDataSO

验收：

所有数据通过SO读取。

禁止硬编码。

---

# Phase 3 输入系统

目标：

玩家可控制鱼移动。

开发内容：

InputActions

PlayerInput

PlayerController

移动系统

旋转系统

验收：

鱼可自由移动。

---

# Phase 4 摄像机系统

目标：

实现动态跟随。

开发内容：

CameraController

CameraZoom

SmoothFollow

验收：

玩家变大时镜头拉远。

---

# Phase 5 食物系统

目标：

海洋中出现可吞噬目标。

开发内容：

FoodSpawner

FoodController

FoodData

对象池

验收：

食物随机生成。

玩家可吞噬。

---

# Phase 6 成长系统

目标：

等级成长。

开发内容：

ExpSystem

LevelSystem

GrowthSystem

验收：

吞噬获得经验。

经验满升级。

体型变化。

---

# Phase 7 鱼类系统

目标：

普通鱼生成。

开发内容：

FishSpawner

FishController

FishData

验收：

地图出现鱼群。

---

# Phase 8 AI系统

目标：

鱼拥有智能行为。

开发内容：

StateMachine

Patrol

Escape

Chase

Attack

验收：

小鱼逃跑。

大鱼追击。

---

# Phase 9 战斗系统

目标：

支持伤害。

开发内容：

HealthComponent

AttackComponent

DamageSystem

DeathSystem

验收：

可死亡。

可复活。

---

# Phase 10 吞噬系统

目标：

核心玩法完成。

开发内容：

EatSystem

SizeCheck

CollisionSystem

验收：

大鱼吃小鱼。

---

# Phase 11 HUD

目标：

游戏主界面。

开发内容：

等级

经验

金币

技能

小地图

验收：

HUD完整显示。

---

# Phase 12 技能系统

目标：

实现主动技能。

开发内容：

Dash

Sonar

Shield

Frenzy

验收：

技能正常释放。

---

# Phase 13 Buff系统

目标：

支持状态效果。

开发内容：

BuffManager

SpeedBuff

ShieldBuff

ExpBuff

验收：

Buff正常叠加。

---

# Phase 14 存档系统

目标：

数据持久化。

开发内容：

SaveManager

JsonSave

AutoSave

验收：

退出后保留进度。

---

# Phase 15 音频系统

目标：

游戏声音。

开发内容：

AudioManager

BGM

SFX

Mixer

验收：

支持音量控制。

---

# Phase 16 特效系统

目标：

增强反馈。

开发内容：

VFXManager

LevelUpFX

EatFX

DeathFX

验收：

特效正常播放。

---

# Phase 17 Boss系统

目标：

首个Boss。

开发内容：

BossController

BossFSM

BossSkills

验收：

Boss可挑战。

---

# Phase 18 地图系统

目标：

区域切换。

开发内容：

MapManager

AreaTrigger

FogSystem

验收：

解锁新区域。

---

# Phase 19 任务系统

目标：

提供目标驱动。

开发内容：

DailyQuest

Achievement

RewardSystem

验收：

任务可完成。

---

# Phase 20 商店系统

目标：

消耗货币。

开发内容：

ShopManager

FishUnlock

SkinUnlock

验收：

可购买内容。

---

# Phase 21 鱼类图鉴

目标：

收藏玩法。

开发内容：

FishBook

UnlockState

PreviewUI

验收：

图鉴完整展示。

---

# Phase 22 排行榜

目标：

本地排行榜。

开发内容：

ScoreSystem

RankSystem

验收：

记录历史成绩。

---

# Phase 23 广告系统

目标：

商业化。

开发内容：

RewardAds

ReviveAds

DoubleGoldAds

验收：

广告奖励正常。

---

# Phase 24 新手引导

目标：

提升留存。

开发内容：

TutorialSystem

GuideStep

HighlightUI

验收：

首次进入自动引导。

---

# Phase 25 优化阶段

目标：

稳定60FPS。

开发内容：

Profiler分析

GC优化

DrawCall优化

对象池优化

验收：

中低端设备稳定运行。

---

# Phase 26 Beta版本

目标：

完整可玩。

验收：

从开始到Boss全流程打通。

---

# Phase 27 Release Candidate

目标：

上线准备。

开发内容：

Bug修复

平衡性调整

崩溃修复

验收：

无严重Bug。

---

# Phase 28 Release

目标：

正式上线。

输出：

Android APK

iOS IPA

WebGL

版本号：

1.0.0

项目完成。

---

# AI执行规则

当我向你提出开发需求时：

必须遵循以下流程：

1. 先判断当前属于哪个Phase

2. 只开发当前Phase内容

3. 不允许开发后续Phase

4. 输出：

* 架构设计
* 类图说明
* 文件结构
* 代码实现
* Inspector配置
* 测试方案

5. 完成后等待下一步

禁止一次生成整个项目。
