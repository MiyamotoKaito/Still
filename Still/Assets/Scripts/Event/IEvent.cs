public interface IEvent 
{
    public void Initialize();
    public void OnEvent();
    public bool IsFinished { get; }
}
