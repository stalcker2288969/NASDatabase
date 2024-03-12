namespace NASDatabase.Interfaces
{
    public abstract class CommandHandler
    {
        public abstract string Use();
        public virtual void SetData(string Data) { }
    }
}