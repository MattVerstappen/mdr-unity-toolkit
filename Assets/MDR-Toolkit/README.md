# MDR Unity Toolkit

Modular, dependency-free Unity scripts by Matthew Derek Rall.

## WeightedRandomSelector<T>
A generic weighted random selection system. Works with any type - strings, enums,
prefabs, ScriptableObjects, or custom classes.

### Quick Start

```csharp
var selector = new WeightedRandomSelector<string>();
selector.Add("Common",  60f);
selector.Add("Rare",    30f);
selector.Add("Legendary", 10f);

string result = selector.Pick(); // "Common" ~60% of the time
```

### Inspector Support
Use WeightedRandomSelectorDemo.cs on any GameObject to configure
weighted string pools directly in the Unity Inspector.
