using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Fluids.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events;

public sealed class LiquidSlimeRule : StationEventSystem<LiquidSlimeRuleComponent>
{
    [Dependency] private readonly SolutionContainerSystem _solution = default!;

    protected override void Started(
        EntityUid uid,
        LiquidSlimeRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryGetRandomStation(out var station))
        {
            ForceEndSelf(uid, gameRule);
            return;
        }

        var puddles = new List<EntityCoordinates>();
        var totalLiquid = 0f;

        var query = EntityQueryEnumerator<
            PuddleComponent,
            SolutionContainerManagerComponent,
            TransformComponent>();

        while (query.MoveNext(out var puddleUid, out var puddle, out _, out var transform))
        {
            if (CompOrNull<StationMemberComponent>(transform.GridUid)?.Station != station)
                continue;

            if (!_solution.TryGetSolution(
                    puddleUid,
                    puddle.SolutionName,
                    out var solutionEntity))
            {
                continue;
            }

            var volume = solutionEntity.Value.Comp.Solution.Volume.Float();
            if (volume <= 0)
                continue;

            totalLiquid += volume;
            puddles.Add(transform.Coordinates);
        }

        if (totalLiquid < component.LiquidThreshold || puddles.Count == 0)
        {
            ForceEndSelf(uid, gameRule);
            return;
        }

        for (var i = 0; i < component.SlimesToSpawn; i++)
        {
            var coordinates = RobustRandom.Pick(puddles);
            Spawn(component.SlimePrototype, coordinates);
        }

        ForceEndSelf(uid, gameRule);
    }
}