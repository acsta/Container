using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Sirenix.Utilities.dll",
		"System.Core.dll",
		"System.dll",
		"Unity.Mono.dll",
		"Unity.ThirdParty.dll",
		"UnityEngine.CoreModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Nino.Serialization.NinoWrapperBase<object>
	// Nino.Shared.UncheckedStack.Enumerator<object>
	// Nino.Shared.UncheckedStack<object>
	// System.Action<DotRecast.Detour.StraightPathItem>
	// System.Action<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Action<System.ValueTuple<long,int,object>>
	// System.Action<TaoTie.CoroutineLockTimer>
	// System.Action<TaoTie.HitInfo>
	// System.Action<TaoTie.NumericChange>
	// System.Action<UnityEngine.EventSystems.RaycastResult>
	// System.Action<UnityEngine.Vector3,int>
	// System.Action<UnityEngine.Vector3>
	// System.Action<byte>
	// System.Action<int,int>
	// System.Action<int,object>
	// System.Action<int>
	// System.Action<long,byte,object>
	// System.Action<long,byte>
	// System.Action<long,long>
	// System.Action<long>
	// System.Action<object,UnityEngine.Quaternion>
	// System.Action<object,UnityEngine.Vector3>
	// System.Action<object,byte>
	// System.Action<object,float,int,float>
	// System.Action<object,float>
	// System.Action<object,int>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment.Enumerator<ushort>
	// System.ArraySegment<byte>
	// System.ArraySegment<ushort>
	// System.ByReference<byte>
	// System.ByReference<ushort>
	// System.Collections.Generic.ArraySortHelper<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.ArraySortHelper<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.ArraySortHelper<TaoTie.HitInfo>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Comparer<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.Comparer<TaoTie.HitInfo>
	// System.Collections.Generic.Comparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.ComparisonComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.ComparisonComparer<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.ComparisonComparer<TaoTie.HitInfo>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ComparisonComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<long>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.Enumerator<object,float>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,long>
	// System.Collections.Generic.Dictionary.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary.KeyCollection<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary.KeyCollection<object,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<object,float>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,long>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,long>
	// System.Collections.Generic.Dictionary.KeyCollection<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ulong,object>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary.ValueCollection<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary.ValueCollection<object,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<object,float>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,long>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,long>
	// System.Collections.Generic.Dictionary.ValueCollection<ulong,object>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.Dictionary<int,byte>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.Dictionary<object,byte>
	// System.Collections.Generic.Dictionary<object,float>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<ulong,long>
	// System.Collections.Generic.Dictionary<ulong,object>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.EqualityComparer<TaoTie.SensibleInfo>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.EqualityComparer<ulong>
	// System.Collections.Generic.HashSet.Enumerator<byte>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet.Enumerator<uint>
	// System.Collections.Generic.HashSet<byte>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet<uint>
	// System.Collections.Generic.HashSetEqualityComparer<byte>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.HashSetEqualityComparer<uint>
	// System.Collections.Generic.ICollection<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int,object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,TaoTie.SensibleInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Playables.PlayableBinding>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.ICollection<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.ICollection<TaoTie.HitInfo>
	// System.Collections.Generic.ICollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<uint>
	// System.Collections.Generic.IComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.IComparer<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.IComparer<TaoTie.HitInfo>
	// System.Collections.Generic.IComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,LitJson.ArrayMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.ObjectMetadata>
	// System.Collections.Generic.IDictionary<object,LitJson.PropertyMetadata>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IEnumerable<LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int,object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,TaoTie.SensibleInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Playables.PlayableBinding>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.IEnumerable<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.IEnumerable<TaoTie.HitInfo>
	// System.Collections.Generic.IEnumerable<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerable<UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<uint>
	// System.Collections.Generic.IEnumerator<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IEnumerator<LitJson.PropertyMetadata>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int,object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,TaoTie.SensibleInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Playables.PlayableBinding>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ulong,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.IEnumerator<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.IEnumerator<TaoTie.HitInfo>
	// System.Collections.Generic.IEnumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IEnumerator<UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<uint>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IEqualityComparer<ulong>
	// System.Collections.Generic.IList<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IList<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.IList<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.IList<TaoTie.HitInfo>
	// System.Collections.Generic.IList<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,System.ValueTuple<long,int,object>>
	// System.Collections.Generic.KeyValuePair<int,byte>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,TaoTie.SensibleInfo>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.KeyValuePair<object,byte>
	// System.Collections.Generic.KeyValuePair<object,float>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,long>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.KeyValuePair<ulong,long>
	// System.Collections.Generic.KeyValuePair<ulong,object>
	// System.Collections.Generic.LinkedList.Enumerator<int>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<int>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<int>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.List.Enumerator<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.List.Enumerator<TaoTie.HitInfo>
	// System.Collections.Generic.List.Enumerator<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.List<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.List<TaoTie.HitInfo>
	// System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.ObjectComparer<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.ObjectComparer<TaoTie.HitInfo>
	// System.Collections.Generic.ObjectComparer<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<long,int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<TaoTie.SensibleInfo>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Playables.PlayableBinding>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ulong>
	// System.Collections.Generic.Queue.Enumerator<System.ValueTuple<int,long,int>>
	// System.Collections.Generic.Queue.Enumerator<TaoTie.CoroutineLockInfo>
	// System.Collections.Generic.Queue.Enumerator<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.Queue.Enumerator<long>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<System.ValueTuple<int,long,int>>
	// System.Collections.Generic.Queue<TaoTie.CoroutineLockInfo>
	// System.Collections.Generic.Queue<TaoTie.CoroutineLockTimer>
	// System.Collections.Generic.Queue<long>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<long,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<int,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<long,object>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<DotRecast.Detour.StraightPathItem>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<long,int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<TaoTie.CoroutineLockTimer>
	// System.Collections.ObjectModel.ReadOnlyCollection<TaoTie.HitInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.EventSystems.RaycastResult>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<DotRecast.Detour.StraightPathItem>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Comparison<System.ValueTuple<long,int,object>>
	// System.Comparison<TaoTie.CoroutineLockTimer>
	// System.Comparison<TaoTie.HitInfo>
	// System.Comparison<UnityEngine.EventSystems.RaycastResult>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Func<object,byte>
	// System.Func<object,int,int,int,object>
	// System.Func<object,int,object>
	// System.Func<object,object,byte>
	// System.Func<object,object>
	// System.Func<object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Nullable<UnityEngine.Vector3>
	// System.Nullable<int>
	// System.Predicate<DotRecast.Detour.StraightPathItem>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Predicate<System.ValueTuple<long,int,object>>
	// System.Predicate<TaoTie.CoroutineLockTimer>
	// System.Predicate<TaoTie.HitInfo>
	// System.Predicate<UnityEngine.EventSystems.RaycastResult>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<byte>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Predicate<uint>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan.Enumerator<ushort>
	// System.ReadOnlySpan<byte>
	// System.ReadOnlySpan<ushort>
	// System.Span.Enumerator<byte>
	// System.Span.Enumerator<ushort>
	// System.Span<byte>
	// System.Span<ushort>
	// System.ValueTuple<int,long,int>
	// System.ValueTuple<long,int,object>
	// TaoTie.DictionaryComponent<int,object>
	// TaoTie.DictionaryComponent<long,TaoTie.SensibleInfo>
	// TaoTie.DictionaryComponent<long,object>
	// TaoTie.DictionaryComponent<object,object>
	// TaoTie.ETAsyncTaskMethodBuilder<byte>
	// TaoTie.ETAsyncTaskMethodBuilder<int>
	// TaoTie.ETAsyncTaskMethodBuilder<object>
	// TaoTie.ETTask<byte>
	// TaoTie.ETTask<int>
	// TaoTie.ETTask<object>
	// TaoTie.HashSetComponent<byte>
	// TaoTie.HashSetComponent<int>
	// TaoTie.HashSetComponent<object>
	// TaoTie.IManager<TaoTie.UILayerDefine,object>
	// TaoTie.IManager<object,object,object>
	// TaoTie.IManager<object,object>
	// TaoTie.IManager<object>
	// TaoTie.LinkedListComponent<object>
	// TaoTie.ListComponent<UnityEngine.Vector3>
	// TaoTie.ListComponent<int>
	// TaoTie.ListComponent<long>
	// TaoTie.ListComponent<object>
	// TaoTie.LruCache.<System-Collections-Generic-IEnumerable<System-Collections-Generic-KeyValuePair<TKey,TValue>>-GetEnumerator>d__33<object,object>
	// TaoTie.LruCache.<System-Collections-IEnumerable-GetEnumerator>d__32<object,object>
	// TaoTie.LruCache<object,object>
	// TaoTie.MultiMap<long,TaoTie.CoroutineLockTimer>
	// TaoTie.MultiMapSet<int,object>
	// TaoTie.PriorityStack.<System-Collections-Generic-IEnumerable<T>-GetEnumerator>d__15<object>
	// TaoTie.PriorityStack.<System-Collections-IEnumerable-GetEnumerator>d__14<object>
	// TaoTie.PriorityStack<object>
	// TaoTie.TypeInfo<object>
	// TaoTie.UnOrderDoubleKeyDictionary<long,int,System.ValueTuple<long,int,object>>
	// TaoTie.UnOrderDoubleKeyDictionary<object,object,object>
	// TaoTie.UnOrderDoubleKeyDictionary<uint,uint,object>
	// TaoTie.UnOrderMultiMap<int,object>
	// TaoTie.UnOrderMultiMap<long,TaoTie.HitInfo>
	// TaoTie.UnOrderMultiMap<object,object>
	// TaoTie.UnOrderMultiMap<uint,object>
	// TaoTie.UnOrderMultiMapSet<uint,uint>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<int>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<int>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// object LitJson.JsonMapper.ToObject<object>(string)
		// object Nino.Serialization.Deserializer.Deserialize<object>(System.Span<byte>,Nino.Serialization.Reader,Nino.Serialization.CompressOption,bool)
		// object Nino.Serialization.Deserializer.Deserialize<object>(byte[],Nino.Serialization.CompressOption)
		// bool Nino.Serialization.Deserializer.TryDeserializeCodeGenType<object>(System.Type,Nino.Serialization.Reader,bool,bool,object&)
		// bool Nino.Serialization.Deserializer.TryDeserializeWrapperType<object>(System.Type,Nino.Serialization.Reader,bool,bool,object&)
		// System.Void Sirenix.Utilities.LinqExtensions.AddRange<int>(System.Collections.Generic.HashSet<int>,System.Collections.Generic.IEnumerable<int>)
		// object System.Activator.CreateInstance<object>()
		// byte[] System.Array.Empty<byte>()
		// object[] System.Array.Empty<object>()
		// bool System.Enum.TryParse<int>(string,bool,int&)
		// bool System.Enum.TryParse<int>(string,int&)
		// bool System.Linq.Enumerable.Contains<int>(System.Collections.Generic.IEnumerable<int>,int)
		// bool System.Linq.Enumerable.Contains<int>(System.Collections.Generic.IEnumerable<int>,int,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigAIDecisionTreeCategory.<LoadAsync>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigAIDecisionTreeCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigAbilityCategory.<LoadAsync>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigAbilityCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigLoader.<LoadConfigBytes>d__1>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigLoader.<LoadConfigBytes>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigManager.<LoadAsync>d__12>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigManager.<LoadAsync>d__12&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigSceneGroupCategory.<LoadAsync>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigSceneGroupCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigStoryCategory.<LoadAsync>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigStoryCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.ConfigBornType.<AfterBorn>d__4>(TaoTie.ETTaskCompleted&,TaoTie.ConfigBornType.<AfterBorn>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.ConfigStoryActor.<Preload>d__1>(TaoTie.ETTaskCompleted&,TaoTie.ConfigStoryActor.<Preload>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnComplete>d__7>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnComplete>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnCreate>d__3>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnCreate>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnEnter>d__4>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnEnter>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnLeave>d__5>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnLeave>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnPrepare>d__6>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnPrepare>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<OnSwitchSceneEnd>d__10>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<OnSwitchSceneEnd>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoadingScene.<SetProgress>d__8>(TaoTie.ETTaskCompleted&,TaoTie.LoadingScene.<SetProgress>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoginScene.<OnComplete>d__9>(TaoTie.ETTaskCompleted&,TaoTie.LoginScene.<OnComplete>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoginScene.<OnCreate>d__5>(TaoTie.ETTaskCompleted&,TaoTie.LoginScene.<OnCreate>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoginScene.<OnLeave>d__7>(TaoTie.ETTaskCompleted&,TaoTie.LoginScene.<OnLeave>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoginScene.<OnPrepare>d__8>(TaoTie.ETTaskCompleted&,TaoTie.LoginScene.<OnPrepare>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.LoginScene.<SetProgress>d__10>(TaoTie.ETTaskCompleted&,TaoTie.LoginScene.<SetProgress>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.MapScene.<OnComplete>d__17>(TaoTie.ETTaskCompleted&,TaoTie.MapScene.<OnComplete>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.MapScene.<OnCreate>d__13>(TaoTie.ETTaskCompleted&,TaoTie.MapScene.<OnCreate>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.MapScene.<OnLeave>d__15>(TaoTie.ETTaskCompleted&,TaoTie.MapScene.<OnLeave>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.MapScene.<SetProgress>d__18>(TaoTie.ETTaskCompleted&,TaoTie.MapScene.<SetProgress>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.AIPathfindingUpdater.<FindNavmeshTask>d__1>(object&,TaoTie.AIPathfindingUpdater.<FindNavmeshTask>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.AttachAbilityAction.<OnBreakAsync>d__5>(object&,TaoTie.AttachAbilityAction.<OnBreakAsync>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.AttachEffect.<ExecuteAsync>d__4>(object&,TaoTie.AttachEffect.<ExecuteAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.Avatar.<InitAsync>d__4>(object&,TaoTie.Avatar.<InitAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.BillboardComponent.<InitInternal>d__18>(object&,TaoTie.BillboardComponent.<InitInternal>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.BillboardPrefabPlugin.<LoadObj>d__7<object>>(object&,TaoTie.BillboardPrefabPlugin.<LoadObj>d__7<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.BillboardSystem.<PreloadLoadAsset>d__6>(object&,TaoTie.BillboardSystem.<PreloadLoadAsset>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.CameraManager.<LoadAsync>d__6>(object&,TaoTie.CameraManager.<LoadAsync>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigAIBetaCategory.<LoadAsync>d__7>(object&,TaoTie.ConfigAIBetaCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigAIBetaCategory.<LoadOneAsync>d__8>(object&,TaoTie.ConfigAIBetaCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigActorCategory.<LoadAsync>d__7>(object&,TaoTie.ConfigActorCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigActorCategory.<LoadOneAsync>d__8>(object&,TaoTie.ConfigActorCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigBornByAttachPoint.<AfterBorn>d__4>(object&,TaoTie.ConfigBornByAttachPoint.<AfterBorn>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigCommonDialogClip.<Process>d__6>(object&,TaoTie.ConfigCommonDialogClip.<Process>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigFsmControllerCategory.<LoadAsync>d__7>(object&,TaoTie.ConfigFsmControllerCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigFsmControllerCategory.<LoadOneAsync>d__8>(object&,TaoTie.ConfigFsmControllerCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigLoader.<GetAllConfigBytes>d__0>(object&,TaoTie.ConfigLoader.<GetAllConfigBytes>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigManager.<LoadAsync>d__12>(object&,TaoTie.ConfigManager.<LoadAsync>d__12&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigSceneGroupDelayAction.<ExecuteAsync>d__4>(object&,TaoTie.ConfigSceneGroupDelayAction.<ExecuteAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryBranchClip.<Process>d__1>(object&,TaoTie.ConfigStoryBranchClip.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryChangeInputState.<Process>d__4>(object&,TaoTie.ConfigStoryChangeInputState.<Process>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryCharacterActor.<Preload>d__2>(object&,TaoTie.ConfigStoryCharacterActor.<Preload>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryParallelClip.<Process>d__2>(object&,TaoTie.ConfigStoryParallelClip.<Process>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStorySerialClip.<Process>d__1>(object&,TaoTie.ConfigStorySerialClip.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryTimeLine.<Process>d__3>(object&,TaoTie.ConfigStoryTimeLine.<Process>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryWaitTimeClip.<Process>d__2>(object&,TaoTie.ConfigStoryWaitTimeClip.<Process>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.DamageText.<OnInit>d__13>(object&,TaoTie.DamageText.<OnInit>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.Effect.<InitViewAsync>d__18>(object&,TaoTie.Effect.<InitViewAsync>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.Entry.<StartAsync>d__1>(object&,TaoTie.Entry.<StartAsync>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.Entry.<StartGameAsync>d__3>(object&,TaoTie.Entry.<StartGameAsync>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.EnvironmentManager.<LoadAsync>d__44>(object&,TaoTie.EnvironmentManager.<LoadAsync>d__44&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.EquipHoldComponent.<AddEquip>d__9>(object&,TaoTie.EquipHoldComponent.<AddEquip>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.ExecuteAbilityAction.<OnBreakAsync>d__5>(object&,TaoTie.ExecuteAbilityAction.<OnBreakAsync>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectHolderComponent.<EnableHitBox>d__29>(object&,TaoTie.GameObjectHolderComponent.<EnableHitBox>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectHolderComponent.<EnableRenderer>d__28>(object&,TaoTie.GameObjectHolderComponent.<EnableRenderer>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__20>(object&,TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__21>(object&,TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectPoolManager.<CheckAfter>d__34>(object&,TaoTie.GameObjectPoolManager.<CheckAfter>d__34&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectPoolManager.<LoadDependency>d__17>(object&,TaoTie.GameObjectPoolManager.<LoadDependency>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectPoolManager.<PreLoadGameObjectAsync>d__19>(object&,TaoTie.GameObjectPoolManager.<PreLoadGameObjectAsync>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.I18NManager.<InitAsync>d__11>(object&,TaoTie.I18NManager.<InitAsync>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.I18NManager.<SwitchLanguage>d__16>(object&,TaoTie.I18NManager.<SwitchLanguage>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.LoginScene.<OnEnter>d__6>(object&,TaoTie.LoginScene.<OnEnter>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.LoginScene.<OnSwitchSceneEnd>d__13>(object&,TaoTie.LoginScene.<OnSwitchSceneEnd>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.MapScene.<OnEnter>d__14>(object&,TaoTie.MapScene.<OnEnter>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.MapScene.<OnPrepare>d__16>(object&,TaoTie.MapScene.<OnPrepare>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.MapScene.<OnSwitchSceneEnd>d__19>(object&,TaoTie.MapScene.<OnSwitchSceneEnd>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.MaterialManager.<PreLoadMaterial>d__8>(object&,TaoTie.MaterialManager.<PreLoadMaterial>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.MoveComponent.<InitAsync>d__30>(object&,TaoTie.MoveComponent.<InitAsync>d__30&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.RemoveAbility.<ExecuteLater>d__2>(object&,TaoTie.RemoveAbility.<ExecuteLater>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.RemoveModifier.<ExecuteLater>d__2>(object&,TaoTie.RemoveModifier.<ExecuteLater>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SceneManager.<InnerSwitchScene>d__16<object>>(object&,TaoTie.SceneManager.<InnerSwitchScene>d__16<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SceneManager.<SwitchMapScene>d__18>(object&,TaoTie.SceneManager.<SwitchMapScene>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SceneManager.<SwitchScene>d__17<object>>(object&,TaoTie.SceneManager.<SwitchScene>d__17<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<CoPlayMusic>d__27>(object&,TaoTie.SoundManager.<CoPlayMusic>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<InitAsync>d__16>(object&,TaoTie.SoundManager.<InitAsync>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<PlayHttpAudio>d__43>(object&,TaoTie.SoundManager.<PlayHttpAudio>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<PlaySoundAsync>d__32>(object&,TaoTie.SoundManager.<PlaySoundAsync>d__32&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<PlaySoundAsync>d__34>(object&,TaoTie.SoundManager.<PlaySoundAsync>d__34&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<PoolPlay>d__38>(object&,TaoTie.SoundManager.<PoolPlay>d__38&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<RecycleClipSource>d__36>(object&,TaoTie.SoundManager.<RecycleClipSource>d__36&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.StorySystem.<PlayStory>d__16>(object&,TaoTie.StorySystem.<PlayStory>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.StorySystem.<PlayTimeLine>d__17>(object&,TaoTie.StorySystem.<PlayTimeLine>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.StoryTimeLineRunner.<Init>d__23>(object&,TaoTie.StoryTimeLineRunner.<Init>d__23&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.StoryTimeLineRunner.<SetBinding>d__26>(object&,TaoTie.StoryTimeLineRunner.<SetBinding>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIBaseView.<CloseSelf>d__2>(object&,TaoTie.UIBaseView.<CloseSelf>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIButton.<SetBtnGray>d__13>(object&,TaoTie.UIButton.<SetBtnGray>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIButton.<SetSpritePath>d__15>(object&,TaoTie.UIButton.<SetSpritePath>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UICommonStoryDialog.<Play>d__8>(object&,TaoTie.UICommonStoryDialog.<Play>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIImage.<SetImageGray>d__19>(object&,TaoTie.UIImage.<SetImageGray>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIImage.<SetOnlineSpritePath>d__11>(object&,TaoTie.UIImage.<SetOnlineSpritePath>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIImage.<SetSpritePath>d__10>(object&,TaoTie.UIImage.<SetSpritePath>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<AddWindowToStack>d__61>(object&,TaoTie.UIManager.<AddWindowToStack>d__61&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<AddWindowToStack>d__62<object>>(object&,TaoTie.UIManager.<AddWindowToStack>d__62<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<AddWindowToStack>d__63<object,object>>(object&,TaoTie.UIManager.<AddWindowToStack>d__63<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<AddWindowToStack>d__64<object,object,object>>(object&,TaoTie.UIManager.<AddWindowToStack>d__64<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<AddWindowToStack>d__65<object,object,object,object>>(object&,TaoTie.UIManager.<AddWindowToStack>d__65<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<CloseWindow>d__21>(object&,TaoTie.UIManager.<CloseWindow>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<CloseWindow>d__22<object>>(object&,TaoTie.UIManager.<CloseWindow>d__22<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<CloseWindow>d__23>(object&,TaoTie.UIManager.<CloseWindow>d__23&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<CloseWindowByLayer>d__24>(object&,TaoTie.UIManager.<CloseWindowByLayer>d__24&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyAllWindow>d__44>(object&,TaoTie.UIManager.<DestroyAllWindow>d__44&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyUnShowWindow>d__27>(object&,TaoTie.UIManager.<DestroyUnShowWindow>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyWindow>d__25<object>>(object&,TaoTie.UIManager.<DestroyWindow>d__25<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyWindow>d__26>(object&,TaoTie.UIManager.<DestroyWindow>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyWindowByLayer>d__43>(object&,TaoTie.UIManager.<DestroyWindowByLayer>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyWindowExceptLayer>d__42>(object&,TaoTie.UIManager.<DestroyWindowExceptLayer>d__42&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<DestroyWindowExceptNames>d__41>(object&,TaoTie.UIManager.<DestroyWindowExceptNames>d__41&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindowGetGameObject>d__58>(object&,TaoTie.UIManager.<InnerOpenWindowGetGameObject>d__58&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OnDestroyAsync>d__14>(object&,TaoTie.UIManager.<OnDestroyAsync>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindowTask>d__36<object>>(object&,TaoTie.UIManager.<OpenWindowTask>d__36<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindowTask>d__37<object,object>>(object&,TaoTie.UIManager.<OpenWindowTask>d__37<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindowTask>d__38<object,object,object>>(object&,TaoTie.UIManager.<OpenWindowTask>d__38<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindowTask>d__39<object,object,object,object>>(object&,TaoTie.UIManager.<OpenWindowTask>d__39<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindowTask>d__40<object,object,object,object,object>>(object&,TaoTie.UIManager.<OpenWindowTask>d__40<object,object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIMsgBoxManager.<CloseMsgBox>d__7>(object&,TaoTie.UIMsgBoxManager.<CloseMsgBox>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIMsgBoxManager.<OpenMsgBox>d__6<object>>(object&,TaoTie.UIMsgBoxManager.<OpenMsgBox>d__6<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIRawImage.<SetImageGray>d__17>(object&,TaoTie.UIRawImage.<SetImageGray>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIRawImage.<SetOnlineTexturePath>d__12>(object&,TaoTie.UIRawImage.<SetOnlineTexturePath>d__12&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIRawImage.<SetSpritePath>d__11>(object&,TaoTie.UIRawImage.<SetSpritePath>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIToastManager.<ShowToast>d__4>(object&,TaoTie.UIToastManager.<ShowToast>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIUpdateView.<StartCheckUpdate>d__8>(object&,TaoTie.UIUpdateView.<StartCheckUpdate>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UIUpdateView.<UpdateFinishAndStartGame>d__10>(object&,TaoTie.UIUpdateView.<UpdateFinishAndStartGame>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,TaoTie.UpdateTask.<Init>d__9>(object&,TaoTie.UpdateTask.<Init>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectHolderComponent.<WaitLoadGameObjectOver>d__26>(object&,TaoTie.GameObjectHolderComponent.<WaitLoadGameObjectOver>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,TaoTie.NavmeshSystem.<Find>d__11>(object&,TaoTie.NavmeshSystem.<Find>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,TaoTie.PathfindingComponent.<Find>d__3>(object&,TaoTie.PathfindingComponent.<Find>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,TaoTie.UpdateTask.<ShowMsgBoxView>d__13>(object&,TaoTie.UpdateTask.<ShowMsgBoxView>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.MainPackageUpdateProcess.<Process>d__1>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.MainPackageUpdateProcess.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.OtherPackageUpdateProcess.<Process>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.OtherPackageUpdateProcess.<Process>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.UpdateIsSHProcess.<Process>d__0>(TaoTie.ETTaskCompleted&,TaoTie.UpdateIsSHProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.AppUpdateProcess.<Process>d__0>(object&,TaoTie.AppUpdateProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.MainPackageUpdateProcess.<Process>d__1>(object&,TaoTie.MainPackageUpdateProcess.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2>(object&,TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.MainPackageUpdateProcess.<UpdateFail>d__3>(object&,TaoTie.MainPackageUpdateProcess.<UpdateFail>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.OtherPackageUpdateProcess.<Process>d__7>(object&,TaoTie.OtherPackageUpdateProcess.<Process>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9>(object&,TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.OtherPackageUpdateProcess.<UpdateFail>d__8>(object&,TaoTie.OtherPackageUpdateProcess.<UpdateFail>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.SetUpdateListProcess.<Process>d__0>(object&,TaoTie.SetUpdateListProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.SetWhiteListProcess.<Process>d__0>(object&,TaoTie.SetWhiteListProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.UIBranchStoryDialog.<WaitChoose>d__10>(object&,TaoTie.UIBranchStoryDialog.<WaitChoose>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,TaoTie.UpdateTask.<Process>d__10>(object&,TaoTie.UpdateTask.<Process>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TaoTie.ConfigLoader.<GetOneConfigBytes>d__2>(System.Runtime.CompilerServices.TaskAwaiter&,TaoTie.ConfigLoader.<GetOneConfigBytes>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.ConfigStoryCameraActor.<Get3dObj>d__2>(TaoTie.ETTaskCompleted&,TaoTie.ConfigStoryCameraActor.<Get3dObj>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<TaoTie.ETTaskCompleted,TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1>(TaoTie.ETTaskCompleted&,TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.CameraManager.<GetConfig>d__25>(object&,TaoTie.CameraManager.<GetConfig>d__25&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ConfigManager.<LoadOneConfig>d__11<object>>(object&,TaoTie.ConfigManager.<LoadOneConfig>d__11<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryCharacterActor.<Get3dObj>d__3>(object&,TaoTie.ConfigStoryCharacterActor.<Get3dObj>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStoryPlayerActor.<Get3dObj>d__0>(object&,TaoTie.ConfigStoryPlayerActor.<Get3dObj>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1>(object&,TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.CoroutineLockManager.<Wait>d__17>(object&,TaoTie.CoroutineLockManager.<Wait>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.EnvironmentManager.<GetConfig>d__43>(object&,TaoTie.EnvironmentManager.<GetConfig>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.GameObjectPoolManager.<GetGameObjectAsync>d__21>(object&,TaoTie.GameObjectPoolManager.<GetGameObjectAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.HttpManager.<HttpGetImageOnline>d__11>(object&,TaoTie.HttpManager.<HttpGetImageOnline>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.HttpManager.<HttpGetResult>d__15>(object&,TaoTie.HttpManager.<HttpGetResult>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.HttpManager.<HttpGetResult>d__16<object>>(object&,TaoTie.HttpManager.<HttpGetResult>d__16<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.HttpManager.<HttpGetSoundOnline>d__14>(object&,TaoTie.HttpManager.<HttpGetSoundOnline>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.HttpManager.<HttpPostResult>d__17<object>>(object&,TaoTie.HttpManager.<HttpPostResult>d__17<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<GetOnlineSprite>d__28>(object&,TaoTie.ImageLoaderManager.<GetOnlineSprite>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<GetOnlineTexture>d__29>(object&,TaoTie.ImageLoaderManager.<GetOnlineTexture>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadAtlasImageAsync>d__19>(object&,TaoTie.ImageLoaderManager.<LoadAtlasImageAsync>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadAtlasImageAsyncInternal>d__21>(object&,TaoTie.ImageLoaderManager.<LoadAtlasImageAsyncInternal>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadImageAsync>d__17>(object&,TaoTie.ImageLoaderManager.<LoadImageAsync>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadSingleImageAsync>d__20>(object&,TaoTie.ImageLoaderManager.<LoadSingleImageAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadSingleImageAsyncInternal>d__22>(object&,TaoTie.ImageLoaderManager.<LoadSingleImageAsyncInternal>d__22&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ImageLoaderManager.<LoadSpriteImageAsyncInternal>d__24>(object&,TaoTie.ImageLoaderManager.<LoadSpriteImageAsyncInternal>d__24&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.MaterialManager.<LoadMaterialAsync>d__9>(object&,TaoTie.MaterialManager.<LoadMaterialAsync>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.NavmeshSystem.<Load>d__10>(object&,TaoTie.NavmeshSystem.<Load>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ResourcesManager.<LoadConfigBytesAsync>d__21>(object&,TaoTie.ResourcesManager.<LoadConfigBytesAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.ResourcesManager.<LoadConfigJsonAsync>d__20>(object&,TaoTie.ResourcesManager.<LoadConfigJsonAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.SceneManager.<GetScene>d__15<object>>(object&,TaoTie.SceneManager.<GetScene>d__15<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<CreateClipSource>d__18>(object&,TaoTie.SoundManager.<CreateClipSource>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<GetClipSource>d__21>(object&,TaoTie.SoundManager.<GetClipSource>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<GetOnlineClip>d__39>(object&,TaoTie.SoundManager.<GetOnlineClip>d__39&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.SoundManager.<PoolPlayAndReturnSource>d__37>(object&,TaoTie.SoundManager.<PoolPlayAndReturnSource>d__37&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.StorySystem.<Get3dActor>d__18>(object&,TaoTie.StorySystem.<Get3dActor>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.StoryTimeLineRunner.<Get3dActor>d__27>(object&,TaoTie.StoryTimeLineRunner.<Get3dActor>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__51>(object&,TaoTie.UIManager.<InnerOpenWindow>d__51&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__52<object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__52<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__53<object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__53<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__54<object,object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__54<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__55<object,object,object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__55<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__56<object,object,object,object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__56<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<InnerOpenWindow>d__57<object,object,object,object,object>>(object&,TaoTie.UIManager.<InnerOpenWindow>d__57<object,object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__28>(object&,TaoTie.UIManager.<OpenWindow>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__29>(object&,TaoTie.UIManager.<OpenWindow>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__30<object>>(object&,TaoTie.UIManager.<OpenWindow>d__30<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__31<object>>(object&,TaoTie.UIManager.<OpenWindow>d__31<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__32<object,object>>(object&,TaoTie.UIManager.<OpenWindow>d__32<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__33<object,object,object>>(object&,TaoTie.UIManager.<OpenWindow>d__33<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__34<object,object,object,object>>(object&,TaoTie.UIManager.<OpenWindow>d__34<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,TaoTie.UIManager.<OpenWindow>d__35<object,object,object,object,object>>(object&,TaoTie.UIManager.<OpenWindow>d__35<object,object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.AIPathfindingUpdater.<FindNavmeshTask>d__1>(TaoTie.AIPathfindingUpdater.<FindNavmeshTask>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.AttachAbilityAction.<OnBreakAsync>d__5>(TaoTie.AttachAbilityAction.<OnBreakAsync>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.AttachEffect.<ExecuteAsync>d__4>(TaoTie.AttachEffect.<ExecuteAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.Avatar.<InitAsync>d__4>(TaoTie.Avatar.<InitAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.BillboardComponent.<InitInternal>d__18>(TaoTie.BillboardComponent.<InitInternal>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.BillboardPrefabPlugin.<LoadObj>d__7<object>>(TaoTie.BillboardPrefabPlugin.<LoadObj>d__7<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.BillboardSystem.<PreloadLoadAsset>d__6>(TaoTie.BillboardSystem.<PreloadLoadAsset>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.CameraManager.<LoadAsync>d__6>(TaoTie.CameraManager.<LoadAsync>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigAIBetaCategory.<LoadAsync>d__7>(TaoTie.ConfigAIBetaCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigAIBetaCategory.<LoadOneAsync>d__8>(TaoTie.ConfigAIBetaCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigAIDecisionTreeCategory.<LoadAsync>d__7>(TaoTie.ConfigAIDecisionTreeCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigAbilityCategory.<LoadAsync>d__7>(TaoTie.ConfigAbilityCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigActorCategory.<LoadAsync>d__7>(TaoTie.ConfigActorCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigActorCategory.<LoadOneAsync>d__8>(TaoTie.ConfigActorCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigBornByAttachPoint.<AfterBorn>d__4>(TaoTie.ConfigBornByAttachPoint.<AfterBorn>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigBornType.<AfterBorn>d__4>(TaoTie.ConfigBornType.<AfterBorn>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigCommonDialogClip.<Process>d__6>(TaoTie.ConfigCommonDialogClip.<Process>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigFsmControllerCategory.<LoadAsync>d__7>(TaoTie.ConfigFsmControllerCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigFsmControllerCategory.<LoadOneAsync>d__8>(TaoTie.ConfigFsmControllerCategory.<LoadOneAsync>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigLoader.<GetAllConfigBytes>d__0>(TaoTie.ConfigLoader.<GetAllConfigBytes>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigLoader.<LoadConfigBytes>d__1>(TaoTie.ConfigLoader.<LoadConfigBytes>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigManager.<LoadAsync>d__12>(TaoTie.ConfigManager.<LoadAsync>d__12&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigSceneGroupCategory.<LoadAsync>d__7>(TaoTie.ConfigSceneGroupCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigSceneGroupDelayAction.<ExecuteAsync>d__4>(TaoTie.ConfigSceneGroupDelayAction.<ExecuteAsync>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryActor.<Preload>d__1>(TaoTie.ConfigStoryActor.<Preload>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryBranchClip.<Process>d__1>(TaoTie.ConfigStoryBranchClip.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryCategory.<LoadAsync>d__7>(TaoTie.ConfigStoryCategory.<LoadAsync>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryChangeInputState.<Process>d__4>(TaoTie.ConfigStoryChangeInputState.<Process>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryCharacterActor.<Preload>d__2>(TaoTie.ConfigStoryCharacterActor.<Preload>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryParallelClip.<Process>d__2>(TaoTie.ConfigStoryParallelClip.<Process>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStorySerialClip.<Process>d__1>(TaoTie.ConfigStorySerialClip.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryTimeLine.<Process>d__3>(TaoTie.ConfigStoryTimeLine.<Process>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ConfigStoryWaitTimeClip.<Process>d__2>(TaoTie.ConfigStoryWaitTimeClip.<Process>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.DamageText.<OnInit>d__13>(TaoTie.DamageText.<OnInit>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ETTaskHelper.<WaitAll>d__6<object>>(TaoTie.ETTaskHelper.<WaitAll>d__6<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.Effect.<InitViewAsync>d__18>(TaoTie.Effect.<InitViewAsync>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.Entry.<StartAsync>d__1>(TaoTie.Entry.<StartAsync>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.Entry.<StartGameAsync>d__3>(TaoTie.Entry.<StartGameAsync>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.EnvironmentManager.<LoadAsync>d__44>(TaoTie.EnvironmentManager.<LoadAsync>d__44&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.EquipHoldComponent.<AddEquip>d__9>(TaoTie.EquipHoldComponent.<AddEquip>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.ExecuteAbilityAction.<OnBreakAsync>d__5>(TaoTie.ExecuteAbilityAction.<OnBreakAsync>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectHolderComponent.<EnableHitBox>d__29>(TaoTie.GameObjectHolderComponent.<EnableHitBox>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectHolderComponent.<EnableRenderer>d__28>(TaoTie.GameObjectHolderComponent.<EnableRenderer>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__20>(TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__21>(TaoTie.GameObjectHolderComponent.<LoadGameObjectAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectPoolManager.<CheckAfter>d__34>(TaoTie.GameObjectPoolManager.<CheckAfter>d__34&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectPoolManager.<LoadDependency>d__17>(TaoTie.GameObjectPoolManager.<LoadDependency>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.GameObjectPoolManager.<PreLoadGameObjectAsync>d__19>(TaoTie.GameObjectPoolManager.<PreLoadGameObjectAsync>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.I18NManager.<InitAsync>d__11>(TaoTie.I18NManager.<InitAsync>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.I18NManager.<SwitchLanguage>d__16>(TaoTie.I18NManager.<SwitchLanguage>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnComplete>d__7>(TaoTie.LoadingScene.<OnComplete>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnCreate>d__3>(TaoTie.LoadingScene.<OnCreate>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnEnter>d__4>(TaoTie.LoadingScene.<OnEnter>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnLeave>d__5>(TaoTie.LoadingScene.<OnLeave>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnPrepare>d__6>(TaoTie.LoadingScene.<OnPrepare>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<OnSwitchSceneEnd>d__10>(TaoTie.LoadingScene.<OnSwitchSceneEnd>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoadingScene.<SetProgress>d__8>(TaoTie.LoadingScene.<SetProgress>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnComplete>d__9>(TaoTie.LoginScene.<OnComplete>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnCreate>d__5>(TaoTie.LoginScene.<OnCreate>d__5&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnEnter>d__6>(TaoTie.LoginScene.<OnEnter>d__6&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnLeave>d__7>(TaoTie.LoginScene.<OnLeave>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnPrepare>d__8>(TaoTie.LoginScene.<OnPrepare>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<OnSwitchSceneEnd>d__13>(TaoTie.LoginScene.<OnSwitchSceneEnd>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.LoginScene.<SetProgress>d__10>(TaoTie.LoginScene.<SetProgress>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnComplete>d__17>(TaoTie.MapScene.<OnComplete>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnCreate>d__13>(TaoTie.MapScene.<OnCreate>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnEnter>d__14>(TaoTie.MapScene.<OnEnter>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnLeave>d__15>(TaoTie.MapScene.<OnLeave>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnPrepare>d__16>(TaoTie.MapScene.<OnPrepare>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<OnSwitchSceneEnd>d__19>(TaoTie.MapScene.<OnSwitchSceneEnd>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MapScene.<SetProgress>d__18>(TaoTie.MapScene.<SetProgress>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MaterialManager.<PreLoadMaterial>d__8>(TaoTie.MaterialManager.<PreLoadMaterial>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.Messager.<BroadcastNextFrame>d__23<object>>(TaoTie.Messager.<BroadcastNextFrame>d__23<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.MoveComponent.<InitAsync>d__30>(TaoTie.MoveComponent.<InitAsync>d__30&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.RemoveAbility.<ExecuteLater>d__2>(TaoTie.RemoveAbility.<ExecuteLater>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.RemoveModifier.<ExecuteLater>d__2>(TaoTie.RemoveModifier.<ExecuteLater>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SceneManager.<InnerSwitchScene>d__16<object>>(TaoTie.SceneManager.<InnerSwitchScene>d__16<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SceneManager.<SwitchMapScene>d__18>(TaoTie.SceneManager.<SwitchMapScene>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SceneManager.<SwitchScene>d__17<object>>(TaoTie.SceneManager.<SwitchScene>d__17<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<CoPlayMusic>d__27>(TaoTie.SoundManager.<CoPlayMusic>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<InitAsync>d__16>(TaoTie.SoundManager.<InitAsync>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<PlayHttpAudio>d__43>(TaoTie.SoundManager.<PlayHttpAudio>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<PlaySoundAsync>d__32>(TaoTie.SoundManager.<PlaySoundAsync>d__32&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<PlaySoundAsync>d__34>(TaoTie.SoundManager.<PlaySoundAsync>d__34&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<PoolPlay>d__38>(TaoTie.SoundManager.<PoolPlay>d__38&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.SoundManager.<RecycleClipSource>d__36>(TaoTie.SoundManager.<RecycleClipSource>d__36&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.StorySystem.<PlayStory>d__16>(TaoTie.StorySystem.<PlayStory>d__16&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.StorySystem.<PlayTimeLine>d__17>(TaoTie.StorySystem.<PlayTimeLine>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.StoryTimeLineRunner.<Init>d__23>(TaoTie.StoryTimeLineRunner.<Init>d__23&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.StoryTimeLineRunner.<SetBinding>d__26>(TaoTie.StoryTimeLineRunner.<SetBinding>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIBaseView.<CloseSelf>d__2>(TaoTie.UIBaseView.<CloseSelf>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIButton.<SetBtnGray>d__13>(TaoTie.UIButton.<SetBtnGray>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIButton.<SetSpritePath>d__15>(TaoTie.UIButton.<SetSpritePath>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UICommonStoryDialog.<Play>d__8>(TaoTie.UICommonStoryDialog.<Play>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIImage.<SetImageGray>d__19>(TaoTie.UIImage.<SetImageGray>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIImage.<SetOnlineSpritePath>d__11>(TaoTie.UIImage.<SetOnlineSpritePath>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIImage.<SetSpritePath>d__10>(TaoTie.UIImage.<SetSpritePath>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<AddWindowToStack>d__61>(TaoTie.UIManager.<AddWindowToStack>d__61&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<AddWindowToStack>d__62<object>>(TaoTie.UIManager.<AddWindowToStack>d__62<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<AddWindowToStack>d__63<object,object>>(TaoTie.UIManager.<AddWindowToStack>d__63<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<AddWindowToStack>d__64<object,object,object>>(TaoTie.UIManager.<AddWindowToStack>d__64<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<AddWindowToStack>d__65<object,object,object,object>>(TaoTie.UIManager.<AddWindowToStack>d__65<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<CloseWindow>d__21>(TaoTie.UIManager.<CloseWindow>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<CloseWindow>d__22<object>>(TaoTie.UIManager.<CloseWindow>d__22<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<CloseWindow>d__23>(TaoTie.UIManager.<CloseWindow>d__23&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<CloseWindowByLayer>d__24>(TaoTie.UIManager.<CloseWindowByLayer>d__24&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyAllWindow>d__44>(TaoTie.UIManager.<DestroyAllWindow>d__44&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyUnShowWindow>d__27>(TaoTie.UIManager.<DestroyUnShowWindow>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyWindow>d__25<object>>(TaoTie.UIManager.<DestroyWindow>d__25<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyWindow>d__26>(TaoTie.UIManager.<DestroyWindow>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyWindowByLayer>d__43>(TaoTie.UIManager.<DestroyWindowByLayer>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyWindowExceptLayer>d__42>(TaoTie.UIManager.<DestroyWindowExceptLayer>d__42&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<DestroyWindowExceptNames>d__41>(TaoTie.UIManager.<DestroyWindowExceptNames>d__41&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<InnerOpenWindowGetGameObject>d__58>(TaoTie.UIManager.<InnerOpenWindowGetGameObject>d__58&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OnDestroyAsync>d__14>(TaoTie.UIManager.<OnDestroyAsync>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OpenWindowTask>d__36<object>>(TaoTie.UIManager.<OpenWindowTask>d__36<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OpenWindowTask>d__37<object,object>>(TaoTie.UIManager.<OpenWindowTask>d__37<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OpenWindowTask>d__38<object,object,object>>(TaoTie.UIManager.<OpenWindowTask>d__38<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OpenWindowTask>d__39<object,object,object,object>>(TaoTie.UIManager.<OpenWindowTask>d__39<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIManager.<OpenWindowTask>d__40<object,object,object,object,object>>(TaoTie.UIManager.<OpenWindowTask>d__40<object,object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIMsgBoxManager.<CloseMsgBox>d__7>(TaoTie.UIMsgBoxManager.<CloseMsgBox>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIMsgBoxManager.<OpenMsgBox>d__6<object>>(TaoTie.UIMsgBoxManager.<OpenMsgBox>d__6<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIRawImage.<SetImageGray>d__17>(TaoTie.UIRawImage.<SetImageGray>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIRawImage.<SetOnlineTexturePath>d__12>(TaoTie.UIRawImage.<SetOnlineTexturePath>d__12&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIRawImage.<SetSpritePath>d__11>(TaoTie.UIRawImage.<SetSpritePath>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIToastManager.<ShowToast>d__4>(TaoTie.UIToastManager.<ShowToast>d__4&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIUpdateView.<StartCheckUpdate>d__8>(TaoTie.UIUpdateView.<StartCheckUpdate>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UIUpdateView.<UpdateFinishAndStartGame>d__10>(TaoTie.UIUpdateView.<UpdateFinishAndStartGame>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder.Start<TaoTie.UpdateTask.<Init>d__9>(TaoTie.UpdateTask.<Init>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.Start<TaoTie.GameObjectHolderComponent.<WaitLoadGameObjectOver>d__26>(TaoTie.GameObjectHolderComponent.<WaitLoadGameObjectOver>d__26&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.Start<TaoTie.NavmeshSystem.<Find>d__11>(TaoTie.NavmeshSystem.<Find>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.Start<TaoTie.PathfindingComponent.<Find>d__3>(TaoTie.PathfindingComponent.<Find>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<byte>.Start<TaoTie.UpdateTask.<ShowMsgBoxView>d__13>(TaoTie.UpdateTask.<ShowMsgBoxView>d__13&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.AppUpdateProcess.<Process>d__0>(TaoTie.AppUpdateProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.MainPackageUpdateProcess.<Process>d__1>(TaoTie.MainPackageUpdateProcess.<Process>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2>(TaoTie.MainPackageUpdateProcess.<ResetVersion>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.MainPackageUpdateProcess.<UpdateFail>d__3>(TaoTie.MainPackageUpdateProcess.<UpdateFail>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.OtherPackageUpdateProcess.<Process>d__7>(TaoTie.OtherPackageUpdateProcess.<Process>d__7&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9>(TaoTie.OtherPackageUpdateProcess.<ResetVersion>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.OtherPackageUpdateProcess.<UpdateFail>d__8>(TaoTie.OtherPackageUpdateProcess.<UpdateFail>d__8&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.SetUpdateListProcess.<Process>d__0>(TaoTie.SetUpdateListProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.SetWhiteListProcess.<Process>d__0>(TaoTie.SetWhiteListProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.UIBranchStoryDialog.<WaitChoose>d__10>(TaoTie.UIBranchStoryDialog.<WaitChoose>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.UpdateIsSHProcess.<Process>d__0>(TaoTie.UpdateIsSHProcess.<Process>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<int>.Start<TaoTie.UpdateTask.<Process>d__10>(TaoTie.UpdateTask.<Process>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.CameraManager.<GetConfig>d__25>(TaoTie.CameraManager.<GetConfig>d__25&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigLoader.<GetOneConfigBytes>d__2>(TaoTie.ConfigLoader.<GetOneConfigBytes>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigManager.<LoadOneConfig>d__11<object>>(TaoTie.ConfigManager.<LoadOneConfig>d__11<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigStoryCameraActor.<Get3dObj>d__2>(TaoTie.ConfigStoryCameraActor.<Get3dObj>d__2&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigStoryCharacterActor.<Get3dObj>d__3>(TaoTie.ConfigStoryCharacterActor.<Get3dObj>d__3&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigStoryPlayerActor.<Get3dObj>d__0>(TaoTie.ConfigStoryPlayerActor.<Get3dObj>d__0&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1>(TaoTie.ConfigStorySceneGroupActor.<Get3dObj>d__1&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.CoroutineLockManager.<Wait>d__17>(TaoTie.CoroutineLockManager.<Wait>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.EnvironmentManager.<GetConfig>d__43>(TaoTie.EnvironmentManager.<GetConfig>d__43&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.GameObjectPoolManager.<GetGameObjectAsync>d__21>(TaoTie.GameObjectPoolManager.<GetGameObjectAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.HttpManager.<HttpGetImageOnline>d__11>(TaoTie.HttpManager.<HttpGetImageOnline>d__11&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.HttpManager.<HttpGetResult>d__15>(TaoTie.HttpManager.<HttpGetResult>d__15&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.HttpManager.<HttpGetResult>d__16<object>>(TaoTie.HttpManager.<HttpGetResult>d__16<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.HttpManager.<HttpGetSoundOnline>d__14>(TaoTie.HttpManager.<HttpGetSoundOnline>d__14&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.HttpManager.<HttpPostResult>d__17<object>>(TaoTie.HttpManager.<HttpPostResult>d__17<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<GetOnlineSprite>d__28>(TaoTie.ImageLoaderManager.<GetOnlineSprite>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<GetOnlineTexture>d__29>(TaoTie.ImageLoaderManager.<GetOnlineTexture>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadAtlasImageAsync>d__19>(TaoTie.ImageLoaderManager.<LoadAtlasImageAsync>d__19&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadAtlasImageAsyncInternal>d__21>(TaoTie.ImageLoaderManager.<LoadAtlasImageAsyncInternal>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadImageAsync>d__17>(TaoTie.ImageLoaderManager.<LoadImageAsync>d__17&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadSingleImageAsync>d__20>(TaoTie.ImageLoaderManager.<LoadSingleImageAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadSingleImageAsyncInternal>d__22>(TaoTie.ImageLoaderManager.<LoadSingleImageAsyncInternal>d__22&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ImageLoaderManager.<LoadSpriteImageAsyncInternal>d__24>(TaoTie.ImageLoaderManager.<LoadSpriteImageAsyncInternal>d__24&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.MaterialManager.<LoadMaterialAsync>d__9>(TaoTie.MaterialManager.<LoadMaterialAsync>d__9&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.NavmeshSystem.<Load>d__10>(TaoTie.NavmeshSystem.<Load>d__10&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ResourcesManager.<LoadConfigBytesAsync>d__21>(TaoTie.ResourcesManager.<LoadConfigBytesAsync>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.ResourcesManager.<LoadConfigJsonAsync>d__20>(TaoTie.ResourcesManager.<LoadConfigJsonAsync>d__20&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.SceneManager.<GetScene>d__15<object>>(TaoTie.SceneManager.<GetScene>d__15<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.SoundManager.<CreateClipSource>d__18>(TaoTie.SoundManager.<CreateClipSource>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.SoundManager.<GetClipSource>d__21>(TaoTie.SoundManager.<GetClipSource>d__21&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.SoundManager.<GetOnlineClip>d__39>(TaoTie.SoundManager.<GetOnlineClip>d__39&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.SoundManager.<PoolPlayAndReturnSource>d__37>(TaoTie.SoundManager.<PoolPlayAndReturnSource>d__37&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.StorySystem.<Get3dActor>d__18>(TaoTie.StorySystem.<Get3dActor>d__18&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.StoryTimeLineRunner.<Get3dActor>d__27>(TaoTie.StoryTimeLineRunner.<Get3dActor>d__27&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__51>(TaoTie.UIManager.<InnerOpenWindow>d__51&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__52<object>>(TaoTie.UIManager.<InnerOpenWindow>d__52<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__53<object>>(TaoTie.UIManager.<InnerOpenWindow>d__53<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__54<object,object>>(TaoTie.UIManager.<InnerOpenWindow>d__54<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__55<object,object,object>>(TaoTie.UIManager.<InnerOpenWindow>d__55<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__56<object,object,object,object>>(TaoTie.UIManager.<InnerOpenWindow>d__56<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<InnerOpenWindow>d__57<object,object,object,object,object>>(TaoTie.UIManager.<InnerOpenWindow>d__57<object,object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__28>(TaoTie.UIManager.<OpenWindow>d__28&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__29>(TaoTie.UIManager.<OpenWindow>d__29&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__30<object>>(TaoTie.UIManager.<OpenWindow>d__30<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__31<object>>(TaoTie.UIManager.<OpenWindow>d__31<object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__32<object,object>>(TaoTie.UIManager.<OpenWindow>d__32<object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__33<object,object,object>>(TaoTie.UIManager.<OpenWindow>d__33<object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__34<object,object,object,object>>(TaoTie.UIManager.<OpenWindow>d__34<object,object,object,object>&)
		// System.Void TaoTie.ETAsyncTaskMethodBuilder<object>.Start<TaoTie.UIManager.<OpenWindow>d__35<object,object,object,object,object>>(TaoTie.UIManager.<OpenWindow>d__35<object,object,object,object,object>&)
		// TaoTie.ETTask TaoTie.ETTaskHelper.WaitAll<object>(System.Collections.Generic.List<TaoTie.ETTask<object>>)
		// object TaoTie.JsonHelper.FromJson<object>(string)
		// bool TaoTie.JsonHelper.TryFromJson<object>(string,object&)
		// object TaoTie.ManagerProvider.RegisterManager<object,TaoTie.UILayerDefine,object>(TaoTie.UILayerDefine,object,string)
		// object TaoTie.ManagerProvider.RegisterManager<object,object,object,object>(object,object,object,string)
		// object TaoTie.ManagerProvider.RegisterManager<object,object,object>(object,object,string)
		// object TaoTie.ManagerProvider.RegisterManager<object,object>(object,string)
		// System.Void TaoTie.ManagerProvider.RemoveManager<object>(string)
		// System.Void TaoTie.Messager.AddListener<TaoTie.NumericChange>(long,int,System.Action<TaoTie.NumericChange>)
		// System.Void TaoTie.Messager.AddListener<byte>(long,int,System.Action<byte>)
		// System.Void TaoTie.Messager.AddListener<int,int>(long,int,System.Action<int,int>)
		// System.Void TaoTie.Messager.AddListener<long,byte,object>(long,int,System.Action<long,byte,object>)
		// System.Void TaoTie.Messager.AddListener<object,UnityEngine.Quaternion>(long,int,System.Action<object,UnityEngine.Quaternion>)
		// System.Void TaoTie.Messager.AddListener<object,UnityEngine.Vector3>(long,int,System.Action<object,UnityEngine.Vector3>)
		// System.Void TaoTie.Messager.AddListener<object,byte>(long,int,System.Action<object,byte>)
		// System.Void TaoTie.Messager.AddListener<object,float,int,float>(long,int,System.Action<object,float,int,float>)
		// System.Void TaoTie.Messager.AddListener<object,float>(long,int,System.Action<object,float>)
		// System.Void TaoTie.Messager.AddListener<object,int>(long,int,System.Action<object,int>)
		// System.Void TaoTie.Messager.AddListener<object>(long,int,System.Action<object>)
		// System.Void TaoTie.Messager.Broadcast<TaoTie.NumericChange>(long,int,TaoTie.NumericChange)
		// System.Void TaoTie.Messager.Broadcast<UnityEngine.Vector3,int>(long,int,UnityEngine.Vector3,int)
		// System.Void TaoTie.Messager.Broadcast<byte>(long,int,byte)
		// System.Void TaoTie.Messager.Broadcast<int,int>(long,int,int,int)
		// System.Void TaoTie.Messager.Broadcast<long,byte,object>(long,int,long,byte,object)
		// System.Void TaoTie.Messager.Broadcast<long>(long,int,long)
		// System.Void TaoTie.Messager.Broadcast<object,UnityEngine.Quaternion>(long,int,object,UnityEngine.Quaternion)
		// System.Void TaoTie.Messager.Broadcast<object,UnityEngine.Vector3>(long,int,object,UnityEngine.Vector3)
		// System.Void TaoTie.Messager.Broadcast<object,byte>(long,int,object,byte)
		// System.Void TaoTie.Messager.Broadcast<object,float,int,float>(long,int,object,float,int,float)
		// System.Void TaoTie.Messager.Broadcast<object,float>(long,int,object,float)
		// System.Void TaoTie.Messager.Broadcast<object,int>(long,int,object,int)
		// System.Void TaoTie.Messager.Broadcast<object>(long,int,object)
		// TaoTie.ETTask TaoTie.Messager.BroadcastNextFrame<object>(long,int,object)
		// System.Void TaoTie.Messager.RemoveListener<TaoTie.NumericChange>(long,int,System.Action<TaoTie.NumericChange>)
		// System.Void TaoTie.Messager.RemoveListener<byte>(long,int,System.Action<byte>)
		// System.Void TaoTie.Messager.RemoveListener<int,int>(long,int,System.Action<int,int>)
		// System.Void TaoTie.Messager.RemoveListener<long,byte,object>(long,int,System.Action<long,byte,object>)
		// System.Void TaoTie.Messager.RemoveListener<object,UnityEngine.Quaternion>(long,int,System.Action<object,UnityEngine.Quaternion>)
		// System.Void TaoTie.Messager.RemoveListener<object,UnityEngine.Vector3>(long,int,System.Action<object,UnityEngine.Vector3>)
		// System.Void TaoTie.Messager.RemoveListener<object,byte>(long,int,System.Action<object,byte>)
		// System.Void TaoTie.Messager.RemoveListener<object,float,int,float>(long,int,System.Action<object,float,int,float>)
		// System.Void TaoTie.Messager.RemoveListener<object,float>(long,int,System.Action<object,float>)
		// System.Void TaoTie.Messager.RemoveListener<object,int>(long,int,System.Action<object,int>)
		// System.Void TaoTie.Messager.RemoveListener<object>(long,int,System.Action<object>)
		// YooAsset.AssetHandle TaoTie.PackageManager.LoadAssetAsync<object>(string,string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.Object.Instantiate<object>(object)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
	}
}