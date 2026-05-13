using System.ComponentModel;
using Nep2ArchipelagoClient.Template.Configuration;

namespace Nep2ArchipelagoClient.Configuration;

public class Config : Configurable<Config>
{
    [DisplayName("AP Server")]
    [Description("Archipelago server address")]
    [DefaultValue("localhost")]
    public string Server { get; set; } = "localhost";

    [DisplayName("Port")]
    [Description("Archipelago server port")]
    [DefaultValue(38281)]
    public int Port { get; set; } = 38281;

    [DisplayName("Player")]
    [Description("Archipelago slot name")]
    [DefaultValue("Player1")]
    public string Player { get; set; } = "Player1";
}

public class ConfiguratorMixin : ConfiguratorMixinBase { }
