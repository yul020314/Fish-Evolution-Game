# 项目上下文

当前项目技术栈：

Unity 6
Universal 2D

VContainer
UniTask
MessagePipe
DOTween
Addressables
Input System
Newtonsoft Json

架构：

MVVM
DI
Event Driven
Data Driven

开发原则：

所有依赖通过VContainer注入。

所有异步使用UniTask。

所有事件使用MessagePipe。

所有动画使用DOTween。

优先使用 UniTask，仅在必须与Unity生命周期深度绑定时使用Coroutine。

禁止使用Singleton。
禁止FindObjectOfType。
禁止Resources.Load。
