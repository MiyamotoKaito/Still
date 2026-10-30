# Unityゲーム アーキテクチャ設計テンプレート

Oct 31, 2026 · @Miyamoto

## 使い方

実装AIに渡すときは、冒頭に「この設計に沿って `<機能名>` を実装して」と付けて、本書をそのまま貼り付けます。プロジェクト固有の値は `<...>` のプレースホルダで書いているので、使う前に置き換えてください。

| プレースホルダ | 置き換える内容 | 例 |
| --- | --- | --- |
| `<FeatureA>`, `<FeatureB>` | 機能モジュール名 | `Shelf`, `Customer` |
| `<Shared>` | 複数モジュールが共有するデータ名 | `Wallet` |
| `<VO>` | ValueObject名 | `ItemCount` |
| `<X>` | SOで持つパラメータ名 | `Item` |

技術スタックや命名規約はプロジェクトに合わせて変えて構いません。ただし「1. 前提」のC#バージョン制約はUnityの仕様なので変えられません。

## 1. 前提・技術スタック

設計の総則は **Vertical Slice Architecture + 依存性逆転** です。個人〜小規模開発を想定し、過剰な抽象化はしません。どうしても守れない違反は、理由を残したうえで許容します。

| 用途 | 採用 | 備考 |
| --- | --- | --- |
| エンジン | Unity 6（URP等） |  |
| DI | VContainer | Zenject等でも可。本書はVContainer前提 |
| Reactive | R3 | 状態通知に使用 |
| 非同期 | UniTask |  |
| アセット | Addressables |  |

### C#言語バージョンの制約（必ず守る）

Unity 6のコンパイラはC# 9.0です。C# 10以降の機能は使えません。

- `record struct`、`readonly record struct` は**使用禁止**（C# 10）。
- `record`（クラス）と `init` アクセサは、`IsExternalInit` が.NET 5以降にしかないため使わない。
- file-scoped namespace（`namespace X;`）、global using は使えない（C# 10）。
- 値等価が必要な型は `readonly struct` + `IEquatable<T>` で手書きする（7章のサンプル参照）。
- `HashCode.Combine` は使用可（.NET Standard 2.1）。

## 2. アセンブリ（asmdef）構成と依存ルール

機能モジュール同士は直接参照せず、Commonのinterface越しにだけ協調します（依存性逆転）。全モジュールを参照してよいのはContainerだけです。

| アセンブリ | 置くもの | 参照してよいもの |
| --- | --- | --- |
| `Common`（契約層） | interface / enum / 一部VO / Installer基底 | ライブラリのみ |
| `Utility`（横断インフラ） | Audio / Save / SceneTransition など | ライブラリ、Common |
| 機能・能力モジュール（`<FeatureA>Module`、`InputModule`、`StateMachine` など） | 各機能の実装 | ライブラリ、Common、Utility |
| `Container`（合成ルート） | LifetimeScope | すべて |

- Commonには具象ロジックや状態を置かない。
- **具象は、それを所有するモジュールが登録する**。他モジュールの具象を勝手に登録しない。

## 3. 状態管理（StateMachine）

ゲームのモード遷移はStateMachineで管理します。契約はCommonの `IStateMachine` / `IState` に置きます。

「状態」には2種類あり、必ず区別します。

1. **モード状態**：StateMachineのState（画面やモードの遷移）
2. **ドメイン状態**：Entity + ValueObject（データ）

Stateはコンテナ生成後に `IStateMachine.AddState()` 等で登録します。スコープ（シーン）を寿命境界にするため、**スコープ破棄時に対応するStateを必ず解除**します（`RemoveState` 相当、または `IDisposable` 連動）。破棄済みの参照を残さないためです。

## 4. DIと合成（Installerパターン）

LifetimeScopeはInstallerを集めて回すだけにし、登録内容は各モジュールのInstallerに書きます。

- 基底は `InstallerObject`（`ScriptableObject` + VContainerの `IInstaller`）。Commonに置く。
- 各モジュールは自分の `<Feature>Installer : InstallerObject` を**自モジュールのasmdef内**に持ち、`Install(IContainerBuilder builder)` に自分の登録だけを書く。
- SOデータが必要な場合は、そのInstaller（ScriptableObject資産）自身が `[SerializeField]` で保持する。

```csharp
[SerializeField] private List<InstallerObject> _installers;

protected override void Configure(IContainerBuilder builder)
{
    foreach (var installer in _installers) installer.Install(builder);
}
```

寿命は原則 `Lifetime.Singleton` に統一し、寿命の差はスコープ（シーン）で表現します。`Scoped` / `Transient` は必要なときだけの例外とします。実行時に複数生成するオブジェクトの扱いは7章を参照してください。

## 5. シーン構成と寿命境界

**寿命境界 = Additive Scene、1シーンにつきLifetimeScopeは1つ**です。シーン内はInstallerで複数モジュールを合成し、子スコープはネストしません。

シーンは「モジュール単位」ではなく「文脈単位」で切ります。

