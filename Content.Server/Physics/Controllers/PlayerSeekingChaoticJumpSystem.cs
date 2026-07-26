using Content.Server.Physics.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Numerics;

namespace Content.Server.Physics.Controllers;

public sealed class PlayerSeekingChaoticJumpSystem : VirtualController
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSeekingChaoticJumpComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<PlayerSeekingChaoticJumpComponent> comp, ref MapInitEvent args)
    {
        comp.Comp.NextJumpTime = _gameTiming.CurTime + TimeSpan.FromSeconds(_random.NextFloat(comp.Comp.JumpMinInterval, comp.Comp.JumpMaxInterval));
    }

    public override void UpdateBeforeSolve(bool prediction, float frameTime)
    {
        base.UpdateBeforeSolve(prediction, frameTime);

        var query = EntityQueryEnumerator<PlayerSeekingChaoticJumpComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.NextJumpTime <= _gameTiming.CurTime)
            {
                Jump(uid, comp);
                comp.NextJumpTime += TimeSpan.FromSeconds(_random.NextFloat(comp.JumpMinInterval, comp.JumpMaxInterval));
            }
        }
    }

    private void Jump(EntityUid uid, PlayerSeekingChaoticJumpComponent component)
    {
        var transform = Transform(uid);
        var startPos = _transform.GetWorldPosition(uid);
        Vector2 targetPos;
        var aimAtPlayer = false;
        var nearestDist = float.MaxValue;
        EntityUid? nearest = null;

        if (_random.Prob(component.SeekProbability))
        {
            var ents = _lookup.GetEntitiesInRange(uid, component.SeekRange);
            foreach (var e in ents)
            {
                if (e == uid)
                    continue;

                if (!_entManager.HasComponent<ActorComponent>(e))
                    continue;

                var p = _transform.GetWorldPosition(e);
                var d = Vector2.Distance(startPos, p);

                if (d < nearestDist)
                {
                    nearestDist = d;
                    nearest = e;
                }
            }

            if (nearest != null)
                aimAtPlayer = true;
        }

        if (aimAtPlayer && nearest != null)
        {
            var playerPos = _transform.GetWorldPosition(nearest.Value);
            var baseDir = MathF.Atan2(playerPos.Y - startPos.Y, playerPos.X - startPos.X);
            var jitter = _random.NextFloat(-component.SeekJitter, component.SeekJitter);
            var direction = baseDir + jitter;
            var desiredRange = Math.Clamp(nearestDist * _random.NextFloat(0.5f, 1f), component.RangeMin, component.RangeMax);

            var ray = new CollisionRay(startPos, new Vector2(MathF.Cos(direction), MathF.Sin(direction)), component.CollisionMask);
            var rayCastResults = _physics.IntersectRay(transform.MapID, ray, desiredRange, uid, returnOnFirstHit: false).FirstOrNull();

            if (rayCastResults != null)
            {
                var hit = rayCastResults.Value.HitPos;
                targetPos = new Vector2(hit.X - (float)Math.Cos(direction), hit.Y - (float)Math.Sin(direction));
            }
            else
            {
                targetPos = new Vector2(startPos.X + desiredRange * MathF.Cos(direction), startPos.Y + desiredRange * MathF.Sin(direction));
            }
        }
        else
        {
            var direction = _random.NextAngle();
            var range = _random.NextFloat(component.RangeMin, component.RangeMax);
            var ray = new CollisionRay(startPos, direction.ToVec(), component.CollisionMask);
            var rayCastResults = _physics.IntersectRay(transform.MapID, ray, range, uid, returnOnFirstHit: false).FirstOrNull();

            if (rayCastResults != null)
            {
                var hit = rayCastResults.Value.HitPos;
                targetPos = new Vector2(hit.X - (float)Math.Cos(direction), hit.Y - (float)Math.Sin(direction));
            }
            else
            {
                targetPos = new Vector2(startPos.X + range * (float)Math.Cos(direction), startPos.Y + range * (float)Math.Sin(direction));
            }
        }

        Spawn(component.Effect, transform.Coordinates);
        _transform.SetWorldPosition(uid, targetPos);
    }
}