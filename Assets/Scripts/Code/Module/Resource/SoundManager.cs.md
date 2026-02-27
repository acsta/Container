# SoundManager.cs 注解文档

## 文件基本信息

| 属性 | 值 |
|------|-----|
| **文件名** | SoundManager.cs |
| **路径** | Assets/Scripts/Code/Module/Resource/SoundManager.cs |
| **所属模块** | 框架层 → Code/Module/Resource |
| **文件职责** | 音效管理系统，提供背景音乐、音效播放、在线音频加载 |

---

## 类/结构体说明

### SoundManager

| 属性 | 说明 |
|------|------|
| **职责** | 管理所有音频播放，支持本地/在线音频 |
| **泛型参数** | 无 |
| **继承关系** | 无继承 |
| **实现的接口** | `IManager` |

**设计模式**: 单例模式 + 对象池 + 缓存

```csharp
// 单例实现
public static SoundManager Instance { get; private set; }

// 通过 ManagerProvider 注册
ManagerProvider.RegisterManager<SoundManager>();
```

---

## SoundItem (内部类)

| 属性 | 说明 |
|------|------|
| **职责** | 封装单个音频播放项 |
| **字段** | Id, AudioSource, Clip, Path, Token |

**对象池管理**:
```csharp
// 从对象池获取
SoundItem item = ObjectPool.Instance.Fetch<SoundItem>();

// 使用完毕回收到对象池
ObjectPool.Instance.Recycle(item);
```

---

## 字段与属性（按重要程度排序）

| 名称 | 类型 | 访问级别 | 说明 |
|------|------|----------|------|
| `Instance` | `SoundManager` | `public static` | 单例实例 |
| `soundsPool` | `LinkedList<AudioSource>` | `private` | AudioSource 对象池 |
| `sounds` | `Dictionary<long, SoundItem>` | `private` | 正在播放的音频 {id: SoundItem} |
| `audioClips` | `Dictionary<string, AudioClip>` | `private` | 音频缓存 {path: AudioClip} |
| `MusicVolume` | `int` | `public` | 背景音乐音量 |
| `SoundVolume` | `int` | `public` | 音效音量 |
| `curMusic` | `SoundItem` | `private` | 当前播放的背景音乐 |
| `soundsRoot` | `Transform` | `private` | 音频对象父节点 |

---

## 方法说明（按重要程度排序）

### InitAsync()

**签名**:
```csharp
public async ETTask InitAsync()
```

**职责**: 异步初始化音频系统

**核心逻辑**:
```
1. 创建 "SoundsRoot" GameObject
2. 从 UICamera 移除 AudioListener（如果有）
3. 在 SoundsRoot 上添加 AudioListener
4. 加载 AudioSource 预制体
5. 初始化 AudioSource 对象池（DEFAULTVALUE=10 个）
```

**调用者**: Entry.cs（游戏启动时）

---

### PlayMusic(string path)

**签名**:
```csharp
public void PlayMusic(string path)
```

**职责**: 播放背景音乐

**参数**:
- `path`: 音频路径

**核心逻辑**:
```
1. 如果已有背景音乐在播放，停止
2. 加载音频（本地或在线）
3. 创建 SoundItem
4. 设置 AudioSource.clip
5. 播放
```

**调用者**: 场景切换、游戏启动

**使用示例**:
```csharp
// 播放背景音乐
SoundManager.Instance.PlayMusic("Audio/Music/MainView.mp3");
```

---

### PlaySound(string path, bool loop = false)

**签名**:
```csharp
public async ETTask<long> PlaySound(string path, bool loop = false)
```

**职责**: 播放音效

**参数**:
- `path`: 音频路径
- `loop`: 是否循环

**返回**: `long` - 音频 ID（用于停止）

**核心逻辑**:
```
1. 从 soundsPool 获取 AudioSource
2. 加载音频
3. 创建 SoundItem
4. 设置 loop
5. 播放
6. 加入 sounds 字典
7. 返回 ID
```

**调用者**: UI 点击、游戏事件

**使用示例**:
```csharp
// 播放音效
long soundId = await SoundManager.Instance.PlaySound("Audio/Sound/ButtonClick");

// 停止音效
SoundManager.Instance.StopSound(soundId);
```

