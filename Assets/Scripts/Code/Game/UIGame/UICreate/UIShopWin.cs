using System;

namespace TaoTie
{
    public class UIShopWin: UIBaseContainer,IOnCreate<Action<int,int>>,IOnEnable,IOnDisable
    {
        [Timer(TimerType.UIShopWin)]
        public class UIShopWinUpdateTimer : ATimer<UIShopWin>
        {
        	public override void Run(UIShopWin self)
        	{
        		try
        		{
        			self.UpdateTimeDown();
        		}
        		catch (Exception e)
        		{
        			Log.Error($"move timer error: UIShopWin\n{e}");
        		}
        	}
        }

        public UIButton RefreshBtn;
        public UITextmesh RefreshText;
        public ShopItem[] ShopItems;
        public GroupInfoTable GroupInfoTable;
        
        private long timerId;
        private Action<int,int> clickItem;
        public void OnCreate(Action<int,int > clickItem)
        {
            this.clickItem = clickItem;
            GroupInfoTable = AddComponent<GroupInfoTable>("Top/Tip");
            RefreshBtn = AddComponent<UIButton>("Top/RefreshBtn");
            RefreshText= AddComponent<UITextmesh>("Top/RefreshTip/Text");
            ShopItems = new ShopItem[PlayerDataManager.Instance.ClothRefreshCount];
            for (int i = 0; i < ShopItems.Length; i++)
            {
                ShopItems[i] = AddComponent<ShopItem>("Content/ShopItem" + i);
            }
            RefreshText.SetI18NKey(I18NKey.Text_Reresh_TimeDown);
        }
        
        public void OnEnable()
        {
            RefreshBtn.SetActive(AdManager.Instance.PlatformHasAD());
            RefreshBtn.SetOnClick(RefreshCloth);
            RefreshList().Coroutine();
            UpdateTimeDown();
            TimerManager.Instance.Remove(ref timerId);
            timerId = TimerManager.Instance.NewRepeatedTimer(1000, TimerType.UIShopWin, this);
        }

        public void OnDisable()
        {
            TimerManager.Instance.Remove(ref timerId);
        }


        private async ETTask RefreshList()
        {
            var list = PlayerDataManager.Instance.GetClothsSale();
            for (int i = 0; i < ShopItems.Length; i++)
            {
                if (i < list.Count) ShopItems[i].SetData(list[i], clickItem);
                ShopItems[i].SetActive(false);
                if (i < list.Count)
                {
                    ShopItems[i].SetActive(true);
                }
                await TimerManager.Instance.WaitAsync(50);
            }
        }

        private void RefreshCloth()
        {
            if(AdManager.Instance.PlatformHasAD()) PlayAdAsync().Coroutine();
        }
        
        public async ETTask PlayAdAsync()
        {
            RefreshBtn.SetInteractable(false);
            try
            {
                var res = await AdManager.Instance.PlayAd();
                if (res)
                {
                    PlayerDataManager.Instance.RefreshCloth(false);
                    RefreshList().Coroutine();
                    SoundManager.Instance.PlaySound("Audio/Sound/Common_Refresh.mp3");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                RefreshBtn.SetInteractable(true);
            }
        }
        
        public void UpdateTimeDown()
        {
            var time = PlayerDataManager.Instance.LastRefreshDailyTime + TimeInfo.OneDay;
            var deltaTime = time - TimerManager.Instance.GetTimeNow();
            var temp = TimeInfo.Instance.ToDateTime(TimeInfo.Instance.Transition(DateTime.Today) + deltaTime);
            if (temp.Hour > 0)
            {
                RefreshText.SetI18NText(temp.ToString("HH:mm"));
            }
            else
            {
                RefreshText.SetI18NText(temp.ToString("mm:ss"));
            }
        }
    }
}