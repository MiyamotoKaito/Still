# ゴーストAction改善案（ホラー演出の強化）

ゴーストAI（GOAP）の振る舞いを見直し、ホラーとしての怖さを上げるための設計メモです。
実装するときは、プロジェクトルートの `CLAUDE.md`（アーキテクチャ・命名・C# 9制約）に必ず従ってください。

## 前提：関連ファイル

| 役割 | パス |
| --- | --- |
| Action基底 / 契約 | `Assets/Scripts/GOAP/Action/ActionBase.cs`, `IAction.cs` |
| 既存Action | `Assets/Scripts/GOAP/Action/Actions/*.cs` |
| ゴーストの体（移動・ワープ） | `Assets/Scripts/GOAP/Agent/GAgent.cs`（`IAgentController`） |
| ゴーストの視界 | `Assets/Scripts/GOAP/Agent/GhostSightTicker.cs`, `GhostSensorView.cs` |
| ゴーストの設定 | `Assets/Scripts/GOAP/Data/GhostConfig.cs`（アセット: `Assets/Data/GhostConfig.asset`） |
| Action一覧アセット | `Assets/Data/EnemyActionConfig.asset`（`GhostActionData`） |
| Goalアセット | `Assets/Data/Goals/*.asset`（`SubGoalConfig`） |
| ワールドステートのキー | `Assets/Scripts/Common/WorldState/WorldStateType.cs` |
| 恐怖レベル | `Assets/Scripts/Player/Domain/SanValue.cs`（`ToFearLevel()`）→ `PlayerFearLevelPresenter` が `FearLevel` に書き込む |
| 脱出ギミック | `Assets/Scripts/Escape/View/Tv.cs`, `RecorderPlayer.cs`, `DoorView.cs`, `LightSwitch.cs` |

### 守ること
- `WorldStateType` はアセットに数値で保存されるため、**末尾に追加するだけ**にする（並べ替え・途中への挿入は禁止）。追加後は `WorldStateConfig` の「全てのワールドステートを設定」で初期値を登録する。
- 新しいActionは `ActionBase` を継承し、`[Serializable]` を付けて `EnemyActionConfig.asset` の `SubclassSelector` から追加できるようにする。
- 依存は `[Inject]` メソッドで受け取る（既存Actionと同じ書き方）。`SerializeReference` の `IEvent` は `resolver.Inject` で注入する。
- 数値パラメータはActionにconstで埋め込まず、`GhostConfig` にActionごとのHeaderを作って持たせる（既存の書き方に合わせる）。

## 現状の問題（怖くない理由）

1. **追跡がアクションゲームになっている**
   `ChaseAction.SetTarget` は、見つけた瞬間に "Chase" のSEを鳴らしてダッシュする。「気づかれたかもしれない」間がないので、恐怖ではなく鬼ごっこの緊張になっている。
2. **すべてのActionが1回で終わる**
   `KnockAction` は1回鳴ってSANを-5して終わり、`TurnOffLightAction` も照明を消すだけ。溜めも、止んだ後の静寂もない。
3. **ゴーストの行動がプレイヤーと関係ない**
   `PatrolAction` は自分の周りのランダムな地点へ歩くだけ。`TeleportAction` は「ゴースト自身から最も遠い地点」へ飛ぶので、プレイヤーとの距離感が生まれない。さらに `Teleport.asset` は優先度0で、実質ほぼ選ばれない。
4. **FearLevelが上がっても振る舞いの質が変わらない**
   `FearLevel` を見ているのは `Catch` / `Catch2` の発動条件だけ。SANが減ってもゴーストのやることは同じ。

## 改善案

実装順のおすすめは **1 → 3 → 2**。その後に 4・5・6。

### 1. NoticeAction：見つけてもすぐには追わない（優先度：最高）

**狙い**：見つかったかどうか分からない時間を作る。

- `PlayerVisible=1` になったら、まず `StopMove()` してプレイヤーの方へゆっくり回転する。
- `GhostConfig.NoticeSeconds`（目安1.5〜2秒）のあいだ**無音で**見つめ続けてから完了する。
- 完了時の効果で新しいワールドステート（例 `Noticed=1`）を立て、`ChaseAction` の前提条件にする。これで Catch系のプランが「Notice → Chase → Catch」になる。
- 見つめている間にプレイヤーが視界から外れたら（`PlayerVisible=0`）中断し、追跡せずに「その場で見回す」へ戻す。「やり過ごせた」体験を作るため。
- `ChaseAction` で "Chase" のSEを鳴らすタイミングは、Notice完了後のままでよい。Notice中は鳴らさないこと。

**追加するもの**
- `WorldStateType.Noticed`（末尾に追加）
- `GhostConfig`：`NoticeSeconds`, `NoticeTurnSpeed`
- `IAgentController` に回転用のメソッド（例 `LookAt(Vector3 position, float speed)`）。`GAgent` 側で `NavMeshAgent.updateRotation` を一時的に切って回転させる。
- `Noticed` は追跡を終えたとき（Catchの達成後、または `ChaseAction.Execute` で見失ったとき）に0へ戻す。

### 2. StalkAction：背後に立つ（TeleportActionの置き換え）

**狙い**：「後ろにいる気がする」を本当にやる。

- プレイヤーのカメラの後方 `StalkDistance` m の地点をNavMesh上で探し、**プレイヤーの視界の外であるときだけ**ワープする。
- ワープ後は足音だけを鳴らし、遅い速度で少しずつ近づく。
- プレイヤーが振り向いて `GhostDetected=1` になったら即座に消える（遠いテレポート地点へワープ）。これで「振り向いたら誰もいない」になる。
- 振り向かれないまま `StalkCatchDistance` まで近づいたら、FocusMeかCatchへつなぐ。

