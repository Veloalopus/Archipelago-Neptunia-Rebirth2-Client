namespace Nep2ArchipelagoClient.Neptunia2Data;

/// <summary>
/// RVA offsets for NeptuniaReBirth2.exe (ModuleBase = 0x400000).
/// All offsets confirmed via pattern-matching against NeptuniaReBirth3.exe.
/// </summary>
internal static class Offsets
{
    // -----------------------------------------------------------------------
    // Hook injection points
    // -----------------------------------------------------------------------

    /// <summary>
    /// Fired when a new (first-time) enemy kill is registered.
    /// ESI & 0xFFFF = internal enemy ID.
    /// Confirmed: unique exact pattern match.
    /// </summary>
    public const int EnemyKilled = 0xBA9F0;

    /// <summary>
    /// Gather spot load — fires when a gather node is interacted with.
    /// EAX = dungeon ID, EDX = gather flag index.
    /// NOTE: 4 candidate functions (0xBCECE / 0xBCF4E / 0xBD1EE / 0xBD27E).
    /// Needs in-game verification to identify the correct one.
    /// </summary>
    public const int GatherLoad = 0xBCECE; // TODO: verify in-game

    /// <summary>
    /// Gather spot collect — fires when a gather item is actually picked up.
    /// Hook replaces the 'mov ecx,[ebp-4]' instruction inside the function.
    /// Confirmed via unique epilogue pattern match.
    /// </summary>
    public const int GatherCollectFn   = 0x17E240;
    public const int GatherCollectHook = 0x17E287;

    /// <summary>
    /// Treasure chest opened — ECX = internal treasure ID.
    /// Confirmed: unique 28-byte pattern match.
    /// </summary>
    public const int TreasureChest = 0xB416D;

    // -----------------------------------------------------------------------
    // Game functions (callable wrappers)
    // -----------------------------------------------------------------------

    /// <summary>
    /// stdcall: int AddItemToInventory(uint itemID, uint quantity, char dunno)
    /// Confirmed: unique pattern match.
    /// </summary>
    public const int AddItemFn = 0xB8390;

    /// <summary>
    /// stdcall: int TogglePlan(int planID, int enable)
    /// Hook injection point is +3 from function start (same delta as RB3).
    /// Confirmed: unique masked pattern match.
    /// </summary>
    public const int TogglePlanFn   = 0xB8DA0;
    public const int TogglePlanHook = 0xB8DA3;

    /// <summary>
    /// stdcall: int AddNewCharacter(uint characterID)
    /// Confirmed: post-call signature match.
    /// </summary>
    public const int AddCharacter = 0xB6180;

    /// <summary>
    /// stdcall: nuint FindCharacterPointer(int characterID)
    /// Two nearly identical candidates: 0xB6470 (primary) and 0xB64E0.
    /// Try 0xB6470 first; swap to 0xB64E0 if character lookups fail.
    /// </summary>
    public const int FindCharacter = 0xB6470;

    /// <summary>
    /// stdcall: nuint RemovePartyMember(int characterID)
    /// Confirmed: unique masked match.
    /// </summary>
    public const int RemovePartyMember = 0xB6300;

    /// <summary>
    /// Shop sell hook injection point.
    /// Confirmed: epilogue + body context match.
    /// </summary>
    public const int SellHook = 0x157740;

    /// <summary>
    /// stdcall: int SellItem(nuint itemStackPointer)
    /// Resolved via call-target of SellHook.
    /// </summary>
    public const int SellFn = 0x1594D0;

    // -----------------------------------------------------------------------
    // Save data
    // -----------------------------------------------------------------------

    /// <summary>
    /// Pointer to save game base, read as: Memory.Read&lt;uint&gt;(ModuleBase + SavePointer)
    /// Confirmed from RB2 speedrun ASL file.
    /// </summary>
    public const int SavePointer = 0x43310;

    // -----------------------------------------------------------------------
    // Inventory layout (relative to save game base)
    // -----------------------------------------------------------------------
    public const int InventorySize   = 0xCA4C;
    public const int InventoryStart  = 0xCA50; // InventorySize + 0x04
    public const int ItemAmountOffset = 0x02;
    public const int ItemLength      = 0x04;
}
