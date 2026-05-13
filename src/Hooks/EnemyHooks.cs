using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Nep2ArchipelagoClient.Neptunia2Data;

namespace Nep2ArchipelagoClient.src.Hooks;

internal class EnemyHooks
{
    // -----------------------------------------------------------------------
    // AP location base IDs — keep in sync with the Archipelago world definition
    // -----------------------------------------------------------------------
    public const long EnemyBaseID = 2_000_000;

    // -----------------------------------------------------------------------
    // Hook state
    // -----------------------------------------------------------------------
    public static IReverseWrapper<OnNewEnemyKilled> _onNewEnemyKilled = null!;
    public static List<IAsmHook> _asmHooks = new();

    // -----------------------------------------------------------------------
    // Delegate — same register convention as RB3 (eax + esi in, eax out)
    // -----------------------------------------------------------------------
    [Function(
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.esi },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee)]
    public delegate int OnNewEnemyKilled(int eax, int esi);

    /// <summary>
    /// Called from the ASM hook whenever the game registers an enemy kill.
    /// ESI & 0xFFFF gives the internal enemy ID (matches stcharamonster.gbin IDs).
    /// The AP server/client handles deduplication of already-checked locations.
    /// </summary>
    public static int OnEnemyKilled(int eax, int esi)
    {
        var enemyId = esi & 0xFFFF;
        long apLocationId = EnemyBaseID + enemyId;
        Console.WriteLine($"[Nep2AP] Killed enemy ID {enemyId} -> AP location {apLocationId}");

        // TODO: wire up to APClient once it is implemented, e.g.:
        // Mod.APClient.SendLocation(apLocationId);

        return eax;
    }

    // -----------------------------------------------------------------------
    // Hook setup
    // -----------------------------------------------------------------------

    /// <summary>
    /// Installs the enemy-kill ASM hook.
    /// The hook fires at <see cref="Offsets.EnemyKilled"/> (RB2 RVA 0xBA9F0),
    /// the same logical point as RB3's 0xC0280.
    /// </summary>
    public static void SetupHooks(IReloadedHooks hooks)
    {
        string[] enemyKilledAsm =
        {
            "use32",
            "pushad",
            "pushfd",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnEnemyKilled, out _onNewEnemyKilled)}",
            "popfd",
            "popad",
        };

        _asmHooks.Add(
            hooks.CreateAsmHook(
                    enemyKilledAsm,
                    (int)(Mod.ModuleBase + Offsets.EnemyKilled),
                    AsmHookBehaviour.ExecuteFirst)
                .Activate());
    }
}
