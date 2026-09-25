# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Color-Room-VR is a Unity VR project (a challenge for the Unity VR Development pathway) where the player paints objects in a room. Unity **6000.3.17f1**, URP 17.3, XR Interaction Toolkit 3.3.1, OpenXR. Single scene: `Assets/Scenes/Color Room.unity`.

The Unity CLI (`unity`, run `unity --help`) is installed, and the `unity@claude-plugins-official` Claude Code plugin is enabled (see `.claude/settings.local.json`), which provides Unity skills such as `unity:unity-cli`. Use the CLI for editor/project operations, e.g. `unity build` (batch-mode build), `unity editors`, `unity doctor`, and driving a running Editor. There is no lint setup. You can still open the project in the Unity Editor (or Unity Hub) to build and run; scripts compile automatically on focus. The `com.unity.test-framework` package is installed, but no tests exist yet.

## Architecture

All game code lives in `Assets/Scripts` under the `ColorRoomVR` namespace, split by role:

- **Coloring/** – `PaintableObject` (single mesh, requires MeshRenderer + MeshCollider) and `PaintableGroup` (a set of objects painted together). An object with a `paintableGroup` reference registers itself with the group in `Awake` and skips its own initial-state load; the group loads/saves the color under `groupID` instead of each member's `objectID`. Colors are applied via `MaterialPropertyBlock` on `_BaseColor` (URP Lit), not by modifying materials.
- **Data/** – `ColorsDataManager` is a singleton holding an `id -> Color` dictionary, saved (debounced by 1s) through `IColorPersistenceService`. `JsonFilePersistenceService` writes JSON to `Application.persistentDataPath/ColorsRoom_{roomID}`. Change `roomID` to use a separate save slot.
- **Interaction/** – `ObjectDetection` raycasts to find the hovered `PaintableObject`/group and outlines it (via the QuickOutline asset); `PaintVFXManager` handles paint effects. Currently the raycast uses the mouse position (`Input.mousePosition`) as a desktop testing stand-in; the XR controller ray is commented out.
- **Reactions/** – `AnimationOnPaint`, `EnableOnPaint`: components wired to `PaintableObject.OnPainted` (UnityEvent) to trigger animations/unlocks.

Key conventions/flow:
- `SetColor(color, isPlayerAction)`: `isPlayerAction = true` persists the color and fires `OnPainted`; `false` is used for loading saved state. For groups, `OnPainted` fires only on the first member. Saved objects re-fire `OnPainted` on load so reactions re-activate.
- Ids (`objectID`/`groupID`) default to the GameObject name in `Reset()`; they are the persistence keys, so renaming them orphans saved colors.
- `ColorsDataManager.Instance` must exist in the scene before any paintable's `Start`.

## Third-party assets/packages

`Assets/AnotherColorPicker` (color palette UI; `ColorPaletteController` provides `SelectedColor`), `Assets/QuickOutline` (`Outline` component), `EditorAttributes` (git package, provides `[ShowField]`, `[Button]`, `[Clamp]` attributes used in scripts). Avoid editing these vendored/package sources.
