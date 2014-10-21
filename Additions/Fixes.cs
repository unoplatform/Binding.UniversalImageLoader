using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections;
using Android.Graphics;

// Fixes to get rid of "does not implement inherited abstract member" errors

namespace Com.Nostra13.Universalimageloader.Cache.Memory.Impl
{
	//partial class FIFOLimitedMemoryCache
	//{
	//	protected override int GetSize(Java.Lang.Object value)
	//	{
	//		return GetSize(value);
	//	}

	//	protected override Java.Lang.Ref.Reference CreateReference(Java.Lang.Object value)
	//	{
	//		return CreateReference(value);
	//	}
	//}

	//partial class LargestLimitedMemoryCache
	//{
	//	protected override int GetSize(Java.Lang.Object value)
	//	{
	//		return GetSize(value);
	//	}

	//	protected override Java.Lang.Ref.Reference CreateReference(Java.Lang.Object value)
	//	{
	//		return CreateReference(value);
	//	}
	//}

	//partial class LRULimitedMemoryCache
	//{
	//	protected override int GetSize(Java.Lang.Object value)
	//	{
	//		return GetSize(value);
	//	}

	//	protected override Java.Lang.Ref.Reference CreateReference(Java.Lang.Object value)
	//	{
	//		return CreateReference(value);
	//	}
	//}

	//partial class UsingFreqLimitedMemoryCache
	//{
	//	protected override int GetSize(Java.Lang.Object value)
	//	{
	//		return GetSize(value);
	//	}

	//	protected override Java.Lang.Ref.Reference CreateReference(Java.Lang.Object value)
	//	{
	//		return CreateReference(value);
	//	}
	//}


	partial class FuzzyKeyMemoryCache
	{
		Java.Lang.Object IMemoryCacheAware.Get(Java.Lang.Object p0)
		{
			if (p0 != null)
			{
				return Get((String)p0);
			}
			else
			{
				return null;
			}
		}

		ICollection IMemoryCacheAware.Keys()
		{
			return (ICollection)Keys();
		}

		bool IMemoryCacheAware.Put(Java.Lang.Object p0, Java.Lang.Object p1)
		{
			return Put((String)p0, (Bitmap)p1);
		}

		Java.Lang.Object IMemoryCacheAware.Remove(Java.Lang.Object p0)
		{
			return Remove((string)p0);
		}
	}

	partial class LimitedAgeMemoryCache
	{
		Java.Lang.Object IMemoryCacheAware.Get(Java.Lang.Object p0)
		{
			if (p0 != null)
			{
				return Get((String)p0);
			}
			else
			{
				return null;
			}
		}

		ICollection IMemoryCacheAware.Keys()
		{
			return (ICollection)Keys();
		}

		bool IMemoryCacheAware.Put(Java.Lang.Object p0, Java.Lang.Object p1)
		{
			return Put((String)p0, (Bitmap)p1);
		}

		Java.Lang.Object IMemoryCacheAware.Remove(Java.Lang.Object p0)
		{
			return Remove((string)p0);
		}

	}

	// Fixing ther error 'Com.Nostra13.Universalimageloader.Cache.Memory.Impl.LruMemoryCache' does not implement interface member 'Com.Nostra13.Universalimageloader.Cache.Memory.IMemoryCacheAware.Keys()'. 'Com.Nostra13.Universalimageloader.Cache.Memory.Impl.LruMemoryCache.Keys()' 
	// cannot implement 'Com.Nostra13.Universalimageloader.Cache.Memory.IMemoryCacheAware.Keys()' because it does not have the matching return type of 'System.Collections.ICollection'. -->

	partial class LruMemoryCache
	{
		Java.Lang.Object IMemoryCacheAware.Get(Java.Lang.Object p0)
		{
			if (p0 != null)
			{
				return Get((String)p0);
			}
			else
			{
				return null;
			}
		}

		ICollection IMemoryCacheAware.Keys()
		{
			return (ICollection)Keys();
		}

		bool IMemoryCacheAware.Put(Java.Lang.Object p0, Java.Lang.Object p1)
		{
			return Put((String)p0, (Bitmap)p1);
		}

		Java.Lang.Object IMemoryCacheAware.Remove(Java.Lang.Object p0)
		{
			return Remove((string)p0);
		}

	}
}

namespace Com.Nostra13.Universalimageloader.Cache.Memory
{
	partial class BaseMemoryCache : IMemoryCache
	{
		Java.Lang.Object IMemoryCacheAware.Get(Java.Lang.Object p0)
		{
			if (p0 != null)
			{
				return Get((String)p0);
			}
			else
			{
				return null;
			}
		}

		ICollection IMemoryCacheAware.Keys()
		{
			return (ICollection)Keys();
		}

		bool IMemoryCacheAware.Put(Java.Lang.Object p0, Java.Lang.Object p1)
		{
			return Put((String)p0, (Bitmap)p1);
		}

		Java.Lang.Object IMemoryCacheAware.Remove(Java.Lang.Object p0)
		{
			return Remove((string)p0);
		}

	}

	abstract partial class BaseMemoryCacheInvoker
	{

	}
}

namespace Com.Nostra13.Universalimageloader.Core.Assist.Deque
{
	partial class LinkedBlockingDeque
	{
		partial class DescendingItr
		{
			public override Java.Lang.Object Next()
			{
				return Next();
			}
		};

		partial class Itr
		{
			public override Java.Lang.Object Next()
			{
				return Next();
			}
		}
	}
}