```csharp
// SetTarget の核となる処理（イメージ）
var p = _player.Transform;
var behind = p.position - p.forward * agent.Config.StalkDistance;
if (NavMesh.SamplePosition(behind, out var hit, 2f, NavMesh.AllAreas))
{
    // 見られている方向には出現しない
    var toGhost = (hit.position - p.position).normalized;
    if (Vector3.Dot(p.forward, toGhost) < -0.3f) agent.Teleport(hit.position);
}
```

**注意点**
- `IPlayerAvatar.Transform` がカメラの向きと一致していない場合は、カメラの forward を取れる手段をCommonの契約に足す。Viewを直接参照しない。
- NavMeshの判定はゴースト側（`GAgent`）に寄せ、Actionは `IAgentController` 経由で呼ぶ（例 `TryGetNavMeshPosition`）。ActionからNavMeshを直接触らない。
- 既存の `Teleport.asset`（優先度0）は使わなくなるので、Stalk用のGoalを新しく作る。

### 3. Knockを段階的にする（KnockEscalation）

**狙い**：溜めと、止んだ後の静寂で怖がらせる。

今の `KnockAction`（1回鳴って終わり）を、次のような段階式にする。

1. 遠くのドアをノックする（音量小・間隔長め）
2. 近くのドアをノックし、間隔が短くなる
3. プレイヤーがいる部屋のドアを激しく叩く
4. **ぴたりと止み、`KnockSilenceSeconds`（5〜10秒）の無音**
5. ドアノブがゆっくり回る（`DoorView` のアニメーション）、またはドアの下に影が差す

- SANのダメージは各段階で少しずつ与え、4の静寂の後に大きめに与える。
- 段階の進行はAction内部の状態で管理してよい（1つのActionで `Perform` が完了するまで続く）。または段階ごとにActionを分け、`KnockStage` のようなワールドステートでつなぐ。後者のほうが、間にプレイヤーが部屋を出た場合に再計画しやすい。
- プレイヤーが途中で部屋を出たら（`InTheRoom=0`）中断する。
- 音の発生位置はドアのTransformにする。`IAudioService.PlaySe` は位置付き（AudioSource指定）で鳴らす。

**追加するもの**
- `GhostConfig`：`KnockIntervals`（段階ごとの間隔）, `KnockSilenceSeconds`
- ドアノブ演出：`IEvent` 実装として `Escape` 側に置き、Actionからは `SerializeReference` の `IEvent` で呼ぶ（`CatchAction` / `FocusMeAction` と同じ形）

### 4. FlickerApproachAction：暗転するたびに近づく

**狙い**：ウィーピング・エンジェル型の接近。

- プレイヤーがいる部屋の照明を点滅させる（`ILightSwitchUsecase` を利用）。
- **暗転している間だけ**、ゴーストがプレイヤーへ一定距離ずつワープして近づく。明るくなるたびに近くに立っている。
- 最後は照明が消えたままになり、Catchへつなぐ。
- 点滅のON/OFFを照明スイッチの `Toggle` で表現すると、プレイヤーの操作と区別できなくなる。そのため、ゴースト専用の「一時的に消す」操作を `ILightSwitchUsecase` に足すことを検討する。

### 5. ReplayAction：プレイヤーの行動をなぞる

**狙い**：姿を見せずに「見られている」と感じさせる。低コスト。

- プレイヤーが調べた `Tv` / `RecorderPlayer` を、ゴーストが勝手にもう一度再生する。
- 対象は `EscapeProgress.IsAchieved` で「調べ終わった」ものから選ぶ。
- Escape機能はGOAPから直接参照できないので、Commonに契約（例 `IHauntableQuery` / `IHauntable.Haunt()`）を置き、Escape側で実装・登録する。

### 6. FearLevelで出し方を変える（Goalの設計）

Actionを足すだけでなく、Goalの発動条件で「どの恐怖段階で何をするか」を分ける。

| FearLevel | ゴーストの出し方 | 使うAction / Goal |
| --- | --- | --- |
| 0〜1 | 痕跡だけ（物音、開いたドア、勝手に動く家電） | Replay, Patrol |
| 2 | 一瞬見えるだけ（廊下の奥に立っていて、見たら消える） | Stalk（遠距離版）, FocusMe |
| 3 | 近くにいる | Stalk, KnockEscalation |
| 4 | 接触 | FlickerApproach → Catch |

- 各Goalの `_settingGoalConditions` に `FearLevel` の範囲を入れて実現する。
- 序盤に姿を見せすぎないことが、怖さを長持ちさせるコツ。

## 発展：ディレクター

プレイヤーとゴーストの距離や、最後に怖がらせてからの経過時間を見て、緊張と緩和の波を作る `ITickable`（例 `TensionDirector`）を足す。結果をワールドステート（例 `Tension`）に書き込み、Goalの発動条件で使う。Alien: Isolationの「近くにいるが見つけない時間」を管理する仕組みに近い。GOAPのGoal優先度と相性が良い。

## 実装時のチェックリスト

- [ ] `WorldStateType` は末尾に追加したか。`WorldStateConfig` の初期値も登録したか
- [ ] 新しいActionを `EnemyActionConfig.asset` に追加し、前提条件・効果・コストを設定したか
- [ ] Goalアセットを作り、優先度が既存Goal（Catch=6, Knock/TurnOffLight=5, Catch2=4, FocusMe=3, Patrol=1）と矛盾しないか
- [ ] `Execute` の中断条件で、立てたワールドステートを戻しているか（`ChaseAction.Execute` を参考にする）
- [ ] モジュールをまたぐ参照はCommonの契約経由にしたか
- [ ] C# 9の範囲で書いたか（`record struct`、file-scoped namespace は使わない）
