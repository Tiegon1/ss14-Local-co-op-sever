using Content.Server.Physics.Controllers;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Physics.Components;

[RegisterComponent, Access(typeof(PlayerSeekingChaoticJumpSystem))]
public sealed partial class PlayerSeekingChaoticJumpComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextJumpTime;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float JumpMinInterval = 5f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float JumpMaxInterval = 15f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int CollisionMask = (int)CollisionGroup.Impassable;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float RangeMin = 5f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float RangeMax = 10f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SeekRange = 15f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SeekProbability = 0.6f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SeekJitter = 0.4f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId Effect = "EffectEmpPulse";
}