using Reloaded.Mod.Interfaces;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using Nep2ArchipelagoClient.Configuration;

namespace Nep2ArchipelagoClient.Template;

public class ModContext
{
    public IModLoader ModLoader { get; set; } = null!;
    public IReloadedHooks? Hooks { get; set; } = null!;
    public ILogger Logger { get; set; } = null!;
    public Config Configuration { get; set; } = null!;
    public IModConfig ModConfig { get; set; } = null!;
    public IMod Owner { get; set; } = null!;
}
