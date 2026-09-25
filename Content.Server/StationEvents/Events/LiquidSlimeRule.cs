using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Fluids.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Shared.Chemistry.Components;

namespace Content.Server.StationEvents.Events;

public sealed class LiquidSlimeRule : StationEventSystem<LiquidSlimeRuleComponent>
{
    [Dependency] private readonly SolutionContainerSystem _solution = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    private readonly HashSet<EntityUid> _nearbyEntities = new();

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

        var stationPuddles =
            new Dictionary<EntityUid, (EntityCoordinates Coordinates, float Volume)>();

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

            stationPuddles.Add(puddleUid, (transform.Coordinates, volume));
        }

        var puddlesInQualifyingAreas = new List<EntityCoordinates>();

        foreach (var (puddleUid, puddle) in stationPuddles)
        {
            _nearbyEntities.Clear();
            _lookup.GetEntitiesInRange(
                puddle.Coordinates,
                component.PuddleRange,
                _nearbyEntities);

            var localVolume = 0f;

            foreach (var nearbyEntity in _nearbyEntities)
            {
                if (stationPuddles.TryGetValue(nearbyEntity, out var nearbyPuddle))
                    localVolume += nearbyPuddle.Volume;
            }

            if (localVolume >= component.LiquidThreshold)
                puddlesInQualifyingAreas.Add(puddle.Coordinates);
        }

        if (puddlesInQualifyingAreas.Count == 0)
        {
            ForceEndSelf(uid, gameRule);
            return;
        }

        var slimesToSpawn = RobustRandom.Next(
            component.MinSlimesToSpawn,
            component.MaxSlimesToSpawn + 1);
        var slimePrototype = RobustRandom.Pick(component.SlimePrototypes);

        for (var i = 0; i < slimesToSpawn; i++)
        {
            var coordinates = RobustRandom.Pick(puddlesInQualifyingAreas);
            Spawn(slimePrototype, coordinates);
        }

        ForceEndSelf(uid, gameRule);
    }
}