namespace TallyJ3.Code.Hubs
{
    public interface IStatusUpdateHub
    {
        void StatusUpdate(string msg, bool msgIsTemp = false);
    }
}