using NUnit.Framework.Internal;
using Still.GOAP.Action;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
namespace Still.GOAP.Planner
{
    public static class Planner
    {
        public static Stack<IAction> Planning(List<IAction> usableActions,
                                              Dictionary<string, int> currentWorldStates,
                                              Dictionary<string, Condition> HighestPriorityGoal)
        {
            var open = new PriorityQueue<Node>();
            var bestG = new Dictionary<string, int>();

            var start = new Node(null, currentWorldStates, null, 0, Heruistic(currentWorldStates,
                                                                              HighestPriorityGoal,
                                                                              usableActions));
            open.Enqueue(start, start.F);
            bestG[StateHash(currentWorldStates)] = 0;
            while (open.Count > 0)
            {
                var current = open.Dequeue();

                // ゴールに達成していたら経路を再構築して返す
                if (Evalution.IsSatisfied(currentWorldStates, HighestPriorityGoal))
                {
                    return ReconstructPath(current);
                }
                // 使用可能なアクションを全て試す
                foreach (var action in usableActions)
                {
                    var nextState = ApplyEffects(current.States, action.Effects);
                    int newG = current.G + action.ActionCost;
                    var hash = StateHash(nextState);

                    // 既に同じかそれ以上の効率的な経路が見つかっていればスキップ
                    if (bestG.TryGetValue(hash, out int oldG) && oldG <= newG) continue;

                    // 最良コストを更新し、新しいノードをOpenに追加
                    bestG[hash] = newG;
                    int h = Heruistic(currentWorldStates, HighestPriorityGoal, usableActions);
                    var nextNode = new Node(current, nextState, action, newG, h);
                    open.Enqueue(nextNode, nextNode.F);
                }
            }
            return null;
        }
        /// <summary>
        /// 推定コストを計算するヒューリスティック関数
        /// </summary>
        /// <param name="currentStates"></param>
        /// <param name="goal"></param>
        /// <param name="usableActions"></param>
        /// <returns></returns>
        private static int Heruistic(Dictionary<string, int> currentStates,
                                     Dictionary<string, Condition> goal,
                                     List<IAction> usableActions)
        {
            //既にゴールに達している？
            if (Evalution.IsSatisfied(currentStates, goal)) return 0;

            var tempStates = new Dictionary<string, int>();
            //推定ステップ数
            int estimatedSteps = 0;
            int maxSteps = 15;

            while (estimatedSteps < maxSteps)
            {
                IAction bestAction = FindBestAction(currentStates, goal, usableActions);

                // そんな事ないと思いたいが、アクションのリストがnullだったらコストをintの最大値を返す
                if (bestAction == null)
                {
                    return int.MaxValue;
                }

                // 選んだアクションを1回実行したと仮定して、ステートを更新
                foreach (var effect in bestAction.Effects)
                {
                    tempStates[effect.Key] = effect.Value;
                }

                // アクションを1回実行したのでステップを1足す
                estimatedSteps++;
                // もしゴールに達成していたらステップ数を返す
                if (Evalution.IsSatisfied(currentStates, goal))
                {
                    return estimatedSteps;
                }
            }
            // 最大回数試してみてもたどり着けなかった
            return int.MaxValue;
        }
        /// <summary>
        /// 効果を反映したあとどれくらいゴールの条件が達成されたかを見て達成数は一番大きかったアクションを返します
        /// </summary>
        /// <param name="currentStates"></param>
        /// <param name="goal"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        private static IAction FindBestAction(Dictionary<string, int> currentStates,
                                              Dictionary<string, Condition> goal,
                                              List<IAction> actions)
        {
            IAction bestAction = null;
            int maxProgress = -1;

            foreach (var action in actions)
            {
                // アクションの前提条件が達成されていなかったらスキップ
                if (!Evalution.IsSatisfied(currentStates, action.Preconditions)) continue;

                // 達成数
                int progress = CountGoalProgress(currentStates, goal, action.Effects);

                // 現在の最大の達成数より多かったら更新する
                if (progress > maxProgress)
                {
                    maxProgress = progress;
                    bestAction = action;
                }
            }
            return bestAction;
        }
        /// <summary>
        /// アクションの効果によって、ゴール条件が「未達」から「達成」に変わったものの数を数えています。
        /// </summary>
        /// <param name="currentStates"></param>
        /// <param name="goal"></param>
        /// <param name="effects"></param>
        /// <returns></returns>
        private static int CountGoalProgress(Dictionary<string, int> currentStates,
                                             Dictionary<string, Condition> goal,
                                             Dictionary<string, int> effects)
        {
            // 達成数
            int progress = 0;
            // 比較用のステート
            var nextState = new Dictionary<string, int>(currentStates);
            foreach (var effect in effects)
            {
                nextState[effect.Key] = effect.Value;
            }
            foreach (var goalEntry in goal)
            {
                // 効果が反映される前のステートはゴールの条件が達成されているか
                bool isMetBefore = Evalution.IsSatisfied(currentStates, new Dictionary<string, Condition>
                {
                    {
                        goalEntry.Key,
                        goalEntry.Value
                    }
                });
                // 効果が反映された後のステートはゴールの条件が達成されているか
                bool isMetAfter = Evalution.IsSatisfied(nextState, new Dictionary<string, Condition>
                {
                    {
                        goalEntry.Key,
                        goalEntry.Value
                    }
                });
                // 効果が反映された後だけゴールの条件が達成されていたら達成数を1足す
                if (!isMetBefore && isMetAfter)
                {
                    progress++;
                }
            }
            return progress;
        }
        /// <summary>
        /// ステートにアクションの効果を適用し、新しいステートを返します。
        /// </summary>
        private static Dictionary<string, int> ApplyEffects(
            Dictionary<string, int> state,
            Dictionary<string, int> effects)
        {
            var newState = new Dictionary<string, int>(state);
            foreach (var eff in effects)
                newState[eff.Key] = eff.Value;
            return newState;
        }
        /// <summary>
        /// ステートをキーでソートし、一意の文字列（ハッシュ）を生成します。
        /// </summary>
        private static string StateHash(Dictionary<string, int> state)
        {
            // キーでソートすることで、順序が違っても同じ内容なら同じハッシュになるようにする
            return string.Join("|", state.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        }
        /// <summary>
        /// ゴールノードから親を遡ってアクションの経路を構築します。
        /// </summary>
        private static Stack<IAction> ReconstructPath(Node goalNode)
        {
            var path = new Stack<IAction>();
            var current = goalNode;
            while (current.Parent != null && current.Action != null)
            {
                path.Push(current.Action);
                current = current.Parent;
            }
            return path;
        }
        /// <summary>
        /// GOAPのプランニングをするためのノードクラス
        /// </summary>
        public class Node
        {
            public Node Parent;
            public Dictionary<string, int> States;
            public IAction Action;
            public int G;
            public int H;
            public int F => G + H;

            public Node(Node parent, Dictionary<string, int> states, IAction action, int g, int h)
            {
                Parent = parent;
                States = states;
                Action = action;
                G = g;
                H = h;
            }
        }
        /// <summary>
        /// 最も小さい値を取り出すQueue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class PriorityQueue<T>
        {
            private List<(T item, int priority)> elements = new();
            public int Count => elements.Count;
            public void Enqueue(T item, int priority)
            {
                elements.Add((item, priority));
            }
            public T Dequeue()
            {
                var index = 0;
                for (int i = 1; i < elements.Count; i++)
                {
                    if (elements[index].priority > elements[i].priority)
                    {
                        index = i;
                    }
                }
                var best = elements[index].item;
                elements.RemoveAt(index);
                return best;
            }
        }
    }
}
