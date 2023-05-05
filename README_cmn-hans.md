[English](README.md)

# Cryville.Input
项目 #A013 [cin] Cryville.Input 是一个 .NET Framework 3.5 下的实时输入库。

本项目于 2023-04-30 创建，于 2023-05-05 开源。

## 构建
目前该项目没有 release，您需要自己构建该项目。

### 构建核心库
1. 将 [Cryville.Common](https://github.com/cryville/Cryville.Common) 和 [Cryville.Input](https://github.com/cryville/Cryville.Input) 克隆至同一文件夹。
2. 打开 Cryville.Common 仓库内的 Cryville.Common.sln 并构建解决方案。
3. 打开 Cryville.Input 仓库内的 Cryville.Input.sln 并构建解决方案。
4. 输出的库和文档可以在 `Cryville.Input/bin/Release` 找到。

### 在 Unity 中配置核心库
直接复制 `Cryville.Input/bin/Release` 文件夹内的所有文件到 Unity 项目的 `Assets/Plugins` 文件夹内。

### 在 Unity 中配置 Unity 内置输入
直接复制 `Cryville.Input.Unity.Builtin` 文件夹内的所有内容到 Unity 项目的 `Assets/Plugins/Cryville.Input.Unity.Builtin` 文件夹内。

### 在 Unity 中配置 Android 本地输入
1. 复制 `Cryville.Input.Unity.Android` 文件夹内**除了 `Native` 文件夹**的所有内容到 Unity 项目的 `Assets/Plugins/Cryville.Input.Unity.Android` 文件夹内。
2. 确认安装了 Android NDK。在 `Cryville.Input.Unity.Android/Native` 文件夹打开命令行（作为工作目录）运行 Android NDK 内的 `ndk-build`。
3. 复制 `Cryville.Input.Unity.Android/Native/libs` 中的两个文件夹到 Unity 项目的 `Assets/Plugins/Android` 文件夹内。

## 使用
使用本库最简单的方法是创建一个 `SimpleInputConsumer` 并周期性（通常是每个游戏刻）检查新的输入事件。创建 `SimpleInputConsumer` 前需要创建一个 `InputManager`，在那之前，需要对想要使用的输入处理器类型调用 `InputManager.HandlerRegistries.Add()`。如下面的例子所示：

```cs
SimpleInputConsumer consumer;
Action<InputEvent> d_eventcb;
void Start() {
	InputManager.HandlerRegistries.Add(typeof(AndroidTouchHandler)); // 注册 AndroidTouchHandler
	InputManager manager = new InputManager();
	consumer = new(manager);
	consumer.Activate();
	d_eventcb = HandleInputEvent;
}
void Update() {
	consumer.EnumerateEvents(d_eventcb);
}
void HandleInputEvent(InputEvent ev) {
	// 在此处处理输入事件
}
```

## 支持输入设备
### Unity 内置输入
- [x] Unity GUI 输入（通过 `UnityEngine.IMGUIModule` 的 `Event`）
- [x] Unity 鼠标输入
- [x] Unity 触控输入

### Android 本地输入（通过 `UnityEngine.AndroidJNIModule`）
- [x] Android 传感器
  - [x] 加速度计
  - [x] 未校准的加速度计
  - [ ] 环境温度
  - [ ] 地磁旋转矢量
  - [x] 重力计
  - [x] 陀螺仪
  - [ ] 心跳
  - [ ] 心率
  - [ ] 光照
  - [x] 线性加速度计
  - [x] 磁场计
  - [x] 未校准的磁场计
  - [ ] 压力
  - [ ] 距离
  - [ ] 相对湿度
  - [x] 旋转矢量
  - [ ] 计步器
- [x] Android 触控

### Windows 本地输入
- [ ] Windows 按键（`WM_KEY`）
- [ ] Windows 鼠标（`WM_MOUSE`）
- [ ] Windows 指针（`WM_POINTER*`）
- [ ] Windows 触控（`WM_TOUCH`）
