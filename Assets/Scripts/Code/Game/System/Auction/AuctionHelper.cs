using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaoTie
{
    public static class AuctionHelper
    {
        private static float[] raiseMul;
        static AuctionHelper()
        {
            if (!GlobalConfigCategory.Instance.TryGetArray("RaiseCoefficient", out raiseMul))
            {
                raiseMul = new float[1] {1};
            }
        }
        /// <summary>
        /// 获取抬价收益倍率
        /// </summary>
        /// <returns></returns>
        public static float GetRaiseMul(int successCount)
        {
            successCount -= 1;
            if (successCount < 0) return 1;
            if (successCount >= raiseMul.Length)
            {
                return raiseMul[raiseMul.Length - 1];
            }

            return raiseMul[successCount];
        }
        
        /// <summary>
        /// 随机特殊集装箱数量
        /// </summary>
        /// <param name="counts"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int RandomSpecialCount(int[] counts,int[] weights)
        {
            if (counts == null || weights == null) return 0;
#if UNITY_EDITOR
            if (counts.Length != weights.Length)
            {
                Log.Error("当前难度特殊集装箱随机数量和权重不对应！");
            }
#endif
            if (counts.Length < 1) return 0;
            if (counts.Length == 1) return counts[0];
            int total = 0;
            for (int i = 0; i < Mathf.Min(counts.Length, weights.Length); i++)
            {
                total += weights[i];
            }

            if (total == 0)
            {
                Log.Error("当前难度特殊集装箱随机权重和为0！");
                return 0;
            }
            var val = Random.Range(0, total * 10) % total;
            for (int i = 0; i < Mathf.Min(counts.Length, weights.Length); i++)
            {
                val -= weights[i];
                if (val <= 0)
                {
                    return Mathf.Max(0, counts[i]);
                }
            }

            return 0;
        }
        
        #region 装箱

        public static readonly Quaternion[] Rotations = new Quaternion[]
        {
	        Quaternion.identity,
	        Quaternion.Euler(new Vector3(0, 0, 90)),
	        Quaternion.Euler(new Vector3(0, 90, 0)),
	        Quaternion.Euler(new Vector3(90, 0, 0)),
        };
        
        public static readonly Vector3[] Flag = new Vector3[]
        {
	        new Vector3(0.5f, 0, -0.5f),
	        new Vector3(-1, 0.5f, -0.5f),
	        new Vector3(0.5f, 0, 0.5f),
	        new Vector3(0.5f, -0.5f, -1),
        };
		// 定义空间结构体
	    public struct Space
	    {
	        public Vector3 Position; // 空间左下角坐标
	        public Vector3 Size;     // 空间尺寸 (x=宽, y=高, z=深)

	        public Space(Vector3 position, Vector3 size)
	        {
	            Position = position;
	            Size = size;
	        }
	    }

	    public static bool PackBoxes(Vector3 containerSize, List<UnitConfig> boxSizes, out int[] rotations, out Vector3[] positions)
	    {
		    // 初始化结果数组（存储每个箱子的坐标）
	        positions = new Vector3[boxSizes.Count];
	        rotations = new int[boxSizes.Count];
	        
	        bool success = true;
	        // 按体积降序排序（大箱子优先）
	        int[] sortedIndices = boxSizes
	            .Select((box, index) => new { box, index })
	            .OrderByDescending(item => item.box.Size[0] * item.box.Size[1] * item.box.Size[2])
	            .Select(item => item.index)
	            .ToArray();

	        // 初始化剩余空间列表（从容器左下角开始）
	        using ListComponent<Space> remainingSpaces = ListComponent<Space>.Create();
	        remainingSpaces.Add(new Space(Vector3.zero, containerSize));

	        // 尝试放置每个箱子
	        foreach (int idx in sortedIndices)
	        {
		        bool placed = false;
		        // 尝试每个剩余空间
		        for (int i = 0; !placed && i < remainingSpaces.Count; i++)
		        {
			        Space space = remainingSpaces[i];
			        Vector3 size = new Vector3(boxSizes[idx].Size[0],boxSizes[idx].Size[1],boxSizes[idx].Size[2]);
			        for (int j = 0; j < boxSizes[idx].SupportRot.Length; j++)
			        {
				        var index = boxSizes[idx].SupportRot[j];
				        var boxSize = Rotations[index] * size;
				        boxSize.x = Mathf.Abs(boxSize.x);
				        boxSize.y = Mathf.Abs(boxSize.y);
				        boxSize.z = Mathf.Abs(boxSize.z);
				        // 检查箱子是否能以原始方向放入
				        if (boxSize.x <= space.Size.x &&
				            boxSize.y <= space.Size.y &&
				            boxSize.z <= space.Size.z)
				        {
					        // 记录箱子位置（当前空间左下角）
					        positions[idx] = space.Position;
					        rotations[idx] = index;
					        // 从当前空间分割新空间
					        Vector3 newSize1 = new Vector3(
						        space.Size.x - boxSize.x,
						        space.Size.y,
						        boxSize.z
					        );
					        Vector3 newPos1 = space.Position + new Vector3(boxSize.x, 0, 0);

					        Vector3 newSize2 = new Vector3(
						        boxSize.x,
						        space.Size.y - boxSize.y,
						        boxSize.z
					        );
					        Vector3 newPos2 = space.Position + new Vector3(0, boxSize.y, 0);

					        Vector3 newSize3 = new Vector3(
						        space.Size.x,
						        space.Size.y,
						        space.Size.z - boxSize.z
					        );
					        Vector3 newPos3 = space.Position + new Vector3(0, 0, boxSize.z);

					        // 移除当前空间
					        remainingSpaces.RemoveAt(i);

					        // 添加有效新空间（尺寸>0）
					        AddValidSpace(remainingSpaces, new Space(newPos1, newSize1));
					        AddValidSpace(remainingSpaces, new Space(newPos2, newSize2));
					        AddValidSpace(remainingSpaces, new Space(newPos3, newSize3));

					        placed = true;
					        break;
				        }
			        }
		        }

		        if (!placed)
	            {
		            success = false;
		            break; 
	            }
	        }
	        
	        return success;
	    }

	    // 添加有效空间到列表（过滤无效空间）
	    private static void AddValidSpace(List<Space> spaces, Space newSpace)
	    {
	        if (newSpace.Size.x > 0 && newSpace.Size.y > 0 && newSpace.Size.z > 0)
	        {
	            spaces.Add(newSpace);
	        }
	    }
	    
	    #endregion

	    public static int GetMaxCharacter()
	    {
		    switch (PerformanceManager.Instance.Level)
		    {
			    case PerformanceManager.DevicePerformanceLevel.High:
				    return 99;
			    case PerformanceManager.DevicePerformanceLevel.Mid:
				    return 7;
			    case PerformanceManager.DevicePerformanceLevel.Low:
				    return 5;
			    default:
				    return 5;
		    }
	    }
	    
	    public static async ETTask PlayFx(string path, Vector3 pos, int during = 2000)
	    {
		    var obj = await GameObjectPoolManager.GetInstance().GetGameObjectAsync(path);
		    obj.transform.position = pos;
		    await TimerManager.Instance.WaitAsync(during);
		    GameObjectPoolManager.GetInstance().RecycleGameObject(obj);
	    }
	    
	    public static void ShowPlayView(ItemConfig cfg)
	    {
		    var name = ((ItemType) cfg.Type).ToString();
		    UIManager.Instance.OpenWindow($"TaoTie.UI{name}View",
			    $"UIGame/UIMiniGame/Prefabs/UI{name}View.prefab", cfg.Id).Coroutine();
		    PlayerDataManager.Instance.RemoveUnlockList(cfg.ContainerId, (ItemType) cfg.Type);
	    }
    }
}