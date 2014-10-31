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

			options = options ?? new DisplayImageOptions.Builder()
				.CacheInMemory(true)
				.CacheOnDisk(true)
				.Build();

			var aware = new ImageViewAware(imageView);

			if (targetSize != null)
			{
				imageView.SetMaxHeight(targetSize.Value.Height);
				imageView.SetMaxWidth(targetSize.Value.Width);

				ImageLoader.Instance.DisplayImage(
					uri,
					imageView,
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

		private class ImageListener : Java.Lang.Object, IImageLoadingListener
		{
			private TaskCompletionSource<Android.Graphics.Bitmap> _source;

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
				_source.TrySetResult(bitmap);
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