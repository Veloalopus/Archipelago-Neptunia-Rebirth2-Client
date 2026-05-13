using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Nep2ArchipelagoClient.Template;
using Nep2ArchipelagoClient.Configuration;
using Nep2ArchipelagoClient.src.Hooks;
using System.Diagnostics;

namespace Nep2ArchipelagoClient;

public class Mod : ModBase
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _logger;
    private readonly IMod _owner;
    private Config _configuration;
    private readonly IModConfig _modConfig;

    /// <summary>
    /// Base address of NeptuniaReBirth2.exe in the current process.
    /// Reloaded-II always injects into the target process, so this resolves correctly at runtime.
    /// </summary>
    public static UIntPtr ModuleBase = 0x400000;

    public Mod(ModContext context)
    {
        _modLoader     = context.ModLoader;
        _hooks         = context.Hooks;
        _logger        = context.Logger;
        _owner         = context.Owner;
        _configuration = context.Configuration;
        _modConfig     = context.ModConfig;

#if DEBUG
        Debugger.Launch();
#endif

        // Resolve the actual base address at runtime.
        ModuleBase = (UIntPtr)Process.GetCurrentProcess().MainModule!.BaseAddress;
        _logger.WriteLine($"[Nep2AP] Module base: 0x{ModuleBase:X}");

        // Install all hooks.
        Hooks.SetupAllHooks(_hooks!);

        _logger.WriteLine("[Nep2AP] Hooks installed. Waiting for game...");

        // TODO: start AP client connection thread here once APClient is implemented.
        // APClient.ConnectToServer(_configuration.Server, _configuration.Port, _configuration.Player);
    }

    public override void ConfigurationUpdated(Config configuration)
    {
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config updated.");
    }

#pragma warning disable CS8618
    public Mod() { }
#pragma warning restore CS8618
}
