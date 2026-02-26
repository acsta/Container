using System.Collections.Generic;

namespace TaoTie
{
	public class NumericComponent:Component,IComponent
	{
		#region override
		public void Init()
		{
			NumericDic = new Dictionary<int, decimal>();
			ManagerProvider.RegisterManager<NumericSystem>().AddComponent(this);
		}
		
		public void Destroy()
		{
			ManagerProvider.RegisterManager<NumericSystem>().RemoveComponent(this);
			NumericDic.Clear();
			NumericDic = null;
		}

		#endregion
		
		private Dictionary<int, decimal> NumericDic;

		private decimal this[int numericType]
		{
			get
			{
				return this.GetByKey(numericType);
			}
			set
			{
				if (numericType < NumericType.Max)
				{
					Log.Error("不允许直接设置最终值，请检查代码");
					numericType = numericType * 10 + 1;
				}
				this.Insert(numericType,value);
			}
		}
		
		public float GetAsFloat(int numericType)
		{
			if(this.IsFloat(numericType))
				return (float)this.GetByKey(numericType)/10000f;
			else
				return (float)this.GetByKey(numericType);
		}

		public int GetAsInt(int numericType)
		{
			if(this.IsFloat(numericType))
				return (int)this.GetByKey(numericType)/10000;
			else
				return (int)this.GetByKey(numericType);
		}
		
		public decimal GetAsDecimal(int numericType)
		{
			if(this.IsFloat(numericType))
				return this.GetByKey(numericType)/10000;
			else
				return this.GetByKey(numericType);
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="nt"> nt>NumericType.Max </param>
		/// <param name="value"></param>
		/// <param name="isRealValue"></param>
		public void Set(int nt, float value, bool isRealValue = false)
		{
			if (!isRealValue && this.IsFloat(nt))
				this[nt] = (int) (value * 10000);
			else
				this[nt] = (int) value;
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="nt"> nt>NumericType.Max </param>
		/// <param name="value"></param>
		/// <param name="isRealValue"></param>
		public void Set(int nt, int value, bool isRealValue = false)
		{
			if (!isRealValue && this.IsFloat(nt))
				this[nt] = value * 10000;
			else
				this[nt] = value;
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="nt"> nt>NumericType.Max </param>
		/// <param name="value"></param>
		/// <param name="isRealValue"></param>
		public void Set(int nt, decimal value, bool isRealValue = false)
		{
			if (!isRealValue && this.IsFloat(nt))
				this[nt] = value * 10000;
			else
				this[nt] = value;
		}

		public void SetNoEvent(int numericType, decimal value)
		{
			this.Insert(numericType,value,false);
		}
		
		private void Insert(int numericType, decimal value,bool isPublicEvent = true)
		{
			decimal oldValue = this.GetByKey(numericType);
			if (oldValue == value)
			{
				return;
			}

			this.NumericDic[numericType] = value;

			if (numericType >= NumericType.Max)
			{
				this.Update(numericType,isPublicEvent);
				return;
			}

			if (isPublicEvent)
			{
				NumericChange args = new NumericChange();
				args.Parent = this.parent;
				args.NumericType = numericType;
				args.Old = oldValue;
				args.New = value;
				Messager.Instance.Broadcast(Id,MessageId.NumericChangeEvt,args);
			}
		}
		
		private decimal GetByKey(int key)
		{
			decimal value = 0;
			this.NumericDic.TryGetValue(key, out value);
			return value;
		}

		private void Update(int numericType,bool isPublicEvent)
		{
			int final = numericType / 10;
			int bas = final * 10 + 1; 
			int add = final * 10 + 2;
			int pct = final * 10 + 3;
			int finalAdd = final * 10 + 4;
			int finalPct = final * 10 + 5;

			// 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
			// final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
			decimal result = ((GetByKey(bas) + GetByKey(add)) * (100 + GetAsDecimal(pct)) / 100 + GetByKey(finalAdd)) * (100 + GetAsDecimal(finalPct)) / 100;
			Insert(final,result,isPublicEvent);
		}
		
		/// <summary>
		/// 读表判断是取整还是保留小数
		/// </summary>
		/// <param name="numericType"></param>
		/// <returns></returns>
		private bool IsFloat(int numericType)
		{
			if (numericType > NumericType.Max)
			{
				var flag = numericType % 10;
				if (flag == 3 || flag == 5) return true;//百分比的是小数,否则看配置表
				numericType /= 10;
			}
			var attr = AttributeConfigCategory.Instance.Get(numericType);
			return attr.Type == 1;
		}
	}
	
}