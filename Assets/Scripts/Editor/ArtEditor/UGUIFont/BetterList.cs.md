# BetterList.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | BetterList.cs |
| **路径** | Assets/Scripts/Editor/ArtEditor/UGUIFont/BetterList.cs |
| **所属模块** | Editor → 美术编辑器 → UGUI 字体工具 |
| **文件职责** | 高性能列表实现，避免 Clear() 时释放缓冲区导致的 GC 开销 |

---

## 类/结构体说明

### BetterList<T>

| 属性 | 说明 |
|------|------|
| **职责** | 提供比 System.Collections.Generic.List<T> 更好的性能，特别是在 Add/Remove 操作时，通过复用内部缓冲区减少 GC 分配 |
| **泛型参数** | `T` - 列表元素类型 |
| **继承关系** | 无继承 |
| **实现的接口** | `IEnumerable<T>` (通过 GetEnumerator) |

**设计模式**: 对象池模式 (缓冲区复用)

**来源**: 基于 NGUI (Next-Gen UI kit) 的 BetterList，Copyright © 2011-2014 Tasharen Entertainment

**平台差异**:
- **Unity Flash**: 使用内置 `List<T>` 包装 (Flash 不支持直接数组操作)
- **其他平台**: 使用原生数组实现，性能更优

---

## 字段与属性（按重要程度排序）

### 非 Flash 平台实现

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `buffer` | `T[]` | `public` | 内部缓冲区数组 |
| `size` | `int` | `public` | 当前元素数量 (不是数组长度) |
| `this[int i]` | `T` | `public` | 索引器，访问指定位置的元素 |

### Flash 平台实现

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `mList` | `List<T>` | `private` | 包装的 List<T> |
| `buffer` | `List<T>` | `public` | 返回 mList (兼容语法) |
| `size` | `int` | `public` | 返回 mList.Count |

---

## 方法说明（按重要程度排序）

### Add()

**签名**:
```csharp
public void Add(T item)
```

**职责**: 在列表末尾添加元素

**核心逻辑** (非 Flash):
```
1. 检查 buffer 是否为 null 或已满
2. 如需扩容，调用 AllocateMore() (容量翻倍，最小 32)
3. 将元素放入 buffer[size]
4. size++
```

**性能优势**: 不触发 GC (缓冲区复用)

---

### Clear()

**签名**:
```csharp
public void Clear()
```

**职责**: 清空列表，但不释放内存

**核心逻辑**:
```
非 Flash: size = 0 (缓冲区保留)
Flash: mList.Clear()
```

**与 List<T>.Clear() 的区别**:
- `List<T>.Clear()`: 清空并释放内部数组
- `BetterList.Clear()`: 仅重置 size，缓冲区保留，下次 Add 无需重新分配

---

### Release()

**签名**:
```csharp
public void Release()
```

**职责**: 清空列表并释放内存

**核心逻辑**:
```
非 Flash: size = 0; buffer = null;
Flash: mList.Clear()
```

**使用场景**: 确定不再使用该列表时，主动释放内存

---

### RemoveAt()

**签名**:
```csharp
public void RemoveAt(int index)
```

**职责**: 移除指定索引的元素

**核心逻辑**:
```
1. 检查索引有效性
2. 将 buffer[index] 设为 default(T)
3. 将 index 之后的元素向前移动一位
4. size--
5. 将最后一个位置设为 default(T)
```

**时间复杂度**: O(n) - 需要移动元素

---

### Remove()

**签名**:
```csharp
public bool Remove(T item)
```

**职责**: 移除指定值的元素

**核心逻辑**:
```
1. 遍历数组查找匹配项 (使用 EqualityComparer<T>.Default)
2. 找到后调用 RemoveAt()
3. 返回 true，未找到返回 false
```

**时间复杂度**: O(n)

---

### Pop()

**签名**:
```csharp
public T Pop()
```

**职责**: 移除并返回最后一个元素

**核心逻辑**:
```
1. 检查 buffer 和 size
2. size--
3. 返回 buffer[size]
4. 将 buffer[size] 设为 default(T)
```

**时间复杂度**: O(1)

---

### Sort()

**签名**:
```csharp
public void Sort(CompareFunc comparer)
```

