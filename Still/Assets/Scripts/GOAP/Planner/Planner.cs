using System.Collections.Generic;
using System.Linq;
using Still.Common;
using Template.Runtime;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ゴールから逆向きにA*で探索してアクション列を作る
    ///</summary>
    public static class Planner
    {
        ///<summary>
        ///ゴールまでのアクション列を作る（見つからなければnull）
        ///</summary>
        public static Stack<IAction> Planning(IReadOnlyList<IAction> usableActions,
                                              IReadOnlyDictionary<WorldStateType, int> currentStates,
                                              IReadOnlyDictionary<WorldStateType, Condition> goal)
        {
            var open = new PriorityQueue<Node, int>();
            var closed = new HashSet<string>();

            var start = new Node(null, new Dictionary<WorldStateType, Condition>(goal.ToDictionary(kv => kv.Key, kv => kv.Value)),
                                 null, 0, CountUnsatisfiedConditions(currentStates, goal));
            open.Enqueue(start, start.F);

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                var hash = StateHash(current.SubGoals);
                if (!closed.Add(hash)) continue;

                if (Evaluation.IsSatisfied(currentStates, current.SubGoals))
                {
                    return ReconstructPath(current);
                }

                foreach (var action in usableActions)
                {
                    if (!CanSatisfyAnyGoal(action, current.SubGoals)) continue;

                    var subGoals = ApplyRegression(current.SubGoals, action);
                    if (closed.Contains(StateHash(subGoals))) continue;

                    var next = new Node(current, subGoals, action,
                                        current.G + action.ActionCost,
                                        CountUnsatisfiedConditions(currentStates, subGoals));
                    open.Enqueue(next, next.F);
                }
            }

            Debug.LogWarning("[Planner] ゴールに到達する経路が見つかりませんでした");
            return null;
        }

        // アクションの効果がサブゴールの少なくとも1つを満たせるか
        private static bool CanSatisfyAnyGoal(IAction action, IReadOnlyDictionary<WorldStateType, Condition> subGoals)
        {
            foreach (var effect in action.Effects)
            {
                if (subGoals.TryGetValue(effect.Key, out var condition) && condition.IsSatisfiedBy(effect.Value)) return true;
            }
            return false;
        }

        // 効果で満たされるサブゴールを外し、代わりにアクションの前提条件を加える
        private static Dictionary<WorldStateType, Condition> ApplyRegression(IReadOnlyDictionary<WorldStateType, Condition> subGoals, IAction action)
        {
            var result = subGoals.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var effect in action.Effects)
            {
                if (result.TryGetValue(effect.Key, out var condition) && condition.IsSatisfiedBy(effect.Value))
                {
                    result.Remove(effect.Key);
                }
            }
            foreach (var precondition in action.Preconditions)
            {
                result[precondition.Key] = precondition.Value;
            }
            return result;
        }

        // ヒューリスティック：今のワールドステートで満たせていないサブゴールの数
        private static int CountUnsatisfiedConditions(IReadOnlyDictionary<WorldStateType, int> currentStates,
                                                      IReadOnlyDictionary<WorldStateType, Condition> subGoals)
        {
            return subGoals.Count(goal => !currentStates.TryGetValue(goal.Key, out int value) || !goal.Value.IsSatisfiedBy(value));
        }

        private static string StateHash(IReadOnlyDictionary<WorldStateType, Condition> subGoals)
        {
            return string.Join("|", subGoals.OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}={kv.Value.Comparison}:{kv.Value.Value}"));
        }

        // 逆向き探索なので、ゴール側から辿った順がそのまま実行順になる
        private static Stack<IAction> ReconstructPath(Node node)
        {
            var actions = new List<IAction>();
            for (var current = node; current.Action != null; current = current.Parent)
            {
                actions.Add(current.Action);
            }
            Debug.Log($"[Planner] 最適パス（コスト{node.G}）: {string.Join(" -> ", actions)}");

            var path = new Stack<IAction>();
            for (int i = actions.Count - 1; i >= 0; i--) path.Push(actions[i]);
            return path;
        }

        private sealed class Node
        {
            public Node(Node parent, Dictionary<WorldStateType, Condition> subGoals, IAction action, int g, int h)
            {
                Parent = parent;
                SubGoals = subGoals;
                Action = action;
                G = g;
                H = h;
            }

            public readonly Node Parent;
            public readonly Dictionary<WorldStateType, Condition> SubGoals;
            public readonly IAction Action;
            public readonly int G;
            public readonly int H;

            public int F => G + H;
        }
    }
}
