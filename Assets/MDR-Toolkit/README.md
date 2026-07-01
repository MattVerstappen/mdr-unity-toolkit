## WeightedRandomSelector<T>
A generic weighted random selection system. Works with any type - strings, enums,
prefabs, ScriptableObjects, or custom classes.

### Quick Start

```csharp
var selector = new WeightedRandomSelector<string>();
selector.Add("Common",    60f);
selector.Add("Rare",      30f);
selector.Add("Legendary", 10f);

string result = selector.Pick(); // "Common" ~60% of the time
```

### Inspector Support
Use WeightedRandomSelectorDemo.cs on any GameObject to configure
weighted string pools directly in the Unity Inspector.

## AudioRandomizer
Eliminates the "machine gun" audio effect by blocking recently played clips.
Supports weighted clip pools, pitch randomisation, and volume randomisation.

### Quick Start
1. Add AudioRandomizer to any GameObject with an AudioSource
2. Add your AudioClips to the Entries list in the Inspector
3. Set Anti Repeat Count to 2 (blocks the last 2 played clips)
4. Call PlayRandom() from any script or trigger

```csharp
// From another script
GetComponent<AudioRandomizer>().PlayRandom();
```

### Anti-Repeat Logic
If antiRepeatCount is 2 and your pool has 3 clips [A, B, C]:
- Play A -> blocked: [A]
- Play B -> blocked: [A, B]
- Play C -> blocked: [A, B] -> C chosen, queue becomes [B, C]
- A is now available again

## TraumaCameraShake
GDC-recommended trauma-based camera shake. Uses Perlin noise and a trauma^2 curve
for natural, smooth shake falloff instead of random jitter.

### Quick Start
1. Attach TraumaCameraShake to your Camera GameObject
2. Call AddTrauma() from any script when something impactful happens

```csharp
// On explosion:
FindObjectOfType<TraumaCameraShake>().AddTrauma(0.8f);

// Multiple hits stack up:
cameraShake.AddTrauma(0.3f); // each hit adds to the trauma pool
```

### Presets
Apply a ShakePreset ScriptableObject from the Inspector, or use:

```csharp
cameraShake.ApplyPreset(ShakePreset.Explosion);
```

### Why trauma^2?
Squaring the trauma value means shake intensity drops off quickly from 1.0
but lingers at low values, which feels more physically correct than linear decay.

## OffScreenIndicator
Screen-edge arrows that point toward off-screen targets.
Supports multiple targets, custom colors, custom icons, and automatic pooling.

### Quick Start
1. Create a Canvas (Screen Space - Overlay)
2. Attach OffScreenIndicator to the Canvas
3. Assign an arrow prefab (Image with a RectTransform, arrow pointing UP)
4. Attach OffScreenIndicatorTarget to any world GameObject you want tracked
5. Press Play - indicators appear automatically when targets leave the screen

### Arrow Prefab Setup
- Create a UI Image inside the Canvas
- Point the arrow art UPWARD (12 o'clock) at 0 degrees rotation
- The system rotates the RectTransform to point toward the target

### Runtime Registration

```csharp
// Manual registration if autoRegister is false:
OffScreenIndicator indicator = FindObjectOfType<OffScreenIndicator>();
indicator.RegisterTarget(GetComponent<OffScreenIndicatorTarget>());
```

## SaveSystem
A versioned save system with schema migration support.
When your save data structure changes between game versions, existing saves upgrade automatically.

### Quick Start
1. Implement ISaveable on any MonoBehaviour:

```csharp
public class PlayerHealth : MonoBehaviour, ISaveable
{
    public float health = 100f;

    public string GetSaveKey() => "playerHealth";

    public void OnSave(SaveData data) => data.Set("health", health);

    public void OnLoad(SaveData data) => health = data.Get("health", 100f);
}
```

2. Save and load from anywhere:

```csharp
SaveSystem.Save("slot1");
SaveSystem.Load("slot1");
```

### Schema Migration
When you add new fields, increment CURRENT_SCHEMA_VERSION in SaveSystem.cs
and add a migration step in SaveMigrator.cs:

```csharp
private static void MigrateV1ToV2(SaveData data)
{
    if (!data.Has("newField"))
        data.Set("newField", "defaultValue");
    data.schemaVersion = 2;
}
```

Existing saves will be automatically upgraded on next load.

### Multiple Save Slots

```csharp
SaveSystem.Save("autosave");
SaveSystem.Save("slot1");
List<SaveSlot> slots = SaveSystem.GetAllSlots();
```
