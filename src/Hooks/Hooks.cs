using Reloaded.Hooks.Definitions;

namespace Nep2ArchipelagoClient.src.Hooks;

internal class Hooks
{
    public static void SetupAllHooks(IReloadedHooks hooks)
    {
        EnemyHooks.SetupHooks(hooks);
        ItemCollectionHooks.SetupHooks(hooks);

        // TODO: port remaining hooks from RB3 once AP client is wired up
        // PlanHooks.SetupHooks(hooks);
        // CharacterHooks.SetupHooks(hooks);
        // ShopHook.SetupHooks(hooks);
    }
}
