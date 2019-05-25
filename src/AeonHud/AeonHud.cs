using BepInEx;
using BepInEx.Logging;

namespace AeonHud
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.aeonlucid.aeonhud", "AeonHud", "1.0")]
    public class AeonHud : BaseUnityPlugin
    {
        private static ManualLogSource LoggerStatic;
        private PlayerManager _playerManager;
        
        public AeonHud()
        {
            LoggerStatic = Logger;
            
            _playerManager = new PlayerManager(Logger);
        }

        public void Awake()
        {
            Logger.LogInfo("Initializing AeonHud.");
        }

        public void Update()
        {
            // Logger.LogInfo("Update.");
        }
    }
}