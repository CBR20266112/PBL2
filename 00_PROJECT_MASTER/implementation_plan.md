# Implementation Plan - UI Placeholder Removal and Art Integration

This revised plan aligns with our updated development principles, focusing on removing solid-color placeholders and replacing them with high-quality, pre-made art assets. We prioritize standard Unity workflows (Inspector, Prefabs, ScriptableObjects) and will proceed in small, verifiable steps.

---

## Development Principles & Goals

1. **Aesthetics & Polish First**: Prioritize visual fidelity over bug fixes to establish a solid visual baseline for the prototype.
2. **Standard Unity Workflow**: Expose properties as `SerializeField` in the Inspector rather than using runtime `AddComponent`, reflection, or hardcoded path loading.
3. **No Breaking Changes**: Keep existing game logic, managers, and button events intact.
4. **Step-by-Step Iteration**: Apply a change, verify in Unity, fix issues, commit, and proceed.

---

## Proposed Changes

### Phase 1: UI Placeholder Removal & Art Integration

#### Step 1.1: Title Screen Visuals
- Expose `backgroundSprite`, `buttonSprite`, and `panelSprite` in `TitleScreenUIBuilder.cs`.
- Update `CreateBackground()`, `CreateMainButton()`, and `CreateNameInputPanel()` to render sprites and reset color tints.
- Assign GUID references directly inside `Title.unity` for a zero-configuration start.

#### Step 1.2: Main Screen HUD & Bottom Navigation
- Remove dynamic `AddComponent` for UI builders in `MainScreenUIBuilder.cs`.
- Attach `MainScreenUIBuilder`, `TutorialUIBuilder`, and `SettingsUIBuilder` directly to the Canvas in `Main.unity`.
- Expose serialized sprite fields for HUD, navigation panel background, and buttons in `MainScreenUIBuilder.cs`.
- Map sprites using their asset GUIDs in `Main.unity`.

#### Step 1.3: Tutorial Dialogue & Speech Bubbles
- Add serialized sprite fields to `TutorialUIBuilder.cs` for the main dialogue frame and buttons.
- Update `TutorialUIBuilder.cs` to apply these sprites.

#### Step 1.4: Customer Speech Bubbles & Aspect Ratio
- Expose `speechBubbleSprite` in `CustomerAppearanceUIBuilder.cs` and map it to `UI_SpeechBubble_01` (GUID: `2cdf66a259a46394dba37e356bba1a94`).
- Update rendering of customer sprites to center-fit and preserve aspect ratio, replacing old solid-color panel fallbacks.

---

## Verification Plan

### Manual Verification
1. **Title Screen Test**:
   - Verify the background matches `Background_Concept_01.png`.
   - Verify buttons use the new styled textures instead of solid orange rectangles.
2. **Main Scene Test**:
   - Verify the game screen loads `Background_Gameplay_Mockup_01.png` instead of a flat peach color.
   - Verify Top HUD and Navigation panels use styled panel elements.
   - Verify that the tutorial bubble is shaped like a speech bubble.
3. **Customer Render Test**:
   - Verify the customer sprite is loaded and displayed correctly with proper proportions.
