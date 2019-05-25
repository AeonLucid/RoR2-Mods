using RoR2;

namespace AeonHud
{
    public class PlayerData
    {
        public PlayerData(NetworkUserId userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
        
        public NetworkUserId UserId { get; }
        public string UserName { get; }
    }
}