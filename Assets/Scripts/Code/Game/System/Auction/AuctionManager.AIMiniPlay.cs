using UnityEngine;

namespace TaoTie
{
    public partial class AuctionManager
    {
	    /// <summary>
        /// AI小玩法
        /// </summary>
        private async ETTask AIRandomMiniPlay()
        {
	        if(LastAuctionPlayerId <= 0) return;
	        if (Random.Range(0, 100) > LevelConfig.AIMiniPlayPercent) return;
	        await TimerManager.Instance.WaitAsync(1000);
	        for (int i = 0; i < Boxes.Count; i++)
	        {
		        var box = EntityManager.Instance.Get<Box>(Boxes[i]);
		        switch (box.ItemConfig.Type)
		        {
			        case (int) ItemType.Appraisal:
				        if (box.ItemResultId == 0 &&
				            SubIdentificationConfigCategory.Instance.TryGet(box.ItemId, out var sConfig))
				        {
					        var rand = Random.Range(0, sConfig.TotalAIWidget * 10) % sConfig.TotalAIWidget;
					        for (int j = 0; j < sConfig.AIWidget.Length; j++)
					        {
						        rand -= sConfig.AIWidget[j];
						        if (rand <= 0)
						        {
							        box.SetAppraisalResult(sConfig.Result[j]);
							        Report.PlayData[i] = box.ItemResult;
							        RefreshPrice();
							        Messager.Instance.Broadcast(0, MessageId.SetChangeItemResult, box.ItemId,
								        box.ItemResultId, true);
							        break;
						        }
					        }
				        }

				        break;
			        case (int) ItemType.Quarantine:
				        var qConfig = QuarantineConfigCategory.Instance.Get(box.ItemId);
				        bool isSuccess = Random.Range(0, 100) < qConfig.Percent;
				        var price = IAuctionManager.Instance.AllPrice;
				        if (isSuccess)
				        {
					        price = Random.Range(qConfig.SuccessMin, qConfig.SuccessMax + 1) / 100f * price;
				        }
				        else
				        {
					        price = Random.Range(qConfig.FailMin, qConfig.FailMax + 1) / 100f * price;
				        }
				        BigNumber.Round2Integer(price);
				        var newPrice = price - IAuctionManager.Instance.AllPrice;
				        box.SetMiniGameResult(newPrice);
				        RefreshPrice();
				        Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, box.ItemId, newPrice, true);
				        break;
			        case (int) ItemType.Repair:
				        var rConfig = RepairConfigCategory.Instance.Get(box.ItemId);
				        isSuccess = Random.Range(0, 2) < 1;
				        price = IAuctionManager.Instance.AllPrice;
				        if (isSuccess)
				        {
					        price = Random.Range(rConfig.SuccessMin, rConfig.SuccessMax + 1) / 100f * price;
				        }
				        else
				        {
					        price = Random.Range(rConfig.FailMin, rConfig.FailMax + 1) / 100f * price;
				        }
				        BigNumber.Round2Integer(price);
				        box.SetMiniGameResult(price);
				        RefreshPrice();
				        Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, box.ItemId, price, true);
				        break;
			        case (int) ItemType.GoodsCheck:
				        var gConfig = GoodsCheckConfigCategory.Instance.Get(box.ItemId);
				        isSuccess = Random.Range(0, 2) < 1;
				        price = IAuctionManager.Instance.AllPrice;
				        if (isSuccess)
				        {
					        price = Random.Range(gConfig.SuccessMin, gConfig.SuccessMax + 1) / 100f * price;
				        }
				        else
				        {
					        price = Random.Range(gConfig.FailMin, gConfig.FailMax + 1) / 100f * price;
				        }
				        BigNumber.Round2Integer(price);
				        box.SetMiniGameResult(price);
				        RefreshPrice();
				        Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, box.ItemId, price, true);
				        break;
			        case (int) ItemType.BombDisposal:
				        var weight = BombDisposalConfigCategory.Instance.TotalWeight;
				        var target = Random.Range(0, weight);
				        var list = BombDisposalConfigCategory.Instance.GetAllList();
				        int diffId = 0;
				        for (int j = 0; j < list.Count; j++)
				        {
					        diffId = list[j].Id;
					        target -= list[j].Weight;
					        if (target <= 0)
					        {
						        break;
					        }
				        }
				        var bConfig = BombDisposalConfigCategory.Instance.Get(diffId);
				        
				        price = IAuctionManager.Instance.AllPrice;
				        price = Random.Range(bConfig.FailMin, bConfig.SuccessMax + 1) / 100f * price;
				        BigNumber.Round2Integer(price);
				        box.SetMiniGameResult(price);
				        RefreshPrice();
				        Messager.Instance.Broadcast(0, MessageId.SetChangePriceResult, box.ItemId, price, true);
				        break;
			        case (int) ItemType.Const:
			        case (int) ItemType.Story:
			        case (int) ItemType.None:
			        case (int) ItemType.Container:
			        case (int) ItemType.AppraisalResult:
				        break;
			        default:
				        Log.Error("AI玩小游戏未处理" + box.ItemConfig.Type);
				        break;
		        }
	        }
        }
    }
}