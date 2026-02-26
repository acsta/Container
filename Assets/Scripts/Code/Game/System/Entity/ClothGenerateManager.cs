using System.Linq;
using UnityEngine;

namespace TaoTie
{
    public class ClothGenerateManager: IManager
    {
        public static ClothGenerateManager Instance;

        private int offset = 0;
        private ListComponent<int[]> temp;
        private int total;
        #region IManager

        public void Init()
        {
            Instance = this;
            total = 0;
            temp = ListComponent<int[]>.Create();
        }
        
        public void Destroy()
        {
            total = 0;
            temp.Dispose();
            Instance = null;
        }
        #endregion

        public void Generate(int count, int? level)
        {
            var list = CharacterConfigCategory.Instance.GetAllList();
            for (int i = temp.Count; i < count; i++)
            {
                temp.Add(new int[list.Count]);
            }

            total = count;
            if (total <= 0)
            {
                total = 1;
                Log.Error("total <= 0");
            }
            for (int i = 0; i < list.Count; i++)
            {
                var models = ClothConfigCategory.Instance.GetModule(list[i].Id);
                if (list[i].DefaultCloth == 0)
                {
                    var tempIndex = Random.Range(0, models.Count);
                    var tempPos = Random.Range(0, total);
                    for (int j = 0; j < total; j++)
                    {
                        temp[j][i] = 0;
                    }

                    for (int j = 0; j < models.Count; j++)
                    {
                        var index = (tempIndex + j) % models.Count;
                        temp[tempPos][i] = models[index].Id;
                        if (level == null && models[index].MainScene == 1)
                        {
                            if (PlayerDataManager.Instance?.Show == null ||
                                models[index].Id != PlayerDataManager.Instance.Show[i]) break;
                        }

                        if (level != null && models[index].LevelIds.Contains((int) level))
                        {
                            if (PlayerDataManager.Instance?.Show == null ||
                                models[index].Id != PlayerDataManager.Instance.Show[i]) break;
                        }
                    }
                }
                else
                {
                    models.RandomSort();
                    for (int j = 0; j < total; j++)
                    {
                        temp[j][i] = list[i].DefaultCloth;
                    }
                    var tempIndex = 0;
                    for (int j = 0; j < models.Count * temp.Count; j++)
                    {
                        var index = j % models.Count;
                        if (level == null && models[index].MainScene == 1)
                        {
                            temp[tempIndex][i] = models[index].Id;
                            if (PlayerDataManager.Instance?.Show == null ||
                                models[index].Id != PlayerDataManager.Instance.Show[i])
                            {
                                tempIndex++;
                                if (tempIndex >= temp.Count) break;
                                continue;
                            }
                        }

                        if (level != null && models[index].LevelIds.Contains((int) level))
                        {
                            temp[tempIndex][i] = models[index].Id;
                            if (PlayerDataManager.Instance?.Show == null ||
                                models[index].Id != PlayerDataManager.Instance.Show[i])
                            {
                                tempIndex++;
                                if (tempIndex >= temp.Count) break;
                            }
                        }
                    }
                    var tempPos = Random.Range(0, total);
                    temp[tempPos][i] = list[i].DefaultCloth;
                }
            }
        }

        public int[] GetNext()
        {
            var max = Mathf.Max(total, temp.Count);
            offset = (offset + 1) % max;
            return temp[offset];
        }
    }
}