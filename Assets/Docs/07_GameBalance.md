# GameBalance.md

# Fish Evolution 数值平衡文档

版本：

1.0

---

# 设计原则

成长明显

前期快

中期稳

后期慢

避免数值膨胀。

---

# 等级经验曲线

公式：

EXP = 100 × Level^1.5

---

# 等级区间

Lv1-10

新手期

预计：

10分钟

---

Lv11-30

成长期

预计：

40分钟

---

Lv31-50

中期

预计：

2小时

---

Lv51-80

后期

预计：

5小时

---

Lv81-100

终局

预计：

10小时+

---

# HP成长

公式：

HP =
100 + Level × 25

---

# 攻击成长

公式：

Attack =
10 + Level × 5

---

# 体型成长

公式：

Scale =
1 + Level × 0.12

---

# 速度成长

公式：

Speed =
6 - Level × 0.03

最小：

2

---

# 吞噬判定

PlayerScale >

TargetScale × 1.1

允许吞噬

---

PlayerScale ≤

TargetScale × 1.1

无法吞噬

---

# 食物经验

Plankton

1

---

Shrimp

3

---

SmallFish

10

---

MediumFish

30

---

BigFish

80

---

Predator

150

---

# Boss数值

## Boss1

Octopus King

HP

5000

Attack

100

---

## Boss2

Deep Shark

HP

15000

Attack

200

---

## Boss3

Ice Whale

HP

50000

Attack

500

---

## Boss4

Leviathan

HP

200000

Attack

1000

---

# 金币掉落

普通鱼

1-5

---

中型鱼

10-20

---

大型鱼

20-50

---

Boss

500-5000

---

# 商城平衡

金币包

1000

￥6

---

5000

￥18

---

10000

￥30

---

钻石包

100

￥6

---

500

￥30

---

1000

￥68

---

# 广告奖励

观看一次：

金币 ×2

持续：

10分钟

---

复活广告：

每日3次

---

# 新手保护

前10分钟：

受到伤害降低50%

经验增加30%

---

# AI难度曲线

浅海：

20%攻击性

---

珊瑚区：

40%

---

深海：

60%

---

冰海：

80%

---

远古海域：

100%

---

# 留存目标

Day1

40%

---

Day3

20%

---

Day7

10%

---

Day30

3%

---

# 平衡调整原则

优先调整：

经验

金币

掉率

AI行为

禁止频繁修改：

HP

Attack

Scale

避免版本震荡。
