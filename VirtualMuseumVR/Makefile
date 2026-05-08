# =============================================================================
# Makefile — Virtual Museum VR — Gradle-based Build System
# =============================================================================
# Two-phase build:
#   Phase 1 (Unity CLI) : C# compile + IL2CPP + export Gradle project
#   Phase 2 (Gradle)    : Package → APK/AAB
#
# Commands:
#   make run             → Open project in Unity Editor (Play with XR Simulator)
#   make build-desktop   → Build macOS/Windows app for local testing (no headset)
#   make open-desktop    → Launch the built desktop app
#   make export          → Phase 1: Unity exports Gradle project
#   make apk             → Phase 2: Gradle builds release APK
#   make apk-debug       → Phase 2: Gradle builds debug APK
#   make aab             → Phase 2: Gradle builds release AAB (app bundle)
#   make build           → Phase 1 + Phase 2 (full pipeline)
#   make deploy          → Full build + push to Quest via ADB
#   make install         → Push existing APK to Quest
#   make logcat          → Stream Unity logs from connected Quest
#   make clean           → Remove build outputs
# =============================================================================

# ─── Configuration ────────────────────────────────────────────────────────────
UNITY_VERSION  := 6000.4.5f1
UNITY_PATH     := /Applications/Unity/Hub/Editor/$(UNITY_VERSION)/Unity.app/Contents/MacOS/Unity
PROJECT_PATH   := $(shell pwd)
EXPORT_DIR     := $(PROJECT_PATH)/Builds/GradleExport
BUILD_LOG      := /tmp/unity_build.log
GRADLE_LOG     := /tmp/gradle_build.log

# APK output paths (Gradle puts them here after assembling)
APK_RELEASE    := $(EXPORT_DIR)/launcher/build/outputs/apk/release/launcher-release.apk
APK_DEBUG      := $(EXPORT_DIR)/launcher/build/outputs/apk/debug/launcher-debug.apk
AAB_RELEASE    := $(EXPORT_DIR)/launcher/build/outputs/bundle/release/launcher-release.aab

# Signing (set these as environment variables for CI, or fill in for local builds)
KEYSTORE_PATH  ?= $(PROJECT_PATH)/release.keystore
KEYSTORE_PASS  ?= 
KEY_ALIAS      ?= 
KEY_PASS       ?= 

# ADB for Quest deployment
ADB            := adb
PACKAGE_NAME   := com.virtualmuseum.vr
ACTIVITY       := com.unity3d.player.UnityPlayerActivity

# Build metadata
VERSION_NAME   ?= 1.0.0
VERSION_CODE   ?= 1

# Gradle wrapper
GRADLEW        := $(EXPORT_DIR)/gradlew

.PHONY: all export apk apk-debug aab build deploy install uninstall \
        launch logcat clean clean-export clean-all check-unity check-adb help

# ─── Default ──────────────────────────────────────────────────────────────────
all: build

# ─────────────────────────────────────────────────────────────────────────────
# Phase 1: Unity Export
# ─────────────────────────────────────────────────────────────────────────────

## Phase 1: Compile C# and export Gradle project from Unity
export: check-unity
	@echo ""
	@echo "══════════════════════════════════════════════"
	@echo "  Phase 1: Unity → Gradle Export"
	@echo "══════════════════════════════════════════════"
	@mkdir -p "$(EXPORT_DIR)"
	$(UNITY_PATH) \
		-batchmode \
		-nographics \
		-projectPath "$(PROJECT_PATH)" \
		-buildTarget Android \
		-executeMethod GradleExportScript.Export \
		-exportPath "$(EXPORT_DIR)" \
		-version "$(VERSION_NAME)" \
		-versionCode "$(VERSION_CODE)" \
		-logFile "$(BUILD_LOG)" \
		-quit
	@if [ -f "$(GRADLEW)" ]; then \
		chmod +x "$(GRADLEW)"; \
		echo "✅ Gradle project exported: $(EXPORT_DIR)"; \
	else \
		echo "❌ Export failed. Check log: $(BUILD_LOG)"; \
		tail -40 "$(BUILD_LOG)"; \
		exit 1; \
	fi

# ─────────────────────────────────────────────────────────────────────────────
# Phase 2: Gradle Builds
# ─────────────────────────────────────────────────────────────────────────────

## Phase 2: Build release APK using Gradle
apk: check-export
	@echo ""
	@echo "══════════════════════════════════════════════"
	@echo "  Phase 2: Gradle → Release APK"
	@echo "══════════════════════════════════════════════"
	cd "$(EXPORT_DIR)" && ./gradlew \
		-PversionCode=$(VERSION_CODE) \
		-PversionName=$(VERSION_NAME) \
		:launcher:assembleRelease \
		--stacktrace \
		2>&1 | tee "$(GRADLE_LOG)"
	@if [ -f "$(APK_RELEASE)" ]; then \
		echo ""; \
		echo "✅ Release APK built:"; \
		echo "   $(APK_RELEASE)"; \
		echo "   Size: $$(du -sh $(APK_RELEASE) | cut -f1)"; \
	else \
		echo "❌ Gradle build failed. Log: $(GRADLE_LOG)"; \
		exit 1; \
	fi

