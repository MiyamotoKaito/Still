using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Still.Common;
using UnityEngine;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Still.Player
{
    ///<summary>
    ///SAN値に応じた常時の効果、敵を見たときの反応、幻覚を動かす
    ///</summary>
    public sealed class FearController : IInitializable, ITickable, IDisposable
    {
        public FearController(FearConfig config, PlayerStatusEntity status, IWorldStates worldStates, FearEffectContext context)
        {
            _config = config;
            _status = status;
            _worldStates = worldStates;
            _context = context;
        }

        private float _fear;
        private float _nextGhostReactionTime;
        private float _hallucinationTimer;
        private IDisposable _subscription;

        private readonly FearConfig _config;
        private readonly PlayerStatusEntity _status;
        private readonly IWorldStates _worldStates;
        private readonly FearEffectContext _context;
        private readonly CancellationTokenSource _cts = new();

        // 0=平常、1=SAN値0
        private float TargetFear => 1f - _status.San.CurrentValue.Value / _status.San.CurrentValue.Max;
        private int FearLevel => _status.San.CurrentValue.ToFearLevel();

        ///<summary>
        ///敵の視認の監視を始める
        ///</summary>
        public void Initialize()
        {
            // 前回の再生で残った状態（SOに保存される演出データの実行時の値）を消してから始める
            foreach (var effect in _config.SanEffects) effect?.Reset(_context);
            _fear = TargetFear;
            ResetHallucinationTimer();

            _subscription = _worldStates.OnStateChanged
                .Where(key => key == WorldStateType.GhostDetected && _worldStates.GetStateValue(WorldStateType.GhostDetected) == 1)
                .Subscribe(_ => OnGhostSeen());
        }

        ///<summary>
        ///常時の効果と幻覚を1フレーム分進める
        ///</summary>
        public void Tick()
        {
            if (_context.IsDisposed) return;

            _fear =Mathf.MoveTowards(_fear, TargetFear, _config.FearFollowSpeed * Time.deltaTime);
            foreach (var effect in _config.SanEffects) effect?.Apply(_fear, _context);
            _context.ApplyCamera();

            TickHallucination();
        }

        ///<summary>
        ///演出を止めて元に戻す
        ///</summary>
        public void Dispose()
        {
            _subscription?.Dispose();
            _cts.Cancel();
            _cts.Dispose();
            // スタミナ回復の倍率などViewに依存しない値も戻したいが、Viewが既に無い場合は触らない
            if (_context.IsDisposed)
            {
                _context.Status.RecoveryMultiplier = 1f;
                return;
            }
            foreach (var effect in _config.SanEffects) effect?.Reset(_context);
            _context.ShakeDutch = 0f;
            _context.ApplyCamera();
        }

        private void OnGhostSeen()
        {
            // 見るたびに毎回驚かせると慣れてしまうので、間隔を空ける
            if (Time.time < _nextGhostReactionTime) return;
            _nextGhostReactionTime = Time.time + _config.GhostSeenCooldown;
            Play(Pick(_config.GhostSeenReactions));
        }

        private void TickHallucination()
        {
            if (FearLevel < _config.MinHallucinationFearLevel) return;

            _hallucinationTimer -= Time.deltaTime;
            if (_hallucinationTimer > 0f) return;

            ResetHallucinationTimer();
            Play(Pick(_config.Hallucinations));
        }

        // SAN値が低いほど幻覚の間隔を短くする
        private void ResetHallucinationTimer()
        {
            var range = Vector2.Lerp(_config.HallucinationIntervalCalm, _config.HallucinationIntervalInsane, _fear);
            _hallucinationTimer = Random.Range(range.x, range.y);
        }

        private FearReaction Pick(IReadOnlyList<FearReaction> reactions)
        {
            var candidates = reactions.Where(r => r.IsAvailable(FearLevel)).ToList();
            float total = candidates.Sum(r => r.Weight);
            if (total <= 0f) return null;

            float pick = Random.value * total;
            foreach (var reaction in candidates)
            {
                pick -= reaction.Weight;
                if (pick <= 0f) return reaction;
            }
            return candidates[candidates.Count - 1];
        }

        private void Play(FearReaction reaction)
        {
            if (reaction == null) return;
            foreach (var action in reaction.Actions)
            {
                if (action != null) action.PlayAsync(_context, _cts.Token).Forget();
            }
        }
    }
}
