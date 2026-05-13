using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;
using Nep2ArchipelagoClient.Neptunia2Data;

namespace Nep2ArchipelagoClient.src.Hooks;

public class ItemCollectionHooks
{
    // -----------------------------------------------------------------------
    // AP location base IDs — keep in sync with Archipelago world definition
    // -----------------------------------------------------------------------
    public const long TreasureBaseID = 1_000_000;
    public const long GatherBaseID   = 1;  // location ID = (dungeonID * 10) + flagIndex + 1

    // -----------------------------------------------------------------------
    // Hook state
    // -----------------------------------------------------------------------
    public static List<IAsmHook> _asmHooks = new();

    public static IReverseWrapper<GetGatherSpot>      _onGatherSpot        = null!;
    public static IReverseWrapper<GetDungeonTreasureId> _onGetDungeonTreasureId = null!;

    public static IFunction<AddItemToInventory> _addItemFunction = null!;

    // -----------------------------------------------------------------------
    // Delegates
    // -----------------------------------------------------------------------

    [Function(CallingConventions.Stdcall)]
    public delegate int AddItemToInventory(uint itemID, uint quantity, char dunno);

    /// <summary>
    /// Fires when a gather node is interacted with.
    /// EAX = dungeon ID, EDX = gather flag index.
    /// Confirmed at RB2 offset 0xBCECE via Cheat Engine.
    /// </summary>
    [Function(
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee)]
    public delegate int GetGatherSpot(int dungeonID, int gatherFlag);

    /// <summary>
    /// Fires when a treasure chest is opened.
    /// ECX = internal treasure ID.
    /// Confirmed at RB2 offset 0xB416D via Cheat Engine.
    /// </summary>
    [Function(
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.ecx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee)]
    public delegate int GetDungeonTreasureId(int eax, int treasureID);

    // -----------------------------------------------------------------------
    // Callbacks
    // -----------------------------------------------------------------------

    public static int OnGetGatherSpot(int eax, int edx)
    {
        long locationId = (eax * 10) + edx + 1;
        Console.WriteLine($"[Nep2AP] Gather: dungeon={eax} flag={edx} -> AP location {locationId}");

        // TODO: wire up to APClient, e.g.:
        // Mod.APClient.SendLocation(locationId);

        return eax;
    }

    public static int OnGetDungeonTreasureId(int eax, int ecx)
    {
        long locationId = TreasureBaseID + ecx;
        Console.WriteLine($"[Nep2AP] Treasure: ID={ecx} -> AP location {locationId}");

        // TODO: wire up to APClient, e.g.:
        // Mod.APClient.SendLocation(locationId);

        return eax;
    }

    // -----------------------------------------------------------------------
    // Hook setup
    // -----------------------------------------------------------------------

    public static void SetupHooks(IReloadedHooks hooks)
    {
        if (hooks == null) return;

        // Wrap the game's add-item function so we can give items to the player later
        _addItemFunction = hooks.CreateFunction<AddItemToInventory>(
            (int)(Mod.ModuleBase + Offsets.AddItemFn));
        Console.WriteLine($"[Nep2AP] AddItemFn at 0x{_addItemFunction.Address:X}");

        // --- Gather spot hook ---
        // At 0xBCECE: EAX = gather flag index (about to be pushed as arg to BC5D0)
        //             [EBP+0C] = dungeon ID
        string[] gatherSpotAsm =
        {
            "use32",
            "pushad",
            "pushfd",
            "mov edx,[ebp+0x0C]",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetGatherSpot, out _onGatherSpot)}",
            "popfd",
            "popad",
        };
        _asmHooks.Add(
            hooks.CreateAsmHook(
                    gatherSpotAsm,
                    (int)(Mod.ModuleBase + Offsets.GatherLoad),
                    AsmHookBehaviour.ExecuteFirst)
                .Activate());

        // --- Treasure chest hook ---
        // ECX = treasure ID at 0xB416D, EAX is also available.
        string[] treasureAsm =
        {
            "use32",
            "pushad",
            "pushfd",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnGetDungeonTreasureId, out _onGetDungeonTreasureId)}",
            "popfd",
            "popad",
        };
        _asmHooks.Add(
            hooks.CreateAsmHook(
                    treasureAsm,
                    (int)(Mod.ModuleBase + Offsets.TreasureChest),
                    AsmHookBehaviour.ExecuteFirst)
                .Activate());
    }
}
