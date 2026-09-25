using Content.Server.StationEvents.Events;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(LiquidSlimeRule))]
public sealed partial class LiquidSlimeRuleComponent : Component
{
    [DataField]
    public float LiquidThreshold = 5000f;

    [DataField]
    public int SlimesToSpawn = 3;

    [DataField]
    public EntProtoId SlimePrototype = "MobAdultSlimesGreen";
}