#include <jni.h>
#include <map>
#include <stdint.h>

typedef void (*AndroidInputProxy_Callback) (
	int32_t id,
	int32_t action,
	int64_t time,
	float x,
	float y,
	float z,
	float w
	);

std::map<int32_t, AndroidInputProxy_Callback> _callbacks;

extern "C" {
	JNIEXPORT void JNICALL Java_world_cryville_input_unity_android_NativeMethods_feed(JNIEnv* env, jobject clazz, jint hid, jint id, jint action, jlong time, jfloat x, jfloat y, jfloat z, jfloat w) {
		auto cb = _callbacks.find(hid);
		if (cb != _callbacks.end()) cb->second(id, action, time, x, y, z, w);
	}

	void AndroidInputProxy_RegisterCallback(int32_t hid, AndroidInputProxy_Callback cb) {
		_callbacks[hid] = cb;
	}
}
