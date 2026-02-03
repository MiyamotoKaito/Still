using System.Collections.Generic;
using System.Linq;
using Still.GOAP.Action;
using UnityEngine;

namespace Still.GOAP.Planner
{
    public static class Planner
    {
        public static Stack<IAction> Planning(List<IAction> usableActions,
                                              Dictionary<string, int> currentWorldStates,
                                              Dictionary<string, Condition> highestPriorityGoal)
        {
            var open = new PriorityQueue<Node>();
            var closed = new HashSet<string>();

            var start = new Node(null, highestPriorityGoal, null, 0,
                                CountUnsatisfiedConditions(currentWorldStates, highestPriorityGoal));
            open.Enqueue(start, start.F);

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                var hash = StateHash(current.SubGoals);

                if (closed.Contains(hash)) continue;
                closed.Add(hash);

                // ゴール到達チェック
                if (Evalution.IsSatisfied(currentWorldStates, current.SubGoals))
                {
                    return ReconstructPath(current);
                }

                // アクション展開
                foreach (var action in usableActions)
                {
                    if (!CanSatisfyAnyGoal(action, current.SubGoals)) continue;

                    var newSubGoal = ApplyRegression(current.SubGoals, action);
                    var newHash = StateHash(newSubGoal);

                    if (closed.Contains(newHash)) continue;

                    int newG = current.G + action.ActionCost;
                    int h = CountUnsatisfiedConditions(currentWorldStates, newSubGoal);
                    var nextNode = new Node(current, newSubGoal, action, newG, h);
                    open.Enqueue(nextNode, nextNode.F);
                }
            }

            Debug.LogWarning("プランニング失敗: ゴールに到達する経路が見つかりませんでした");
            return null;
        }

        /// <summary>
        /// アクションの効果がサブゴールの少なくとも1つを満たせるかチェック
        /// </summary>
        private static bool CanSatisfyAnyGoal(IAction action, Dictionary<string, Condition> subGoals)
        {
            foreach (var effect in action.Effects)
            {
                if (subGoals.ContainsKey(effect.Key) &&
                    IsSingleConditionMet(effect.Value, subGoals[effect.Key]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// サブゴールからアクションの効果を除去し、前提条件を追加
        /// </summary>
        private static Dictionary<string, Condition> ApplyRegression(
            Dictionary<string, Condition> subGoals, IAction action)
        {
            var newSubGoals = new Dictionary<string, Condition>(subGoals);

            // 効果で満たされる条件を削除
            foreach (var effect in action.Effects)
            {
                if (newSubGoals.TryGetValue(effect.Key, out var condition) &&
                    IsSingleConditionMet(effect.Value, condition))
                {
                    newSubGoals.Remove(effect.Key);
                }
            }

            // 前提条件を追加
            foreach (var precondition in action.Preconditions)
            {
                newSubGoals[precondition.Key] = precondition.Value;
            }

            return newSubGoals;
        }

        /// <summary>
        /// 単一の条件が満たされているかチェック（Evalution.IsSatisfiedのラッパー）
        /// </summary>
        private static bool IsSingleConditionMet(int value, Condition condition)
        {
            var state = new Dictionary<string, int> { { "temp", value } };
            var cond = new Dictionary<string, Condition> { { "temp", condition } };
            return Evalution.IsSatisfied(state, cond);
        }

        /// <summary>
        /// 満たされていない条件の数をカウント
        /// </summary>
        private static int CountUnsatisfiedConditions(Dictionary<string, int> currentStates,
                                                     Dictionary<string, Condition> subGoals)
        {
            return subGoals.Count(goal => !IsSingleConditionMet(
                currentStates.GetValueOrDefault(goal.Key, int.MinValue), goal.Value));
        }

        /// <summary>
        /// 状態のハッシュ文字列を生成
        /// </summary>
        private static string StateHash(Dictionary<string, Condition> subGoals)
        {
            return string.Join("|", subGoals.OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}={kv.Value.Comparison}:{kv.Value.Value}"));
        }

        /// <summary>
        /// アクション経路を再構築
        /// </summary>
        private static Stack<IAction> ReconstructPath(Node goalNode)
        {
            var actions = new List<IAction>();
            var current = goalNode;

            while (current.Parent != null && current.Action != null)
            {
                actions.Add(current.Action);
                current = current.Parent;
            }
            Debug.Log($"最適パスを発見 : 累計コスト{goalNode.F} \n{string.Join(" -> ", actions)}");

            // リストは既に正しい順序（開始→ゴール）なので、そのままStackに詰める
            var path = new Stack<IAction>();
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                path.Push(actions[i]);
            }
            return path;
        }

        public class Node
        {
            public Node Parent;
            public Dictionary<string, Condition> SubGoals;
            public IAction Action;
            public int G;
            public int H;
            public int F => G + H;

            public Node(Node parent, Dictionary<string, Condition> subGoals, IAction action, int g, int h)
            {
                Parent = parent;
                SubGoals = subGoals;
                Action = action;
                G = g;
                H = h;
            }
        }

        public class PriorityQueue<T>
        {
            private List<(T item, int priority)> elements = new();
            public int Count => elements.Count;

            public void Enqueue(T item, int priority) => elements.Add((item, priority));

            public T Dequeue()
            {
                int index = 0;
                for (int i = 1; i < elements.Count; i++)
                {
                    if (elements[i].priority < elements[index].priority)
                        index = i;
                }
                var best = elements[index].item;
                elements.RemoveAt(index);
                return best;
            }
        }
    }
}