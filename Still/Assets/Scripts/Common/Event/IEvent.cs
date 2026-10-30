namespace Still.Common
{
    ///<summary>
    ///カメラ演出などの一回きりのイベント。SerializeReferenceで差し替える
    ///</summary>
    public interface IEvent
    {
        ///<summary>演出が終わったか</summary>
        bool IsFinished { get; }

        ///<summary>
        ///演出を開始する
        ///</summary>
        void Play();
    }
}
