
namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public float money;
        public float dmg;
        public int isBonusActive;
        public float passiveDmg;
        public float passiveCooldown;
        public int objectIndex;
        public int curBackgroundIndex;
        public bool isGameWasCompleted;
        public int adWatchEndBonus;

        // Вы можете выполнить какие то действия при загрузке сохранений
        public SavesYG()
        {
            if (dmg==0)
            {
                dmg = 1;
            }
            if (passiveDmg == 0)
            {
                passiveDmg = 1;
            }
            if (passiveCooldown == 0)
            {
                passiveCooldown = 5;
            }

        }
    }
}
