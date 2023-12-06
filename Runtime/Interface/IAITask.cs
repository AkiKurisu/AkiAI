namespace Kurisu.AkiAI
{
    public enum TaskStatus
    {
        Disabled, Enabled, Pending
    }
    public interface IAITask
    {
        string TaskID { get; }
        void Init(IAIHost host);
        void Tick();
        bool IsPersistent { get; }
        TaskStatus Status { get; }
        void Stop();
        void Start();
        void Pause();
    }
}
