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
using System.Threading.Tasks;
using Android.Graphics;
using System.Threading;
using System.Drawing;
using Com.Nostra13.Universalimageloader.Core.Listener;
using System.Globalization;
using Com.Nostra13.Universalimageloader.Core.Imageaware;
using Android.Graphics.Drawables;
using Com.Nostra13.Universalimageloader.Core.Assist;

namespace Com.Nostra13.Universalimageloader.Core
{
	public partial class ImageLoader
	{
		public Task<Bitmap> LoadImageAsync(CancellationToken ct, string uri, ImageView imageView, Size? targetSize = null)
		{
			return LoadImageAsync(ct, uri, imageView, targetSize, null);
		}

		public async Task<Bitmap> LoadImageAsync(CancellationToken ct, string uri, ImageView imageView, Size? targetSize, DisplayImageOptions options)
		{
			TaskCompletionSource<Android.Graphics.Bitmap> source = new TaskCompletionSource<Android.Graphics.Bitmap>();

			ct.Register(() => source.TrySetCanceled());

			options = options ?? new DisplayImageOptions.Builder()
				.CacheInMemory(true)
				.CacheOnDisk(true)
				.Build();

			using (var aware =
				imageView != null ?
					new ImageViewAwareCancellable(imageView, ct) :
					null
			)
			{
				using (var listener = new ImageListener(source))
				{
					if (targetSize != null && imageView != null)
					{
						imageView.SetMaxHeight(targetSize.Value.Height);
						imageView.SetMaxWidth(targetSize.Value.Width);

						ImageLoader.Instance.DisplayImage(
							uri,
							aware,
							options,
							listener
						);
					}
					else if (targetSize != null && imageView == null)
					{
						var targetImageSize = new ImageSize(targetSize.Value.Width, targetSize.Value.Height);

						ImageLoader.Instance.LoadImage(
							uri,
							targetImageSize,
							options,
							new ImageListener(source)
						);
					}
					else
					{
						ImageLoader.Instance.LoadImage(
							uri,
							options,
							new ImageListener(source)
						);
					}

					var target = await source.Task;

					return target;
				}
			}
		}

		private class ImageViewAwareCancellable : ImageViewAware
		{
			private readonly CancellationToken _ct;

			public ImageViewAwareCancellable(System.IntPtr ptr, Android.Runtime.JniHandleOwnership ownership)
				: base(ptr, ownership)
			{
				// Force cancellation of the token. This is used when the instance is forcibly re-created 
				// by the runtime
				CancellationTokenSource cts = new CancellationTokenSource();
				_ct = cts.Token;
				cts.Cancel();
			}

			public ImageViewAwareCancellable(global::Android.Widget.ImageView p0, CancellationToken ct) : base(p0)
			{
				//We do not want to Cancel any request since it makes the ImageLoader code misbehave in multiple scenarios.
				_ct = CancellationToken.None;
			}

			public override bool SetImageBitmap(Bitmap p0)
			{
				if (!_ct.IsCancellationRequested)
				{
					return base.SetImageBitmap(p0);
				}

				return false;
			}
			protected override void SetImageBitmapInto(Bitmap p0, View p1)
			{
				if (!_ct.IsCancellationRequested)
				{
					base.SetImageBitmapInto(p0, p1);
				}
			}

			public override bool SetImageDrawable(Drawable p0)
			{
				if (!_ct.IsCancellationRequested)
				{
					return base.SetImageDrawable(p0);
				}

				return false;
			}

			protected override void SetImageDrawableInto(Drawable p0, View p1)
			{
				if (!_ct.IsCancellationRequested)
				{
					base.SetImageDrawableInto(p0, p1);
				}
			}
		}

		private class ImageListener : Java.Lang.Object, IImageLoadingListener
		{
			private TaskCompletionSource<Android.Graphics.Bitmap> _source;

			public ImageListener(System.IntPtr ptr, Android.Runtime.JniHandleOwnership ownership)
				: base(ptr, ownership)
			{
				// Fake source, used when the instance is forcibly re-created 
				// by the runtime when the display is cancelled.
				_source = new TaskCompletionSource<Bitmap>();
			}

			public ImageListener(TaskCompletionSource<Android.Graphics.Bitmap> source)
			{
				_source = source;
			}

			public void OnLoadingCancelled(string p0, Android.Views.View p1)
			{
				_source.TrySetResult(null);
			}

			public void OnLoadingComplete(string p0, Android.Views.View p1, Android.Graphics.Bitmap bitmap)
			{
				if (!_source.Task.IsCanceled)
				{
					_source.TrySetResult(bitmap);
				}
			}

			public void OnLoadingFailed(string p0, Android.Views.View p1, Com.Nostra13.Universalimageloader.Core.Assist.FailReason p2)
			{
				_source.TrySetException(new Exception(string.Format(CultureInfo.InvariantCulture, "Image loading failed {0}", p2.Type)));
			}

			public void OnLoadingStarted(string p0, Android.Views.View p1)
			{

			}
		}

	}
}