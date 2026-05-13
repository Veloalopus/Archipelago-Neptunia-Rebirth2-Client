using Reloaded.Mod.Interfaces;
using Nep2ArchipelagoClient.Configuration;

namespace Nep2ArchipelagoClient.Template.Configuration;

public class ConfiguratorMixinBase
{
    public virtual IUpdatableConfigurable[] MakeConfigurations(string configFolder)
    {
        return new IUpdatableConfigurable[]
        {
            Configurable<Config>.FromFile(Path.Combine(configFolder, "Config.json"), "Default Config")
        };
    }

    public virtual bool TryRunCustomConfiguration(Configurator configurator) => false;

    public virtual void Migrate(string oldDirectory, string newDirectory)
    {
#pragma warning disable CS8321
        void TryMoveFile(string fileName)
        {
            try { File.Move(Path.Combine(oldDirectory, fileName), Path.Combine(newDirectory, fileName)); }
            catch (Exception) { /* Ignored */ }
        }
#pragma warning restore CS8321
    }
}
