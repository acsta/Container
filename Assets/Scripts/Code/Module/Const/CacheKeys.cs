namespace TaoTie
{
    public class CacheKeys
    {
        public const string Account = "Account";
        public const string LastToken = "LastToken";
        public const string Password = "Password";
        public const string CurLangType = "CurLangType";
        public const string KeyCodeSetting = "KeyCodeSetting";
        public const string MusicVolume = "MusicVolume";
        public const string SoundVolume = "SoundVolume";
        public const string ShockCycle = "ShockCycle";
        public const string ColliderDebug = "ColliderDebug";
        public const string TriggerDebug = "TriggerDebug";
        public const string Guidance = "Guidance";
        public const string CheckAppUpdate = "CheckAppUpdate";
        public static string PlayerData => "PlayerData" + PlayerManager.Instance.Uid;
        public static string DiceSetting => "DiceSetting" + PlayerManager.Instance.Uid;
    }
}