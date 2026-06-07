using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace OSKillBonus;

public class Json
{
	public int Health { get; init; }
	public int HealthHS { get; init; }
	public int HealthMax { get; init; }
}

public class OSKillBonus : BasePlugin
{
	public override string ModuleName => "OS Kill Bonus";
	public override string ModuleVersion => "1.0";
	public override string ModuleAuthor => "KiKiEEKi ( DS: kikieeki | vk.com/kikieeki )";

	private Json Config = null!;

	public override void Load(bool hotReload)
	{
		var configPath = Path.Combine(ModuleDirectory, "OSKillBonus.json");
		if(!File.Exists(configPath)) {
			Config = new Json
			{
				Health = 10,
				HealthHS = 15,
				HealthMax = 125
			};
			File.WriteAllText(configPath,
				JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
		}

		Config = JsonSerializer.Deserialize<Json>(File.ReadAllText(configPath))!;
	}

	[GameEventHandler]
	public HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
	{
		var Attacker = @event.Attacker;
		if(Attacker is null || !Attacker.IsValid) return HookResult.Continue;

		var AttackerPawn = Attacker.PlayerPawn.Value;
		if (AttackerPawn == null) return HookResult.Continue;

		if(@event.Headshot) AttackerPawn.Health += Config.HealthHS;
		else AttackerPawn.Health += Config.Health;

		if(AttackerPawn.Health > Config.HealthMax) AttackerPawn.Health = Config.HealthMax;

		Utilities.SetStateChanged(AttackerPawn, "CBaseEntity", "m_iHealth");

		return HookResult.Continue;
	}
}
