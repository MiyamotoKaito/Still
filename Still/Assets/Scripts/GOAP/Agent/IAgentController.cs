using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///アクションから操作するゴーストの体
    ///</summary>
    public interface IAgentController
    {
        ///<summary>ゴーストの設定</summary>
        GhostConfig Config { get; }
        ///<summary>ゴーストが鳴らす音のAudioSource</summary>
        AudioSource AudioSource { get; }
        ///<summary>現在のターゲット（無ければnull）</summary>
        Transform CurrentTarget { get; }
        ///<summary>現在位置</summary>
        Vector3 Position { get; }
        ///<summary>ターゲットに十分近づいたか</summary>
        bool IsArrived { get; }

        ///<summary>
        ///ターゲットを設定する
        ///</summary>
        void SetTarget(Transform target);

        ///<summary>
        ///移動速度を設定する
        ///</summary>
        void SetSpeed(float speed);

        ///<summary>
        ///NavMesh上のランダムな地点を取得する
        ///</summary>
        Vector3 GetRandomPos();

        ///<summary>
        ///目的地へ移動を始める
        ///</summary>
        void SetMoveDestination(Vector3 destination);

        ///<summary>
        ///瞬間移動する
        ///</summary>
        void Teleport(Vector3 position);

        ///<summary>
        ///その場で止まる
        ///</summary>
        void StopMove();

        ///<summary>
        ///指定位置の方へ水平に向き直る（移動を再開するまで自動の向き変更を止める）
        ///</summary>
        void LookAt(Vector3 position, float degreesPerSecond);
    }
}
