# AI编码规范

你是项目唯一开发者。

所有代码必须符合以下规则。

---

# 第零原则

项目已集成：

- VContainer
- UniTask
- MessagePipe
- DOTween
- Addressables

生成代码时必须优先使用上述框架。

禁止自行实现重复功能。

---

# 第一原则

优先可维护性

优先扩展性

优先性能

禁止为了快速实现牺牲架构

---

# VContainer规范

所有Manager和Service必须注册到LifetimeScope。

禁止：

public static Instance

GameManager.Instance

AudioManager.Instance

---

# UniTask规范

异步代码统一使用UniTask。

禁止：

IEnumerator

Task

async void

---

# MessagePipe规范

系统通信统一使用MessagePipe。

禁止：

直接跨模块引用。

例如：

UI → Player

UI → Combat

必须通过消息发送。

---

# DOTween规范

所有动画统一使用DOTween。

禁止：

Update动画

Coroutine动画

手写Lerp动画

---

# Unity规范

Unity6

URP

Input System

Addressables

ScriptableObject

---

# 类设计规范

单个类职责唯一

遵守SOLID

禁止God Class

---

# 方法规范

单个方法：

不超过50行

单个类：

不超过500行

---

# 命名规范

私有变量：

_playerData

公有属性：

PlayerData

接口：

IPlayerData

枚举：

FishState

---

# 禁止使用

GameObject.Find

FindObjectOfType

Resources.Load

协程滥用

Update滥用

硬编码字符串

硬编码路径

---

# 必须使用

对象池

事件总线

配置表

依赖注入预留

---

# 输出代码要求

每次只生成一个系统。

禁止一次生成整个项目。

顺序：

1. 数据层
2. 核心层
3. 管理器
4. Gameplay
5. AI
6. Combat
7. UI

---

# 代码生成要求

生成代码前：

说明依赖关系

生成代码后：

说明挂载位置

说明Inspector配置

说明测试步骤

---

# Bug修复规则

优先修复根因

禁止临时修复

禁止增加耦合

---

# 最终目标

保持项目：

零红字

零警告

可编译

可扩展

商业级质量
