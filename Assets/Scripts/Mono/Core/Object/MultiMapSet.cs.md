# MultiMapSet.cs 注解文档

## 文件基本信息
- **路径**: Assets/Scripts/Mono/Core/Object/MultiMapSet.cs
- **职责**: 提供一对多的映射集合（T → 多个 K）

## 类说明
### MultiMapSet<T, K>
- **继承**: SortedDictionary<T, HashSet<K>>
- **用途**: 一个 key 对应多个 value 的集合

## 方法
### Add
```csharp
public void Add(T t, K k)
```
添加 key-value 对

### Remove
```csharp
public bool Remove(T t, K k)
```
移除指定的 key-value 对

### GetAll
```csharp
public K[] GetAll(T t)
```
获取 key 对应的所有 value（返回 copy）

### GetOne
```csharp
public K GetOne(T t)
```
获取 key 对应的任意一个 value

### Contains
```csharp
public bool Contains(T t, K k)
```
检查是否包含指定的 key-value 对

## 使用示例
```csharp
var multiMap = new MultiMapSet<string, int>();
multiMap.Add("fruits", 1);
multiMap.Add("fruits", 2);
multiMap.Add("vegetables", 3);

int[] fruits = multiMap.GetAll("fruits");  // [1, 2]
int one = multiMap.GetOne("fruits");       // 1 或 2
bool has = multiMap.Contains("fruits", 1); // true
```
