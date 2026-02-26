using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    /// <summary>
    /// 场景单位
    /// </summary>
    public abstract class Unit : SceneEntity
    {
        public int ConfigId { get; protected set; } //配置表id

        public UnitConfig Config => UnitConfigCategory.Instance.Get(this.ConfigId);
    }
}