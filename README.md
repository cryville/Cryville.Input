[普通话（简体）](README_cmn-hans.md)

# Cryville.Input
Project #A013 [cin] Cryville.Input is a realtime input library under .NET Framework 3.5.

This project was created on 2023-04-30. It has been open-sourced since 2023-05-05.

## Building
There is no release at the moment, so you need to build the project by yourself.

### Building the core library
1. Clone [Cryville.Common](https://github.com/cryville/Cryville.Common) and [Cryville.Input](https://github.com/cryville/Cryville.Input) into the same folder.
2. Open Cryville.Common.sln in the Cryville.Common repository and build the solution.
3. Open Cryville.Input.sln in the Cryville.Input repository and build the solution.
4. The output libraries and documentations should now be in `Cryville.Input/bin/Release`.

### Setting up the core library in Unity
Simply copy all the files in `Cryville.Input/bin/Release` to the `Assets/Plugins` folder in your unity project.

### Setting up Unity built-in input in Unity
Simply copy all the contents in `Cryville.Input.Unity.Builtin` to the `Assets/Plugins/Cryville.Input.Unity.Builtin` folder in your unity project.

### Setting up Android native input in Unity
1. Copy all the contents in `Cryville.Input.Unity.Android` **except the `Native` folder** to the `Assets/Plugins/Cryville.Input.Unity.Android` folder in your unity project.
2. Make sure you have Android NDK installed. Open a command prompt in `Cryville.Input.Unity.Android/Native` (as the working directory) and run `ndk-build` in your Android NDK.
3. Copy the two folders in `Cryville.Input.Unity.Android/Native/libs` to the `Assets/Plugins/Android` folder in your unity project.

## Usage
The simplest way to use the library is to construct a `SimpleInputConsumer` and check new input events periodically, typically every game tick. You need to construct an `InputManager` before constructing a `SimpleInputConsumer`, and before that, you need to call `InputManager.HandlerRegistries.Add()` for every handler types you want to use. See the following example:

```cs
SimpleInputConsumer consumer;
Action<InputEvent> d_eventcb;
void Start() {
	InputManager.HandlerRegistries.Add(typeof(AndroidTouchHandler)); // Register AndroidTouchHandler
	InputManager manager = new InputManager();
	consumer = new(manager);
	consumer.Activate();
	d_eventcb = HandleInputEvent;
}
void Update() {
	consumer.EnumerateEvents(d_eventcb);
}
void HandleInputEvent(InputEvent ev) {
	// Handle input events here
}
```

## Supported input devices
### Unity built-in input
- [x] Unity GUI input (via `Event` in `UnityEngine.IMGUIModule`)
- [x] Unity mouse input
- [x] Unity touch input

### Android native input (via `UnityEngine.AndroidJNIModule`)
- [x] Android sensor
  - [x] Accelerometer
  - [x] Accelerometer Uncalibrated
  - [ ] Ambient Temperature
  - [ ] Geomagnetic Rotation Vector
  - [x] Gravity
  - [x] Gyroscope
  - [ ] Heart Beat
  - [ ] Heart Rate
  - [ ] Light
  - [x] Linear Acceleration
  - [x] Magnetic Field
  - [x] Magnetic Field Uncalibrated
  - [ ] Pressure
  - [ ] Proximity
  - [ ] Relative Humidity
  - [x] Rotation Vector
  - [ ] Step Counter
- [x] Android touch

### Windows native input
- [ ] Windows key (`WM_KEY*`)
- [ ] Windows mouse (`WM_MOUSE*`)
- [ ] Windows pointer (`WM_POINTER*`)
- [ ] Windows touch (`WM_TOUCH`)

## FAQ
### An error occurred while running `ndk-build`: Expected two items in TARGET\_TOOLCHAIN\_LIST.
Go into the `.mk` file shown in the error message and modify the line `ifneq ($(words $(TARGET_TOOLCHAIN_LIST)), 1)` to `ifneq ($(words $(TARGET_TOOLCHAIN_LIST)), 2)`.

### What coordinate system do the input handlers use?
The coordinate system an input handler uses depends on its underlying API. Use `ReferenceCue.InverseTransform(InputFrame)` to transform a received frame to a universal reference. If the device has a boundary, use `ReferenceCue.InverseTransform(InputFrame, InputVector)` instead and specify the size in the second parameter.
