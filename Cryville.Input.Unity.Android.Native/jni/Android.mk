LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
LOCAL_MODULE := AndroidInputProxy
LOCAL_SRC_FILES := AndroidInputProxy.cpp
include $(BUILD_SHARED_LIBRARY)