**职责**: 对列表排序 (冒泡排序实现)

**核心逻辑**:
```
1. 使用冒泡排序
2. 调用 comparer 比较相邻元素
3. 如需要则交换
4. 优化：记录最后交换位置，减少下一轮比较范围
```

**时间复杂度**: O(n²) - 冒泡排序

**注意**: 注释掉的版本使用 Array.Sort()，性能更好但会产生 GC

---

### AllocateMore()

**签名**:
```csharp
void AllocateMore()
```

**职责**: 扩容缓冲区

**核心逻辑**:
```
1. 新容量 = max(当前容量 * 2, 32)
2. 创建新数组
3. 复制旧数据到新数组
4. 替换 buffer 引用
```

---

### Trim()

**签名**:
```csharp
void Trim()
```

**职责**: 修剪缓冲区到实际大小

**核心逻辑**:
```
1. 如果 size < buffer.Length
2. 创建大小为 size 的新数组
3. 复制数据
4. 替换 buffer
```

**使用场景**: 列表稳定后，释放多余内存

---

## 性能对比

### BetterList vs List<T>

| 操作 | BetterList | List<T> | 优势 |
|------|------------|---------|------|
| Add | O(1)* | O(1)* | 相同 |
| RemoveAt | O(n) | O(n) | 相同 |
| Remove | O(n) | O(n) | 相同 |
| Clear | O(1) | O(1) | BetterList 不释放内存 |
| 扩容 | 手动控制 | 自动 | BetterList 可预测 |
| GC 分配 | 少 | 多 | BetterList 胜 |

*均摊时间复杂度

---

## 使用示例

### 示例 1: 基本使用

```csharp
// 创建列表
BetterList<int> list = new BetterList<int>();

// 添加元素
list.Add(1);
list.Add(2);
list.Add(3);

// 访问元素
int first = list[0];  // 1

// 遍历
foreach (int item in list)
{
    Debug.Log(item);
}

// 清空 (保留内存)
list.Clear();

// 再次添加 (不会重新分配)
list.Add(4);
```

### 示例 2: 对象池场景

```csharp
// 对象池中使用 BetterList
public class ObjectPool<T>
{
    private BetterList<T> pool = new BetterList<T>();
    
    public T Fetch()
    {
        if (pool.size > 0)
        {
            return pool.Pop();  // O(1) 取出
        }
        return default(T);
    }
    
    public void Recycle(T obj)
    {
        pool.Add(obj);  // O(1) 归还，无 GC
    }
    
    public void Clear()
    {
        pool.Clear();  // 不清空缓冲区，下次复用
    }
}
```

### 示例 3: 排序

```csharp
BetterList<Entity> entities = new BetterList<Entity>();

// 按 ID 排序
entities.Sort((left, right) => left.id.CompareTo(right.id));

// 按距离排序
entities.Sort((left, right) => 
{
    float distL = Vector3.Distance(left.pos, player.pos);
    float distR = Vector3.Distance(right.pos, player.pos);
    return distL.CompareTo(distR);
});
```

---

## 注意事项

1. **不要使用 buffer.Length**: 应使用 `size` 属性
2. **Clear() 不释放内存**: 如需释放，调用 `Release()`
3. **排序性能**: BetterList.Sort() 使用冒泡排序，大数据量时考虑使用 List<T>
4. **线程安全**: 不是线程安全的，多线程环境需加锁
5. **序列化**: buffer 数组可序列化，size 也会序列化

---

## 适用场景

### 适合使用 BetterList

- 频繁 Add/Remove 的列表
- 对象池实现
- 每帧更新的临时列表
- 对 GC 敏感的场景 (如移动端、VR)

### 不适合使用 BetterList

- 需要频繁排序的列表
- 需要 List<T> 特有功能的场景 (如 FindAll, ConvertAll)
- 需要序列化为 JSON/XML 的场景

---

## 相关文档

- [BMFont.cs.md](./BMFont.cs.md) - BMFont 数据结构 (使用 BetterList 存储 glyphs)
- [NGUI 文档](https://www.nGUI.net) - NGUI 框架文档

---

*文档生成时间：2026-03-02 | OpenClaw AI 助手*
