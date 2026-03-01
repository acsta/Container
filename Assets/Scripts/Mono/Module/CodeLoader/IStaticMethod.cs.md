# IStaticMethod.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|------|
| **文件名** | IStaticMethod.cs |
| **路径** | Assets/Scripts/Mono/Module/CodeLoader/IStaticMethod.cs |
| **所属模块** | 框架层 → Mono/Module/CodeLoader |
| **文件职责** | 定义静态方法调用的接口，支持反射调用热更新代码 |

---

## 类/结构体说明

### IStaticAction

| 属性 | 说明 |
|------|------|
| **职责** | 无参数无返回值的静态方法接口 |
| **泛型参数** | 无 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

**设计模式**: 命令模式

```csharp
// 接口定义
public interface IStaticAction
{
    public void Run();
}
```

**用途**: 调用热更新代码中的静态无参方法（如 Entry.Start）

---

### IStaticFunc<T>

| 属性 | 说明 |
|------|------|
| **职责** | 无参数有返回值的静态方法接口 |
| **泛型参数** | `T` - 返回值类型 |
| **继承关系** | 无 |
| **实现的接口** | 无 |

```csharp
// 接口定义
public interface IStaticFunc<T>
{
    public T Run();
}
```

**用途**: 调用热更新代码中的静态无参方法并获取返回值

---

## 接口方法说明

### IStaticAction.Run

**签名**:
```csharp
public void Run()
```

**职责**: 执行静态方法

**调用者**: CodeLoader.Start()

**用途**: 启动热更新代码入口

---

### IStaticFunc<T>.Run

**签名**:
```csharp
public T Run()
```

**职责**: 执行静态方法并返回结果

**返回值**: `T` - 方法返回值

**用途**: 调用热更新代码中的静态工厂方法、查询方法等

---

## 使用示例

### 示例 1: 调用 Entry.Start

```csharp
// CodeLoader 中调用热更新入口
IStaticAction start = new MonoStaticAction(assembly, "TaoTie.Entry", "Start");
start.Run();
```

### 示例 2: 调用静态工厂方法

```csharp
// 假设热更新代码中有：public static Player Create()
IStaticFunc<Player> factory = new MonoStaticFunc<Player>(assembly, "TaoTie.Player", "Create");
Player player = factory.Run();
```

### 示例 3: 调用静态查询方法

```csharp
// 假设热更新代码中有：public static int GetVersion()
IStaticFunc<int> getVersion = new MonoStaticFunc<int>(assembly, "TaoTie.Version", "GetVersion");
int version = getVersion.Run();
```

---

## 设计要点

### 为什么需要这些接口？

**问题**: 热更新代码在运行时加载，无法在编译时确定类型，无法直接调用方法。

**解决方案**:
1. 定义统一接口（IStaticAction / IStaticFunc<T>）
2. 通过反射创建实现类（MonoStaticAction / MonoStaticFunc<T>）
3. 通过接口调用，解耦调用方和具体实现

### 接口 vs 直接反射

```csharp
// ❌ 直接反射（每次调用都要查找）
var method = assembly.GetType("TaoTie.Entry").GetMethod("Start");
method.Invoke(null, null);

// ✅ 使用接口（预先绑定，调用快速）
IStaticAction start = new MonoStaticAction(assembly, "TaoTie.Entry", "Start");
start.Run(); // 直接调用委托
```

**优势**:
- 性能更好：反射只执行一次，后续直接调用委托
- 类型安全：泛型接口确保返回值类型正确
- 代码清晰：接口定义明确，易于理解

---

## 扩展性

### 添加带参数的接口

```csharp
// 带一个参数的接口
public interface IStaticAction<T1>
{
    public void Run(T1 arg1);
}

// 带两个参数的接口
public interface IStaticAction<T1, T2>
{
    public void Run(T1 arg1, T2 arg2);
}

// 带参数和返回值的接口
public interface IStaticFunc<T1, TResult>
{
    public TResult Run(T1 arg1);
}
```

### 实现示例

```csharp
public class MonoStaticAction<T1> : IStaticAction<T1>
{
    private Action<T1> method;
    
    public MonoStaticAction(Assembly assembly, string typeName, string methodName)
    {
        var methodInfo = assembly.GetType(typeName).GetMethod(methodName);
        this.method = (Action<T1>)Delegate.CreateDelegate(typeof(Action<T1>), null, methodInfo);
    }
    
    public void Run(T1 arg1)
    {
        this.method(arg1);
    }
}
```

---

## 相关文档

- [CodeLoader.cs.md](./CodeLoader.cs.md) - 代码加载器（使用这些接口）
- [MonoStaticMethod.cs.md](./MonoStaticMethod.cs.md) - 接口实现类
- [AssemblyManager.cs.md](../Assembly/AssemblyManager.cs.md) - 程序集管理器

---

*文档生成时间：2026-03-01 | OpenClaw AI 助手*