## Phase 2: Build debug APK using Gradle
apk-debug: check-export
	@echo "Building debug APK via Gradle..."
	cd "$(EXPORT_DIR)" && ./gradlew \
		:launcher:assembleDebug \
		--stacktrace \
		2>&1 | tee "$(GRADLE_LOG)"
	@[ -f "$(APK_DEBUG)" ] && echo "✅ Debug APK: $(APK_DEBUG)" || (echo "❌ Failed" && exit 1)

## Phase 2: Build Android App Bundle (AAB) for app stores
aab: check-export
	@echo "Building release AAB via Gradle..."
	cd "$(EXPORT_DIR)" && ./gradlew \
		-PversionCode=$(VERSION_CODE) \
		-PversionName=$(VERSION_NAME) \
		:launcher:bundleRelease \
		2>&1 | tee "$(GRADLE_LOG)"
	@[ -f "$(AAB_RELEASE)" ] && echo "✅ AAB: $(AAB_RELEASE)" || (echo "❌ Failed" && exit 1)

## Run Gradle tasks directly (e.g. make gradle TASK=":launcher:dependencies")
gradle: check-export
	cd "$(EXPORT_DIR)" && ./gradlew $(TASK)

# ─────────────────────────────────────────────────────────────────────────────
# Full Pipeline
# ─────────────────────────────────────────────────────────────────────────────

## Full build: Unity export + Gradle APK
build: export apk
	@echo ""
	@echo "══════════════════════════════════════════════"
	@echo "  ✅ Full build complete!"
	@echo "     APK: $(APK_RELEASE)"
	@echo "══════════════════════════════════════════════"

## Full build + deploy to connected Quest
deploy: build install launch
	@echo "✅ Deployed and launched on Quest."

# ─────────────────────────────────────────────────────────────────────────────
# Local Run — No VR Headset Required
# ─────────────────────────────────────────────────────────────────────────────

DESKTOP_BUILD := $(PROJECT_PATH)/Builds/Desktop/VirtualMuseumVR.app

## Open project in Unity Editor — press Play to run with XR Device Simulator
run: check-unity
	@echo ""
	@echo "Opening Unity Editor..."
	@echo "Once open: press ▶ Play — the XR Device Simulator handles input."
	@echo ""
	@echo "Controls (no headset needed):"
	@echo "  Right Mouse + drag  → Look around"
	@echo "  W A S D             → Walk"
	@echo "  Q / E               → Move up / down"
	@echo "  Hold T              → Activate left controller"
	@echo "  Hold Y              → Activate right controller"
	@echo "  G                   → Grab object"
	@echo "  Left Click          → Trigger / Teleport"
	@echo ""
	open -a "Unity.app" --args -projectPath "$(PROJECT_PATH)"

## Build a standalone macOS desktop app (no headset, no Quest needed)
build-desktop: check-unity
	@echo ""
	@echo "══════════════════════════════════════════════"
	@echo "  Building Desktop App (macOS, no headset)"
	@echo "══════════════════════════════════════════════"
	@mkdir -p $(PROJECT_PATH)/Builds/Desktop
	$(UNITY_PATH) \
		-batchmode \
		-nographics \
		-projectPath "$(PROJECT_PATH)" \
		-buildTarget StandaloneOSX \
		-executeMethod LocalRunScript.BuildDesktop \
		-outputPath "$(DESKTOP_BUILD)" \
		-logFile "$(BUILD_LOG)" \
		-quit
	@if [ -d "$(DESKTOP_BUILD)" ]; then \
		echo "✅ Desktop app built: $(DESKTOP_BUILD)"; \
		echo "   Run it with: make open-desktop"; \
	else \
		echo "❌ Build failed. Check log: $(BUILD_LOG)"; \
		tail -30 "$(BUILD_LOG)"; \
		exit 1; \
	fi

## Launch the built desktop app
open-desktop:
	@[ -d "$(DESKTOP_BUILD)" ] || (echo "❌ App not found. Run 'make build-desktop' first." && exit 1)
	open "$(DESKTOP_BUILD)"
	@echo "✅ VirtualMuseumVR launched."

# ─────────────────────────────────────────────────────────────────────────────
# Quest Deployment via ADB
# ─────────────────────────────────────────────────────────────────────────────

