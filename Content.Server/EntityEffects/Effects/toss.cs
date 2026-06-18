using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;


public sealed partial class Toss : EntityEffect
{
    [DataField]
    public float BaseImpulse = 50f;

    [DataField]
    public float ScaleImpulse = 1f;

    public override void Effect(EntityEffectBaseArgs args)
    {
        var physics = args.EntityManager.System<SharedPhysicsSystem>();

        var impulse = IoCManager.Resolve<IRobustRandom>()
    .NextVector2(1).Normalized() * BaseImpulse;

        if (args is EntityEffectReagentArgs reagentArgs)
        {
            impulse *= reagentArgs.Scale.Float() * ScaleImpulse;
        }

        physics.ApplyLinearImpulse(args.TargetEntity, impulse);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-toss");
}