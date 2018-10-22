using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		private static readonly Dictionary<string, Task> CurrentRequestForUri = new Dictionary<string, Task>();

		public async Task<Bitmap> LoadImageAsync(CancellationToken ct, string uri, ImageView imageView, Size? targetSize,
			DisplayImageOptions options)
		{
			try
			{
				for (var retryNo = 0; retryNo < 12; retryNo++)
				{
					Task taskToWait;
					Task<Bitmap> taskToRun = null;

					lock (CurrentRequestForUri)
					{
						// Any other concurrent request to this uri?
						if (!CurrentRequestForUri.TryGetValue(uri, out taskToWait)) // yes: we assign it to the variable.
						{
							// Nope, we're the only one (or first one), we create
							// a semaphore others will wait on.
							taskToRun = InnerLoadImageAsync(ct, uri, imageView, targetSize, options);
							CurrentRequestForUri[uri] = taskToRun;
						}
					}

					if (taskToRun != null)
					{
						return await taskToRun;
					}
					else if (taskToWait != null)
					{

						try
						{
							await taskToWait; // wait for other before retrying
						}
						catch (Exception)
						{
							if (ct.IsCancellationRequested)
							{
								throw;
							}
							// else retry the current request
						}

						if (retryNo > 1)
						{
							// After 1st retry, wait for others to execute before retrying to
							// minimize competing tasks issues.
							await Task.Delay(retryNo * 5, ct);
						}
					}
				}

				throw new InvalidOperationException("Retry count exceed.");
			}
			finally
			{
				lock (CurrentRequestForUri)
				{
					// Remove waiting entry in dictionary
					CurrentRequestForUri.Remove(uri);
				}
			}
		}

		private static async Task<Bitmap> InnerLoadImageAsync(CancellationToken ct, string uri, ImageView imageView, Size? targetSize, DisplayImageOptions options)
		{
			TaskCompletionSource<Android.Graphics.Bitmap> source = new TaskCompletionSource<Android.Graphics.Bitmap>();


			options = options ?? new DisplayImageOptions.Builder()
				.CacheInMemory(true)
				.CacheOnDisk(true)
				.Build();

			// Propagate cancellation to the managed TCS, even though we don't propagate it to Universal Image Loader. This is because UIL has 
			// the air of cancelling downloads itself if another uri is requested for the same ImageView, leading us to wait on a task that 
			// will never complete (thanks to the concurrency logic reusing existing tasks).
			using (ct.Register(() => source.TrySetCanceled()))
			{
				using (var aware =
					imageView != null
						? new ImageViewAwareCancellable(imageView, ct)
						: null
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
								listener
								);
						}
						else
						{
							ImageLoader.Instance.LoadImage(
								uri,
								options,
								listener
								);
						}

						var target = await source.Task;

						return target;
					}
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
				_source.TrySetCanceled();
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