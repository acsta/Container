namespace TaoTie
{
    public class DailyTaskRewards: UIBaseContainer,IOnCreate
    {
        private UIImage Icon;
        private UITextmesh Rewards;
        public UIButton Flag;
        public UITextmesh Count;
        public UIImage Check;
        private int overTaskCount;
        public DailyTaskRewardsConfig Config { get; private set; }
        private int curIndex;
        public void OnCreate()
        {
            Flag = AddComponent<UIButton>("Flag");
            Icon = AddComponent<UIImage>("Flag/Icon");
            Rewards = AddComponent<UITextmesh>("Flag/Text");
            Count = AddComponent<UITextmesh>("Flag/Count");
            Check = AddComponent<UIImage>("Flag/Check");
            Flag.SetOnClick(OnClickFlag);
        }

        public void SetData(int index, DailyTaskRewardsConfig config, int overTaskCount)
        {
            curIndex = index;
            Config = config;
            Count.SetText(config.TaskCount.ToString());
            this.overTaskCount = overTaskCount;
            Icon.SetSpritePath(Config.Icon).Coroutine();
            
            bool isGray = overTaskCount < Config.TaskCount;
            Flag.SetBtnGray(isGray, false, false).Coroutine();
            bool isGet = PlayerDataManager.Instance.IsGetDailyRewards(index);
            Count.SetActive(!isGet);
            Count.SetTextColor(isGray ? GameConst.WHITE_COLOR : "#AF5B08");
            Check.SetActive(isGet);
            if (isGet)
            {
                Rewards.SetI18NKey(I18NKey.Text_Rewards_Got);
            }
            else
            {
                Rewards.SetNum(config.RewardCount);
            }
        }

        public void OnClickFlag()
        {
            if (overTaskCount < Config.TaskCount)
            {
                UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Recieve_NotOpen).Coroutine();
                return;
            }
            if (PlayerDataManager.Instance.IsGetDailyRewards(curIndex))
            {
                UIManager.Instance.OpenBox<UIToast, I18NKey>(UIToast.PrefabPath, I18NKey.Tips_Recieve_Twice).Coroutine();
                return;
            }
            Flag.SetInteractable(false);
            OnClickFlagAsync().Coroutine();
        }

        private async ETTask OnClickFlagAsync()
        {
            Check.SetActive(true);
            Count.SetActive(false);
            Rewards.SetI18NKey(I18NKey.Text_Rewards_Got);
            
            PlayTaskCompleteFX().Coroutine();
            
            var top = UIManager.Instance.GetView<UITopView>(1);
            if (top != null)
            {
                await top.Top.DoMoneyMoveAnim(Config.RewardCount, Flag.GetTransform().position, (curIndex + 1) * 3);
            }
            bool res = PlayerDataManager.Instance.GetDailyRewards(curIndex);
            if (res)
            {
                var win = UIManager.Instance.GetView<UIMarketView>(1);
                win?.RefreshTask();
            }
            else
            {
                Messager.Instance.Broadcast(0, MessageId.ChangeMoney);
            }
            Flag.SetInteractable(true);
        }

        private async ETTask PlayTaskCompleteFX()
        {
            var taskFXGO = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(GameConst.TaskPrefab);
            taskFXGO.SetActive(false);
            taskFXGO.transform.position = Check.GetRectTransform().position;
            taskFXGO.SetActive(true);
            await TimerManager.Instance.WaitAsync(500);
            GameObjectPoolManager.GetInstance().RecycleGameObject(taskFXGO);
        }

    }
}