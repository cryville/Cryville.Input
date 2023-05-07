#include <jni.h>
#include <mutex>
#include <queue>
#include <stdint.h>

using namespace std;

struct ProxiedInputFrame {
	int32_t hid;
	int32_t id;
	int32_t action;
	int64_t time;
	float x;
	float y;
	float z;
	float w;
};

queue<ProxiedInputFrame> _aip_frame_queue;
mutex _aip_lock;

extern "C" {
	JNIEXPORT void JNICALL Java_world_cryville_input_unity_android_NativeMethods_feed(JNIEnv* env, jobject clazz, jint hid, jint id, jint action, jlong time, jfloat x, jfloat y, jfloat z, jfloat w) {
		lock_guard<mutex> lock(_aip_lock);
		_aip_frame_queue.push({ hid, id, action, time, x, y, z, w });
	}

	int32_t AndroidInputProxy_Poll(ProxiedInputFrame* result) {
		lock_guard<mutex> lock(_aip_lock);
		if (_aip_frame_queue.empty()) return 0;
		*result = _aip_frame_queue.front();
		_aip_frame_queue.pop();
		return 1;
	}
}