| シーン | 役割 |
| --- | --- |
| `Root` | 永続（DontDestroyOnLoad）、グローバル依存 |
| `OutGame` | タイトル、メニュー、ショップなど |
| `InGame` | プレイ中に同居する機能一式 |

- 常に同居する機能は同じシーンに置く。単独でロード／アンロードする必要が本当にある単位だけ別シーンにする。
- 子シーンのLifetimeScopeはRootを親として解決できるよう配線し、ロード順は必ず「親が先」。
- 単体デバッグ用に「Root + 対象モジュール + そのInstaller」だけを起動するデバッグシーンを用意してよい。

## 6. レイヤーとデータフロー（MVP + CQRS風）

書き（Command）と読み（Query）の経路を分けます。**Entityは普通のプロパティだけを持つピュアC#にし、変更通知はそのEntityの値を参照するModelがReactivePropertyで担います**。Presenterと状態の接着面はModelの購読に一本化します。ViewだけがMonoBehaviourで、それ以外はピュアC#に保ってテスト可能にします。

&#91;embedded content: MVP + CQRS風のデータフロー · 6レイヤー\]

| レイヤー | 責務 | 実装 |
| --- | --- | --- |
| Controller | 入力やモード遷移要求を受け、CommandとしてUsecaseを呼ぶ。StateMachineの遷移もここから命令する。Queryは持たない | ピュアC# |
| Usecase | ルールと計算を実行してEntityを更新し、対応するModelへ反映させる。契約はCommonの `I<Feature>Usecase` | ピュアC# |
| Entity | IDで同一性を持ち、複数のVOを普通のプロパティで束ねる。R3に依存せず、変更通知も持たない | ピュアC# |
| Model | Entityの値を参照し、`ReactiveProperty<VO>` 等で公開する。Entity更新後の反映を受けて値を差し替え、購読側へ通知する | ピュアC# + R3 |
| ValueObject | 不変・値等価。生成時にバリデーションする（7章） | ピュアC# |
| Presenter | Modelを購読してViewへ反映する。Viewの生入力はControllerへ中継する | ピュアC# |
| View | 表示と入力受けのみ。ロジックを持たない。Commonの `I<Feature>View` として登録 | MonoBehaviour |

EntityとModelは、適切なスコープに1対1でSingleton登録します。Entityを書き換えるのはUsecaseだけ、Modelを購読するのはPresenterだけ、という向きを守ります。

## 7. ValueObject / Entity / 複数インスタンス / セーブ

### ValueObject

VOは不変・自己バリデーション・値等価です。計算はVOとUsecaseに寄せます。C# 9の制約により、`readonly struct` + `IEquatable<T>` で書きます。

**「種類 × 量」は種類ごとにVOを作らず、1つの汎用VOにまとめます**（`StrawberryCount`、`OrangeCount` のように分けない）。種類が増えても増えるのはデータだけで、共通処理をそのまま使えます。種類の取り違えは、加算時の検証で防ぎます。

```csharp
namespace Common
{
    ///<summary>
    ///アイテムの種類と個数の組
    ///</summary>
    public readonly struct ItemCount : IEquatable<ItemCount>
    {
        public ItemCount(ItemType itemType, int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            ItemType = itemType;
            Value = value;
        }

        public ItemType ItemType { get; }
        public int Value { get; }
        public bool IsEmpty => Value == 0;

        ///<summary>
        ///同じ種類のItemCountを加算する（種類が異なる場合は例外）
        ///</summary>
        public ItemCount Add(ItemCount other)
        {
            if (ItemType != other.ItemType) throw new InvalidOperationException($"{ItemType}と{other.ItemType}は加算できません");

            return new ItemCount(ItemType, Value + other.Value);
        }

        ///<summary>
        ///種類と個数がともに等しいか
        ///</summary>
        public bool Equals(ItemCount other) => ItemType == other.ItemType && Value == other.Value;

        public override bool Equals(object obj) => obj is ItemCount other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(ItemType, Value);

        public static bool operator ==(ItemCount left, ItemCount right) => left.Equals(right);
        public static bool operator !=(ItemCount left, ItemCount right) => !left.Equals(right);
    }
}
```

種類は、数が少なく固定なら `enum ItemType`、SOを足すだけで増やしたいなら `ItemId`（stringやintを包むVO）+ 10章のカタログ方式にします。

### Entity

EntityはIDで同一性を持ち、VOを差し替えて状態遷移します。値は普通のプロパティで持ち、R3には依存しません。変更の通知は、そのEntityを参照するModelの役割です（6章）。

### 実行時に複数生成するもの

棚・敵・NPCのように同じ種類が実行時に複数存在するものは、個々をDI登録しません。**一覧そのものを1つのEntityにして**、上位スコープにSingletonで登録します。Repository層は作りません。

