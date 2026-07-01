# MDR Unity Toolkit

Modular Unity scripts by Matthew Derek Rall - free, documented, and built to solve real problems other developers actually face.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
![Unity](https://img.shields.io/badge/Unity-6000.0%2B-black.svg)

## What is this?

MDR Unity Toolkit is a small collection of independent, drop-in systems for
common gameplay problems - weighted random selection, non-repetitive audio,
camera feedback, off-screen UI indicators, and versioned save data. Each
script is self-contained, fully documented with XML comments, ships with a
custom Inspector, and comes with a runnable sample scene script. There are
no third-party dependencies and no paid add-ons - just source you can read,
modify, and own.

## Requirements

- Unity 6000.0 (Unity 6) or later
- C# (no external packages required)

## Installation

Copy the `Assets/MDR-Toolkit` folder into your project's `Assets` directory.
Each system lives in its own subfolder under `Runtime/`, with matching
`Editor/` and `Samples/` counterparts, so you can also delete any system you
don't need without breaking the others.

```
Assets/MDR-Toolkit/
  Runtime/    - the actual components and data types
  Editor/     - custom Inspectors and editor windows
  Samples/    - drop-in demo scripts for each system
```

Assembly definitions (`MDR-Toolkit.Runtime.asmdef` / `MDR-Toolkit.Editor.asmdef`)
are included so the Editor scripts are automatically excluded from player builds.

---

## WeightedRandomSelector

A generic weighted random selection system. Works with any type - strings,
enums, prefabs, ScriptableObjects, or custom classes - and picks entries with
probability proportional to a per-entry weight, without requiring weights to
sum to any particular total.

```csharp
var selector = new WeightedRandomSelector<string>();
selector.Add("Common",    60f);
selector.Add("Rare",      30f);
selector.Add("Legendary", 10f);

string result = selector.Pick(); // "Common" ~60% of the time
```

Use `WeightedRandomSelectorDemo.cs` on any GameObject to configure weighted
string pools directly in the Unity Inspector, complete with a live weight-bar
visualization in the custom editor.

**Script:** [WeightedRandomSelector.cs](Assets/MDR-Toolkit/Runtime/Utility/WeightedRandomSelector.cs)

## AudioRandomizer

Eliminates the "machine gun" audio effect by blocking recently played clips.
Supports weighted clip pools, pitch randomisation, and volume randomisation,
all configurable from the Inspector.

```csharp
// Attach to any GameObject with an AudioSource, configure the Entries list,
// then trigger playback from anywhere:
GetComponent<AudioRandomizer>().PlayRandom();
```

With `Anti Repeat Count` set to 2 and a 3-clip pool `[A, B, C]`: playing A
blocks A, playing B blocks A and B, and playing C forces the choice away
from A/B - after which A becomes available again. The custom editor shows
the current anti-repeat queue live during preview.

**Script:** [AudioRandomizer.cs](Assets/MDR-Toolkit/Runtime/Audio/AudioRandomizer.cs)

## TraumaCameraShake

GDC-recommended trauma-based camera shake. Uses Perlin noise sampled per
axis (not random jitter) and a `trauma^2` falloff curve for shake that decays
naturally instead of linearly.

```csharp
// On explosion:
FindObjectOfType<TraumaCameraShake>().AddTrauma(0.8f);

// Multiple hits stack up, each adding to the trauma pool:
cameraShake.AddTrauma(0.3f);
```

Ships with four `ShakePreset` presets (`Light`, `Medium`, `Heavy`,
`Explosion`) usable without creating any asset file:

```csharp
cameraShake.ApplyPreset(ShakePreset.Explosion);
```

**Script:** [TraumaCameraShake.cs](Assets/MDR-Toolkit/Runtime/Camera/TraumaCameraShake.cs)

## OffScreenIndicator

Screen-edge arrows that point toward off-screen targets - objectives,
enemies, or other players. Supports multiple targets, custom colors, custom
icons, and automatic indicator pooling, using plain uGUI (no TextMeshPro
required).

```csharp
// Attach OffScreenIndicator to a Screen Space - Overlay Canvas, assign an
// arrow prefab, then attach OffScreenIndicatorTarget to any world object:

// Manual registration if autoRegister is false:
OffScreenIndicator indicator = FindObjectOfType<OffScreenIndicator>();
indicator.RegisterTarget(GetComponent<OffScreenIndicatorTarget>());
```

Indicators reuse a simple GameObject pool, and the custom Inspector lists
every registered target with its live on/off-screen status during Play Mode.

**Script:** [OffScreenIndicator.cs](Assets/MDR-Toolkit/Runtime/UI/OffScreenIndicator.cs)

## SaveSystem

A versioned save system with schema migration support. When your save data
structure changes between game versions, existing saves upgrade
automatically instead of breaking or silently dropping data.

```csharp
public class PlayerHealth : MonoBehaviour, ISaveable
{
    public float health = 100f;

    public string GetSaveKey() => "playerHealth";
    public void OnSave(SaveData data) => data.Set("health", health);
    public void OnLoad(SaveData data) => health = data.Get("health", 100f);
}

// From anywhere:
SaveSystem.Save("slot1");
SaveSystem.Load("slot1");
```

When you add new fields, increment `CURRENT_SCHEMA_VERSION` in
`SaveSystem.cs` and add a migration step in `SaveMigrator.cs` - existing
saves are upgraded automatically the next time they're loaded.

**Script:** [SaveSystem.cs](Assets/MDR-Toolkit/Runtime/Save/SaveSystem.cs)

---

## License

MIT - see [LICENSE](LICENSE) for details. Use it, modify it, ship it in your
own commercial projects, no attribution required.
