using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace TaoTie
{
	public class UICashGroup : UIBaseContainer, IOnCreate, IOnEnable, IOnDisable, IOnWidthPaddingChange
	{
		public UITextmesh TextCash;
		public UIPointerClick FrameCash;
		public UIMonoBehaviour<ContentSizeFitter> Details;
		public UIImage IconMoney;
		public UITextmesh DetailsNum;
		public UIAnimator MoneyShow;

		public BigNumber ShowNum { get; private set; }

		public void OnCreate()
		{
			MoneyShow = AddComponent<UIAnimator>("IconCash/LargeMoney");
			IconMoney = AddComponent<UIImage>("IconCash/IconMoney");
			this.TextCash = this.AddComponent<UITextmesh>("TextCash");
			Details = AddComponent<UIMonoBehaviour<ContentSizeFitter>>("FrameCash/Details");
			FrameCash = AddComponent<UIPointerClick>("FrameCash");
			DetailsNum = AddComponent<UITextmesh>("FrameCash/Details/DetailsNum");
			FrameCash.SetOnClick(OnClickShowDetails);
		}

		public void OnEnable()
		{
			MoneyShow.SetActive(false);
			Details.SetActive(false);
			FrameCash.SetOnClick(OnClickShowDetails);
			Messager.Instance.AddListener<BigNumber>(0, MessageId.ChangeMoney, RefreshMoney);
			RefreshMoney(PlayerManager.Instance.Uid == 0 ? 0 : PlayerDataManager.Instance.TotalMoney, false);
		}

		public void OnDisable()
		{
			Messager.Instance.RemoveListener<BigNumber>(0, MessageId.ChangeMoney, RefreshMoney);
		}

		public void SetShowNum(BigNumber num)
		{
			RefreshMoney(num, false);
		}
		

		private void OnClickShowDetails()
		{
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Click.mp3");
			Details.SetActive(!Details.GetGameObject().activeSelf);
		}

		/// <summary>
		/// 金币动画
		/// </summary>
		/// <param name="add"></param>
		/// <param name="startPos"></param>
		/// <param name="total"></param>
		public async ETTask DoMoneyMoveAnim3D(BigNumber[] add, Vector3[] startPos, int total)
		{
			if (total > GameConst.MaxBoxCount) total = GameConst.MaxBoxCount;
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				var money = PlayerDataManager.Instance.TotalMoney;
				using ListComponent<GameObject> moveMoney = ListComponent<GameObject>.Create(); 
				for (int i = 0; i < total; i++)
				{
					var item = GameObject.Instantiate(IconMoney.GetGameObject());
					item.SetActive(true);
					item.transform.SetParent(IconMoney.GetTransform().parent);
					item.transform.localScale = Vector3.one;
					moveMoney.Add(item);
				}
		
				using ListComponent<Vector2> startPoses = ListComponent<Vector2>.Create();
				for (int i = 0; i < total; i++)
				{
					Vector2 pt = UIManager.Instance.ScreenPointToUILocalPoint(IconMoney.GetRectTransform(),
						mainCamera.WorldToScreenPoint(startPos[i]));
					startPoses.Add(pt);
					moveMoney[i].GetComponent<RectTransform>().anchoredPosition = pt;
					moveMoney[i].SetActive(add[i] > BigNumber.Zero);
				}

				long timeStart = TimerManager.Instance.GetTimeNow();
				int interval = 50;
				float moveTime = 500f;
				float endTime = moveTime + interval * total;
				bool changeMoney = false;
				SoundManager.Instance.PlaySound("Audio/Sound/Common_Money.mp3");
				while (true)
				{
					await TimerManager.Instance.WaitAsync(1);
					var timeNow = TimerManager.Instance.GetTimeNow();
					for (int i = 0; i < total; i++)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPoses[i], Vector2.zero,
							Mathf.Clamp01((timeNow - timeStart - interval * i) / moveTime));
					}

					if (!changeMoney && timeNow - timeStart > moveTime)
					{
						for (int i = 0; i < total - 1; i++)
						{
							money += add[i];
							TextCash.DoNum(money).Coroutine();
						}
						
						changeMoney = true;
					}

					if (timeNow - timeStart > endTime)
					{
						break;
					}
				}

				for (int i = 0; i < moveMoney.Count; i++)
				{
					GameObject.Destroy(moveMoney[i]);
				}
			}
		}

		public async ETTask DoMoneyMoveAnim(BigNumber[] add, Vector3[] startPos, int total)
		{
			if (total > GameConst.MaxBoxCount) total = GameConst.MaxBoxCount;
			var mainCamera = CameraManager.Instance.MainCamera();
			if (mainCamera != null)
			{
				var money = PlayerDataManager.Instance.TotalMoney;
				using ListComponent<GameObject> moveMoney = ListComponent<GameObject>.Create(); 
				for (int i = 0; i < total; i++)
				{
					var item = GameObject.Instantiate(IconMoney.GetGameObject());
					item.SetActive(true);
					item.transform.SetParent(IconMoney.GetTransform().parent);
					item.transform.localScale = Vector3.one;
					moveMoney.Add(item);
				}
				using ListComponent<Vector2> startPoses = ListComponent<Vector2>.Create();
				for (int i = 0; i < total; i++)
				{
					moveMoney[i].GetComponent<RectTransform>().position = startPos[i];
					startPoses.Add(moveMoney[i].GetComponent<RectTransform>().anchoredPosition);
					moveMoney[i].SetActive(add[i] > BigNumber.Zero);
				}

				long timeStart = TimerManager.Instance.GetTimeNow();
				int interval = 50;
				float moveTime = 500f;
				float endTime = moveTime + interval * total;
				bool changeMoney = false;
				SoundManager.Instance.PlaySound("Audio/Sound/Common_Money.mp3");
				while (true)
				{
					await TimerManager.Instance.WaitAsync(1);
					var timeNow = TimerManager.Instance.GetTimeNow();
					for (int i = 0; i < total; i++)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPoses[i], Vector2.zero,
							Mathf.Clamp01((timeNow - timeStart - interval * i) / moveTime));
					}

					if (!changeMoney && timeNow - timeStart >= moveTime)
					{
						for (int i = 0; i < total - 1; i++)
						{
							money += add[i];
							TextCash.DoNum(money).Coroutine();
						}

						MoneyShow.SetActive(true);
						changeMoney = true;
					}

					if (timeNow - timeStart > endTime)
					{
						break;
					}
				}
				MoneyShow.SetActive(false);
				for (int i = 0; i < moveMoney.Count; i++)
				{
					GameObject.Destroy(moveMoney[i]);
				}
			}
		}

		public async ETTask DoMoneyMoveAnim(BigNumber add, Vector3 startPos, int total, float size = 1)
		{
			if (total > GameConst.MaxBoxCount) total = GameConst.MaxBoxCount;
			if (add <= BigNumber.Zero) return;
			if (total <= 1)
			{
				await DoMoneyMoveAnim(add, startPos);
				return;
			}

			var money = PlayerDataManager.Instance.TotalMoney;
			using ListComponent<GameObject> moveMoney = ListComponent<GameObject>.Create(); 
			for (int i = 0; i < total; i++)
			{
				var item = GameObject.Instantiate(IconMoney.GetGameObject());
				item.SetActive(true);
				item.transform.SetParent(IconMoney.GetTransform().parent);
				item.transform.localScale = Vector3.one;
				moveMoney.Add(item);
			}
			Vector2 pt = Vector2.zero;
			var rand = Random.Range(0, 360);
			var delta = 360 / total;
			using ListComponent<Vector2> startPoses = ListComponent<Vector2>.Create();
			for (int i = 0; i < total; i++)
			{
				moveMoney[i].GetComponent<RectTransform>().position = startPos;
				pt = moveMoney[i].GetComponent<RectTransform>().anchoredPosition;
				Vector3 pos = Quaternion.Euler(0, 0, rand + i * delta) * Vector3.up * Random.Range(50, 100 * size);
				startPoses.Add(pt + (Vector2) pos);
			}

			startPoses.RandomSort();

			//移动
			long timeStart = TimerManager.Instance.GetTimeNow();
			int interval = 50;
			float moveTime = 1000f;
			float endTime = moveTime + interval * total;
			bool changeMoney = false;
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Money.mp3");
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				for (int i = 0; i < total; i++)
				{
					var step = Mathf.Clamp01((timeNow - timeStart - interval * i) / moveTime);
					if (step < 0.1f)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pt, startPoses[i], step * 10f);
					}
					else if (step > 0.5f)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition =
							Vector2.Lerp(startPoses[i], Vector2.zero, (step - 0.5f) / 0.5f);
					}
					
				}
				
				if (!changeMoney && timeNow - timeStart >= moveTime)
				{
					money += add;
					TextCash.DoNum(money).Coroutine();
					MoneyShow.SetActive(true);
					changeMoney = true;
				}

				if (timeNow - timeStart > endTime)
				{
					break;
				}
			}
			MoneyShow.SetActive(false);
			for (int i = 0; i < moveMoney.Count; i++)
			{
				GameObject.Destroy(moveMoney[i]);
			}

		}

		public async ETTask DoMoneyMoveAnim(BigNumber add, Vector3 startPos)
		{
			if (add <= BigNumber.Zero) return;

			var money = PlayerDataManager.Instance.TotalMoney;
			var item = GameObject.Instantiate(IconMoney.GetGameObject());
			item.SetActive(true);
			item.transform.SetParent(IconMoney.GetTransform().parent);
			item.transform.localScale = Vector3.one;
			Vector2 pt = Vector2.zero;
			item.GetComponent<RectTransform>().position = startPos;
			pt = item.GetComponent<RectTransform>().anchoredPosition;

			long timeStart = TimerManager.Instance.GetTimeNow();
			int interval = 50;
			float moveTime = 500f;
			float endTime = moveTime + interval;
			bool changeMoney = false;
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Money.mp3");
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				item.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pt, Vector2.zero,
					Mathf.Clamp01((timeNow - timeStart) / moveTime));
				if (!changeMoney && timeNow - timeStart > moveTime)
				{
					money += add;
					TextCash.DoNum(money).Coroutine();
					changeMoney = true;
					MoneyShow.SetActive(true);
				}

				if (timeNow - timeStart >= endTime)
				{
					break;
				}
			}
			MoneyShow.SetActive(false);
			GameObject.Destroy(item);
		}

		public async ETTask DoMoneyMoveAnim(BigNumber add, Vector3 startPos, Vector2 size, int total)
		{
			if (total > GameConst.MaxBoxCount) total = GameConst.MaxBoxCount;
			if (add <= BigNumber.Zero) return;
			if (total <= 1)
			{
				await DoMoneyMoveAnim(add, startPos);
				return;
			}

			var money = PlayerDataManager.Instance.TotalMoney;
			using ListComponent<GameObject> moveMoney = ListComponent<GameObject>.Create(); 
			for (int i = 0; i < total; i++)
			{
				var item = GameObject.Instantiate(IconMoney.GetGameObject());
				item.SetActive(true);
				item.transform.SetParent(IconMoney.GetTransform().parent);
				item.transform.localScale = Vector3.one;
				moveMoney.Add(item);
			}
			Vector2 pt = Vector2.zero;
			using ListComponent<Vector2> startPoses = ListComponent<Vector2>.Create();
			for (int i = 0; i < total; i++)
			{
				moveMoney[i].GetComponent<RectTransform>().position = startPos;
				pt = moveMoney[i].GetComponent<RectTransform>().anchoredPosition;
				startPoses.Add(pt + new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y)) / 2);
			}

			//移动
			long timeStart = TimerManager.Instance.GetTimeNow();
			int interval = 50;
			float moveTime = 1000f;
			float endTime = moveTime + interval * total;
			bool changeMoney = false;
			SoundManager.Instance.PlaySound("Audio/Sound/Common_Money.mp3");
			while (true)
			{
				await TimerManager.Instance.WaitAsync(1);
				var timeNow = TimerManager.Instance.GetTimeNow();
				for (int i = 0; i < total; i++)
				{
					var step = Mathf.Clamp01((timeNow - timeStart - interval * i) / moveTime);
					if (step < 0.2f)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pt, startPoses[i], step * 5f);
					}
					else if (step > 0.4f)
					{
						moveMoney[i].GetComponent<RectTransform>().anchoredPosition =
							Vector2.Lerp(startPoses[i], Vector2.zero, (step - 0.5f) / 0.5f);
					}
				}
				if (!changeMoney && timeNow - timeStart >= moveTime)
				{
					money += add;
					TextCash.DoNum(money).Coroutine();
					changeMoney = true;
					MoneyShow.SetActive(true);
				}

				if (timeNow - timeStart > endTime)
				{
					break;
				}
			}
			MoneyShow.SetActive(false);
			for (int i = 0; i < moveMoney.Count; i++)
			{
				GameObject.Destroy(moveMoney[i]);
			}
		}

		public void RefreshMoney(BigNumber money)
		{
			RefreshMoney(money, true);
		}

		public void RefreshMoney(BigNumber money, bool anim)
		{
			if (anim)
			{
				TextCash.DoNum(money).Coroutine();
			}
			else
			{
				TextCash.SetNum(money);
			}

			var text = money.ToString(2);
			if (text.Length > 35)
			{
				Details.GetComponent().enabled = false;
				Details.GetRectTransform().sizeDelta = new Vector2(730, 54);
			}
			else
			{
				Details.GetComponent().enabled = true;
			}
			DetailsNum.SetText(text);
			ShowNum = money;
		}
	}
}