```csharp
// Entity：普通のコレクションで値を持つ
public sealed class ShelvesEntity
{
    public IReadOnlyDictionary<ShelfId, ItemCount> Stocks => _stocks;

    private readonly Dictionary<ShelfId, ItemCount> _stocks = new();
}

// Model：Entityの値を参照して通知する
// R3本体にはReactiveDictionaryが無いため、ObservableCollections（+ ObservableCollections.R3）を使う
public sealed class ShelvesModel
{
    public IReadOnlyObservableDictionary<ShelfId, ItemCount> Stocks => _stocks;

    private readonly ObservableDictionary<ShelfId, ItemCount> _stocks = new();
}
```

ViewはFactory（`IObjectResolver.Instantiate` 等）で生成し、自分のIDを受け取って一覧Modelの対応する要素を購読します。

### セーブ

DTOは挟まず、VO・Entityの状態を直接シリアライズします。

- Unity標準の `JsonUtility` はプロパティや読み取り専用の型を扱えないため使わない。コンストラクタ経由で復元できるシリアライザ（Newtonsoft.Json、MemoryPack等）を使う。
- VOの形を変えると古いセーブが読めなくなる。公開後に形を変える場合は、バージョン番号を持たせて移行処理を書く。

## 8. 毎フレーム処理と演出の分離

入力起点ではない処理（敵やNPCのAI、生産タイマーなど）は、`Controller → Usecase` の流れに無理に乗せません。

- VContainerの `ITickable` を実装したピュアC#のクラスが、対象全体のロジックをまとめて回す。Viewは移動やアニメーションだけを担当する。
- ビヘイビアツリーやステートマシンでAIを組む場合も、この位置に置く。

Tweenなどで遅れて反映される演出は、**論理の状態と見た目の状態を分けます**。Entityは論理値を即座に更新し、Viewは「表示中の値」を別に持って、演出の完了時に追いつかせます。両者を同じものとして扱うと、「数値は変わったのに見た目がまだ」「演出中のものを別の処理が掴む」といったズレが起きます。

## 9. モジュール跨ぎの連携

機能モジュールは直接参照できないので、Commonの契約と共有Entity / Usecaseで繋ぎます。Pub/Subのイベント配信ライブラリ（MessagePipe等）は使いません。

### Common契約 + 共有Entity / Usecase

- 共有データは上位スコープ（Root / InGame）にEntityとそのModelを1つずつ登録し、関係するモジュールが同じEntityをDIで受け取る。例：`<Shared>Entity` を機能Aが書き込み、機能BのPresenterが対応するModelを購読して表示する。
- 操作を渡したい場合は、CommonにUsecaseのinterfaceを定義する。所有モジュールが実装・登録し、呼ぶ側はinterfaceをDIで受けて同期呼び出しする。例：`I<Shared>Usecase { void Apply(<VO> value); }` を機能Bが実装し、機能Aが注入して呼ぶ。両者はCommonしか知らない。
- 同期的で明示的なので、呼び出し元と処理の流れを追いやすい。

### 使い分け

| 連携の種類 | 手段 |
| --- | --- |
| モジュール内の状態ストリーム | R3 |
| 跨ぎの操作・共有データ | Commonの共有Entity / Usecase |

## 10. SerializeFieldにinterfaceが入らない制約への対処

`[SerializeField]` にはinterfaceを指定できないので、カタログ経由で具象参照を消します。

1. Commonに `I<X>Parameter` とカタログ `I<X>Catalog` を定義する。
2. ScriptableObject側がそれを実装する。
3. 利用側はDIでカタログを受け取り、名前やIDでカタログを引く。

InstallerをSerializeFieldのリストで持つときも、interfaceではなく具象基底 `InstallerObject` のリストで受けます。

## 11. 命名・コード規約

プロジェクトに合わせて調整してください。

- 名前空間はモジュール名（rootNamespace）に合わせる。
- 型の「種類」を表す名前は `Type` を使う（例：`ItemType`）。プロパティ名は `Type` 単体にせず `ItemType` のようにフルで書く（`System.Type` / `GetType()` との混同を避けるため）。
- メンバー順序（上から）：const → コンストラクタ → public変数 → public event → プロパティ → protected変数 → private変数 → private readonly変数 → publicメソッド → protectedメソッド → privateメソッド。フィールドはメソッドの前。修飾子や形態が変わる境界は1行空ける。
- サマリはpublicのみ（private / protectedは `//` コメント）。`///` の直後に空白を入れない。プロパティは1行、メソッドは3行形式。
- コメントは「なぜ」を書く。自明なコメントは書かない。

## 12. 設計判断の指針（過剰設計しない）

- 子スコープが親スコープの登録を `Resolve` するのは、跨ぎ配線の範囲で許容する。
- 理想は1モジュール1関心だが、広めのモジュールは必要になった時点で分割すればよい。最初から完璧を目指さない。
- どうしても守れない規約違反は、理由をコメントに残したうえで許容する。
- 迷ったら：動詞（ユースケース）で機能モジュールを切り、使う「能力」を名詞で切り出し、契約をCommonに置く。asmdefとInstallerは細かく、SceneとScopeは文脈単位で粗く。