---

### StopSound(long id)

**签名**:
```csharp
public void StopSound(long id)
```

**职责**: 停止音效

**参数**:
- `id`: 音频 ID

**核心逻辑**:
```
1. 从 sounds 字典获取 SoundItem
2. 调用 Dispose() 停止播放
3. 从 sounds 字典移除
```

**调用者**: 需要停止音效的场景

---

### StopMusic()

**签名**:
```csharp
public void StopMusic()
```

**职责**: 停止背景音乐

**核心逻辑**:
```
1. 如果 curMusic 不为空，调用 Dispose()
2. curMusic = null
```

**调用者**: 场景切换

---

### SetMusicVolume(int volume)

**签名**:
```csharp
public void SetMusicVolume(int volume)
```

**职责**: 设置背景音乐音量

**参数**:
- `volume`: 音量（0-100）

**核心逻辑**:
```
1. 保存 MusicVolume = volume
2. 如果 curMusic 不为空，设置 AudioSource.volume
```

---

### SetSoundVolume(int volume)

**签名**:
```csharp
public void SetSoundVolume(int volume)
```

**职责**: 设置音效音量

**参数**:
- `volume`: 音量（0-100）

**核心逻辑**:
```
1. 保存 SoundVolume = volume
2. 遍历所有 sounds，更新 AudioSource.volume
```

---

## 在线音频加载

### GetOnlineClip(string url, int tryCount, ETCancellationToken cancel)

**签名**:
```csharp
private async ETTask<AudioClip> GetOnlineClip(string url, int tryCount = 3, ETCancellationToken cancel = null)
```

**职责**: 从网络加载音频

**核心逻辑**:
```
1. 检查本地缓存
2. 如果本地没有，HTTP 下载
3. 重试 tryCount 次
4. 保存为 WAV 文件（后台线程）
5. 返回 AudioClip
```

**WAV 文件头写入**:
```csharp
// 创建 WAV 文件头
private FileStream CreateEmpty(string filepath)
private void WriteHeader(Stream stream, int hz, int channels, int samples)
```

---

## 阅读指引

### 建议的阅读顺序

1. **理解音频管理器作用** - 为什么需要 SoundManager
2. **看 PlayMusic/PlaySound** - 理解音频播放
3. **看 SoundItem** - 理解音频项管理
4. **看在线音频加载** - 理解网络音频处理

### 最值得学习的技术点

1. **对象池**: soundsPool 复用 AudioSource
2. **音频缓存**: audioClips 缓存已加载音频
3. **在线加载**: HTTP 下载音频并保存为 WAV
4. **WAV 文件头**: 手动写入 WAV 文件头
5. **ID 管理**: 每个播放项有唯一 ID

---

## 使用示例

### 示例 1: 播放背景音乐

```csharp
// 播放背景音乐
SoundManager.Instance.PlayMusic("Audio/Music/MainView.mp3");

// 停止背景音乐
SoundManager.Instance.StopMusic();

// 设置音量
SoundManager.Instance.SetMusicVolume(80);  // 80%
```

### 示例 2: 播放音效

```csharp
// 播放音效
long soundId = await SoundManager.Instance.PlaySound("Audio/Sound/ButtonClick");

// 播放循环音效
long ambientId = await SoundManager.Instance.PlaySound("Audio/Sound/Wind", loop: true);

// 停止音效
SoundManager.Instance.StopSound(soundId);
SoundManager.Instance.StopSound(ambientId);
```

### 示例 3: 音量控制

```csharp
// 设置背景音乐音量
SoundManager.Instance.SetMusicVolume(70);

// 设置音效音量
SoundManager.Instance.SetSoundVolume(50);

// 静音
SoundManager.Instance.SetMusicVolume(0);
SoundManager.Instance.SetSoundVolume(0);
```

---

## 相关文档

- [ResourcesManager.cs.md](./ResourcesManager.cs.md) - 资源管理器
- [GameObjectPoolManager.cs.md](./GameObjectPoolManager.cs.md) - GameObject 对象池
- [HttpManager.cs.md](../Net/HttpManager.cs.md) - HTTP 管理

---

*文档生成时间：2026-02-27 | OpenClaw AI 助手*