## Install APK on connected Quest (skips build)
install: check-adb
	@[ -f "$(APK_RELEASE)" ] || (echo "❌ APK not found. Run 'make apk' first." && exit 1)
	@$(ADB) devices | grep -v "List of" | grep -q "device" || \
		(echo "❌ No Quest detected. Connect via USB and allow USB debugging." && exit 1)
	@echo "Installing $(APK_RELEASE)..."
	$(ADB) install -r "$(APK_RELEASE)"
	@echo "✅ Installed on Quest."

## Uninstall the app from Quest
uninstall: check-adb
	$(ADB) uninstall $(PACKAGE_NAME) || true
	@echo "✅ Uninstalled $(PACKAGE_NAME)"

## Launch the app on connected Quest
launch: check-adb
	$(ADB) shell am start -n $(PACKAGE_NAME)/$(ACTIVITY)
	@echo "✅ Launched on Quest."

## Stream Unity logs from connected Quest
logcat: check-adb
	@echo "Streaming Unity logs (Ctrl+C to stop)..."
	$(ADB) logcat -s Unity:V ActivityManager:I

## Show connected Quest device info
device-info: check-adb
	@echo "Connected devices:"
	@$(ADB) devices
	@echo ""
	@echo "Device model:"
	@$(ADB) shell getprop ro.product.model 2>/dev/null || echo "None connected"

# ─────────────────────────────────────────────────────────────────────────────
# Signing Setup
# ─────────────────────────────────────────────────────────────────────────────

## Generate a debug keystore for local builds
gen-keystore:
	keytool -genkeypair \
		-v \
		-keystore "$(PROJECT_PATH)/release.keystore" \
		-alias virtualmuseum \
		-keyalg RSA \
		-keysize 2048 \
		-validity 10000 \
		-dname "CN=Virtual Museum VR, OU=Dev, O=VirtualMuseum, L=Unknown, S=Unknown, C=IN"
	@echo "✅ Keystore created: $(PROJECT_PATH)/release.keystore"
	@echo "   Set KEYSTORE_PASS, KEY_ALIAS, KEY_PASS env vars when building."

# ─────────────────────────────────────────────────────────────────────────────
# Cleanup
# ─────────────────────────────────────────────────────────────────────────────

## Clean only Gradle build outputs (keep Unity export)
clean:
	@[ -f "$(GRADLEW)" ] && cd "$(EXPORT_DIR)" && ./gradlew clean || true
	@echo "✅ Gradle outputs cleaned."

## Remove the entire Unity Gradle export
clean-export:
	rm -rf "$(EXPORT_DIR)"
	@echo "✅ Gradle export removed."

## Remove everything including Unity Library cache
clean-all: clean-export
	rm -rf "$(PROJECT_PATH)/Library"
	rm -rf "$(PROJECT_PATH)/Temp"
	rm -f "$(BUILD_LOG)" "$(GRADLE_LOG)"
	@echo "✅ Full clean done. Next Unity open will reimport (~10 min)."

# ─────────────────────────────────────────────────────────────────────────────
# Guards
# ─────────────────────────────────────────────────────────────────────────────

check-unity:
	@[ -f "$(UNITY_PATH)" ] || ( \
		echo "❌ Unity $(UNITY_VERSION) not found at:"; \
		echo "   $(UNITY_PATH)"; \
		echo "Install: unityhub -- --headless install --version $(UNITY_VERSION)"; \
		exit 1)

check-export:
	@[ -f "$(GRADLEW)" ] || ( \
		echo "❌ Gradle project not found. Run 'make export' first."; \
		exit 1)

check-adb:
	@command -v $(ADB) >/dev/null 2>&1 || ( \
		echo "❌ ADB not found. Install: brew install android-platform-tools"; \
		exit 1)

# ─────────────────────────────────────────────────────────────────────────────
# Help
# ─────────────────────────────────────────────────────────────────────────────

## Show all available commands
help:
	@echo ""
	@echo "Virtual Museum VR — Gradle Build System"
	@echo "Unity $(UNITY_VERSION) → Gradle → Meta Quest APK"
	@echo ""
	@echo "Two-phase workflow:"
	@echo "  Phase 1  make export    Unity compiles C# → exports Gradle project"
	@echo "  Phase 2  make apk       Gradle packages → APK"
	@echo "  Both     make build     Full pipeline (export + apk)"
	@echo ""
	@echo "All commands:"
	@grep -E '^## ' Makefile | sed 's/^## /  make /'
	@echo ""
	@echo "Environment variables for signing:"
	@echo "  KEYSTORE_PATH, KEYSTORE_PASS, KEY_ALIAS, KEY_PASS"
	@echo ""
	@echo "Example full flow:"
	@echo "  make build              # compile + package"
	@echo "  make deploy             # build + push + launch on Quest"
	@echo "  make apk VERSION_NAME=1.2.0 VERSION_CODE=3"
