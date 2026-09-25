using Content.Server.StationEvents.Events;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(LiquidSlimeRule))]
public sealed partial class LiquidSlimeRuleComponent : Component
{
    [DataField]
    public float LiquidThreshold = 5000f;

    [DataField]
    public int MinSlimesToSpawn = 1;

    [DataField]
    public int MaxSlimesToSpawn = 5;

    [DataField]
    public List<EntProtoId> SlimePrototypes = new()
    {
        "MobAdultSlimesBlueAngry",
        "MobAdultSlimesGreenAngry",
        "MobAdultSlimesYellowAngry",
        "ReagentSlime",
        "ReagentSlimeBeer",
        "ReagentSlimePax",
        "ReagentSlimeNocturine",
        "ReagentSlimeTHC",
        "ReagentSlimeSalicylicAcid",
        "ReagentSlimeToxin",
        "ReagentSlimeNapalm",
        "ReagentSlimeOmnizine",
        "ReagentSlimeMuteToxin",
        "ReagentSlimeNorepinephricAcid",
        "ReagentSlimeEphedrine",
        "ReagentSlimeRobustHarvest"
    };
}