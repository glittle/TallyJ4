namespace TallyJ4.Code.Hubs
{
    public interface IStatusUpdateHub
    {
        void StatusUpdate(string msg, bool msgIsTemp = false);
    }
}