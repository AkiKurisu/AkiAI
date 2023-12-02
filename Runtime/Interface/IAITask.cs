namespace Kurisu.AkiAI
{
    public interface IAITask
    {
        string TaskID { get; }
        void Init(IAIHost host);
        void Tick();
        bool IsPersistent { get; }
        bool Enabled { get; }
        void Stop();
        void Start();
    }
}
