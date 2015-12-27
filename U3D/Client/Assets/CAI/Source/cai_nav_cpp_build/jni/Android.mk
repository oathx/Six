# Copyright (C) 2009 The Android Open Source Project
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE    := cai-nav-rcn
LOCAL_C_INCLUDES := $(LOCAL_PATH)/nav-rcn/Detour/Include \
					$(LOCAL_PATH)/nav-rcn/DetourCrowd/Include \
					$(LOCAL_PATH)/nav-rcn/Nav/Include \
					
LOCAL_SRC_FILES := nav-rcn/Detour/Source/DetourAlloc.cpp \
					nav-rcn/Detour/Source/DetourCommon.cpp \
					nav-rcn/Detour/Source/DetourNavMesh.cpp \
					nav-rcn/Detour/Source/DetourNavMeshBuilder.cpp \
					nav-rcn/Detour/Source/DetourNavMeshQuery.cpp \
					nav-rcn/Detour/Source/DetourNode.cpp \
					nav-rcn/DetourCrowd/Source/DetourCrowd.cpp \
					nav-rcn/DetourCrowd/Source/DetourLocalBoundary.cpp \
					nav-rcn/DetourCrowd/Source/DetourObstacleAvoidance.cpp \
					nav-rcn/DetourCrowd/Source/DetourPathCorridor.cpp \
					nav-rcn/DetourCrowd/Source/DetourPathQueue.cpp \
					nav-rcn/DetourCrowd/Source/DetourProximityGrid.cpp \
					nav-rcn/Nav/Source/DetourCrowdEx.cpp \
					nav-rcn/Nav/Source/DetourNavMeshBuildEx.cpp \
					nav-rcn/Nav/Source/DetourNavmeshEx.cpp \
					nav-rcn/Nav/Source/DetourNavMeshQueryEx.cpp \
					nav-rcn/Nav/Source/DetourPathCorridorEx.cpp \
					nav-rcn/Nav/Source/DetourQueryFilterEx.cpp \
					nav-rcn/Nav/Source/NavValidation.cpp \

include $(BUILD_SHARED_LIBRARY)

include $(CLEAR_VARS)

LOCAL_MODULE    := cai-nmgen-rcn
LOCAL_C_INCLUDES := $(LOCAL_PATH)/nmgen-rcn/NMGen/Include \
					$(LOCAL_PATH)/nmgen-rcn/Recast/Include \
					
LOCAL_SRC_FILES := nmgen-rcn/NMGen/Source/BuildContext.cpp \
					nmgen-rcn/NMGen/Source/CompactHeightfieldEx.cpp \
					nmgen-rcn/NMGen/Source/ContoursEx.cpp \
					nmgen-rcn/NMGen/Source/HeightfieldEx.cpp \
					nmgen-rcn/NMGen/Source/HeightfieldLayerSet.cpp \
					nmgen-rcn/NMGen/Source/NMGen.cpp \
					nmgen-rcn/NMGen/Source/PolyMeshDetailEx.cpp \
					nmgen-rcn/NMGen/Source/PolyMeshEx.cpp \
					nmgen-rcn/Recast/Source/Recast.cpp \
					nmgen-rcn/Recast/Source/RecastAlloc.cpp \
					nmgen-rcn/Recast/Source/RecastArea.cpp \
					nmgen-rcn/Recast/Source/RecastContour.cpp \
					nmgen-rcn/Recast/Source/RecastFilter.cpp \
					nmgen-rcn/Recast/Source/RecastLayers.cpp \
					nmgen-rcn/Recast/Source/RecastMesh.cpp \
					nmgen-rcn/Recast/Source/RecastMeshDetail.cpp \
					nmgen-rcn/Recast/Source/RecastRasterization.cpp \
					nmgen-rcn/Recast/Source/RecastRegion.cpp \

include $(BUILD_SHARED_LIBRARY)