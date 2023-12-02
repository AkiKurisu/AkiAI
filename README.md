# AkiAI
AkiAI 是一个个人向、实验性的游戏AI框架，可用于设计和开发游戏中的NPC、敌人AI等。

## 依赖
1. [AkiBT](https://github.com/AkiKurisu/AkiBT)
2. [AkiGOAP](https://github.com/AkiKurisu/AkiGOAP)

## 架构

分为数据层、决策层、行为层

<image src="Images/Framework.png">

## 数据层

数据层主要分为世界状态（WorldState）、黑板（BlackBoard）

### 世界状态
世界状态和GOAP中的定义一致，用于抽象游戏世界产生的变化至一个状态值（例如“疲惫”:True即表示角色处于“疲惫”的抽象状态），这样决策层只需要了解世界状态，而不需要了解具体的数值

### 黑板
黑板是存放公共变量的工具，从而使行动层可以访问相同资源

## 决策层

决策层分为规划器（Planner）、目标（Goal）和行为（Action），AkiAI中使用GOAP来规划任务

详见[GOAP](https://github.com/AkiKurisu/AkiGOAP)

## 行为层
行为层以任务的方式分布，AkiAI中可以选择使用行为树来执行任务
### 观察任务
观察任务用于观测世界变化并修改世界状态，由于大部分情况下观察仅需要在特殊的周期中进行（例如角色已经处于“眩晕”时无需再次观察“眩晕”，而是在角色恢复状态后继续观察），因此可以将其试做一个行动任务。

### 行为任务
行为任务应当服务于一个独立的`行为（Action）`

