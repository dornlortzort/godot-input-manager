using Godot;
using System;

public static class InputProfileLoader {
  public const string SavePath = "user://last_input_profile.res";

  public static InputProfile LoadLastUsedProfile(InputManager manager) {
    if (ResourceLoader.Exists(SavePath)) {
      var loaded = ResourceLoader.Load<InputProfile>(SavePath,
        cacheMode: ResourceLoader.CacheMode.Ignore);
      if (loaded != null) return loaded;
      GD.PushWarning($"Corrupted profile at {SavePath}, falling back to default.");
    }

    if (manager.Registry != null) return manager.Registry.DefaultProfile;

    throw new InvalidOperationException("No valid input Registry or profile available.");
  }

  public static void SaveProfile(InputProfile profile) {
    var err = ResourceSaver.Save(profile, SavePath);
    if (err != Error.Ok)
      throw new InvalidOperationException("No valid input Registry or profile available.");
  }
}