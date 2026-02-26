using System;
using System.Collections.Generic;

namespace TaoTie
{
    public class GroupInfoTable: UIBaseContainer,IOnCreate
    {

        public GroupInfo[] GroupInfos;
        public void OnCreate()
        {
            GroupInfos = new GroupInfo[3];
            for (int i = 0; i < GroupInfos.Length; i++)
            {
                GroupInfos[i] = AddComponent<GroupInfo>("GroupInfo" + i);
            }
        }

        public void SetData(Dictionary<int ,int> equipGroup)
        {
            for (int i = 0; i < GroupInfos.Length; i++)
            {
                GroupInfos[i].SetActive(equipGroup.Count > 0);
            }
            
            if (equipGroup.Count > 0)
            {
                int top = 0;
                int max = int.MinValue;
                int sameMax = 0;
                foreach (var item in equipGroup)
                {
                    if (item.Value > max)
                    {
                        top = item.Key;
                        max = item.Value;
                        sameMax = 1;
                    }
                    else if (item.Value == max)
                    {
                        sameMax++;
                    }
                }

                if (sameMax > 1)
                {
                    int index = 0;
                    foreach (var item in equipGroup)
                    {
                        if (item.Value == max)
                        {
                            EquipGroupConfigCategory.Instance.TryGet(top,out var groupConf);
                            if (groupConf!= null && groupConf.Count.Length > 0 && max >= groupConf.Count[0])
                            {
                                GroupInfos[index].SetData(0, groupConf.Count[0], groupConf.EffectType[0],
                                    groupConf.Param[0], true);
                                index++;
                                top = item.Value;
                            }
                        }
                    }

                    if (index != 1)
                    {
                        for (int i = index; i < GroupInfos.Length; i++)
                        {
                            GroupInfos[i].SetActive(false);
                        }
                        return;
                    }
                }

                EquipGroupConfigCategory.Instance.TryGet(top, out var groupConfig);
                for (int i = 0; i < GroupInfos.Length; i++)
                {
                    if (groupConfig!= null && i < groupConfig.Count.Length)
                    {
                        GroupInfos[i].SetData(i, groupConfig.Count[i], groupConfig.EffectType[i], groupConfig.Param[i],
                            max >= groupConfig.Count[i]);
                    }
                    else
                    {
                        GroupInfos[i].SetActive(false);
                    }
                }
            }
        }
    }
}