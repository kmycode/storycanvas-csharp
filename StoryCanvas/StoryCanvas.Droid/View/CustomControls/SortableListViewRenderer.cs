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
using Android.Graphics;
using Android.Util;

using StoryCanvas.Droid.View.CustomControls;
using StoryCanvas.View.CustomControls;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Collections;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Entities;

[assembly: Xamarin.Forms.ExportRenderer(typeof(SortableListView), typeof(SortableListViewRenderer))]
namespace StoryCanvas.Droid.View.CustomControls
{
	public class SortableListViewRenderer : ListViewRenderer, AdapterView.IOnItemLongClickListener
	{
		private static readonly int SCROLL_SPEED_FAST = 25;
		private static readonly int SCROLL_SPEED_SLOW = 8;
		private static readonly Bitmap.Config DRAG_BITMAP_CONFIG = Bitmap.Config.Argb8888;

		private bool mSortable = true;
		private bool mDragging = false;
		private DragListener mDragListener;
		private int mBitmapBackgroundColor = Android.Graphics.Color.Argb(128, 0xFF, 0xFF, 0xFF);
		private Bitmap mDragBitmap = null;
		private ImageView mDragImageView = null;
		private WindowManagerLayoutParams mLayoutParams = null;
		private MotionEvent mActionDownEvent;
		private int mPositionFrom = -1;

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="context"></param>
		public SortableListViewRenderer()
		{
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);

			var innerActivity = new InnerActivity(this);
			this.mDragListener = innerActivity.mListDragListener;

			this.Control.OnItemLongClickListener = this;
			this.Control.Touch += (sender, ev) =>
			{
				this.OnTouchEvent(ev.Event);
			};
		}

		/// <summary>
		/// ドラッグイベントリスナの設定
		/// </summary>
		/// <param name="listener"></param>
		public void setDragListener(DragListener listener)
		{
			mDragListener = listener;
		}

		/// <summary>
		/// ソートモードの切替 
		/// </summary>
		/// <param name="sortable"></param>
		public void setSortable(bool sortable)
		{
			this.mSortable = sortable;
		}

		/// <summary>
		/// ソート中アイテムの背景色を設定
		/// </summary>
		/// <param name="color"></param>
		public void setBackgroundColor(int color)
		{
			mBitmapBackgroundColor = color;
		}

		/// <summary>
		/// ソートモードの設定 
		/// </summary>
		/// <returns></returns>
		public bool getSortable()
		{
			return mSortable;
		}

		/// <summary>
		/// MotionEvent から position を取得する 
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		private int evToPosition(MotionEvent ev) {
			return this.Control.PointToPosition((int) ev.GetX(), (int) ev.GetY());
		}

		/// <summary>
		/// タッチイベント処理 
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		public override bool OnTouchEvent(MotionEvent ev) {
			if (!mSortable)
			{
				return this.Control.OnTouchEvent(ev);
			}
			switch (ev.Action) {
				case MotionEventActions.Down: {
					storeMotionEvent(ev);
					break;
				}
				case MotionEventActions.Move: {
					if (duringDrag(ev)) {
						return true;
					}
					break;
				}
				case MotionEventActions.Up: {
					if (stopDrag(ev, true)) {
						return true;
					}
					break;
				}
				case MotionEventActions.Cancel:
				case MotionEventActions.Outside: {
					if (stopDrag(ev, false)) {
						return true;
					}
					break;
				}
			}
			return this.Control.OnTouchEvent(ev);
		}

		/// <summary>
		/// リスト要素長押しイベント処理 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="view"></param>
		/// <param name="position"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool OnItemLongClick(AdapterView parent, Android.Views.View view,
			int position, long id)
		{
			return startDrag();
		}

		/// <summary>
		/// ACTION_DOWN 時の MotionEvent をプロパティに格納 
		/// </summary>
		/// <param name="ev"></param>
		private void storeMotionEvent(MotionEvent ev)
		{
			mActionDownEvent = MotionEvent.Obtain(ev); // 複製しないと値が勝手に変わる
		}

		/// <summary>
		/// ドラッグ開始 
		/// </summary>
		/// <returns></returns>
		private bool startDrag()
		{
			// イベントから position を取得
			mPositionFrom = evToPosition(mActionDownEvent);

			// 取得した position が 0未満＝範囲外の場合はドラッグを開始しない
			if (mPositionFrom < 0)
			{
				return false;
			}
			mDragging = true;

			// View, Canvas, WindowManager の取得・生成
			Android.Views.View view = getChildByIndex(mPositionFrom);
			Canvas canvas = new Canvas();
			IWindowManager wm = getWindowManager();

			// ドラッグ対象要素の View を Canvas に描画
			mDragBitmap = Bitmap.CreateBitmap(view.Width, view.Height,
					DRAG_BITMAP_CONFIG);
			canvas.SetBitmap(mDragBitmap);
			view.Draw(canvas);

			// 前回使用した ImageView が残っている場合は除去（念のため？）
			if (mDragImageView != null)
			{
				wm.RemoveView(mDragImageView);
			}

			// ImageView 用の LayoutParams が未設定の場合は設定する
			if (mLayoutParams == null)
			{
				initLayoutParams();
			}

			// ImageView を生成し WindowManager に addChild する
			mDragImageView = new ImageView(this.Control.Context);
			mDragImageView.SetBackgroundColor(new Android.Graphics.Color(mBitmapBackgroundColor));
			mDragImageView.SetImageBitmap(mDragBitmap);
			wm.AddView(mDragImageView, mLayoutParams);
			
			// ドラッグ開始
			if (mDragListener != null)
			{
				mPositionFrom = mDragListener.onStartDrag(mPositionFrom);
			}
			return duringDrag(mActionDownEvent);
		}

		/// <summary>
		/// ドラッグ処理 
		/// </summary>
		/// <param name="ev"></param>
		/// <returns></returns>
		private bool duringDrag(MotionEvent ev)
		{
			if (!mDragging || mDragImageView == null)
			{
				return false;
			}
			int x = (int) ev.GetX();
			int y = (int) ev.GetY();
			int height = this.Control.Height;
			int middle = height / 2;

			// スクロール速度の決定
			int speed;
			int fastBound = height / 9;
			int slowBound = height / 4;
			if (ev.EventTime - ev.DownTime < 500)
			{
				// ドラッグの開始から500ミリ秒の間はスクロールしない
				speed = 0;
			}
			else if (y < slowBound)
			{
				speed = y < fastBound ? -SCROLL_SPEED_FAST : -SCROLL_SPEED_SLOW;
			}
			else if (y > height - slowBound)
			{
				speed = y > height - fastBound ? SCROLL_SPEED_FAST
						: SCROLL_SPEED_SLOW;
			}
			else
			{
				speed = 0;
			}

			// スクロール処理
			if (speed != 0)
			{
				// 横方向はとりあえず考えない
				int middlePosition = this.Control.PointToPosition(0, middle);
				if (middlePosition == AdapterView.InvalidPosition)
				{
					middlePosition = this.Control.PointToPosition(0, middle + this.Control.DividerHeight
							+ 64);
				}
				Android.Views.View middleView = getChildByIndex(middlePosition);
				if (middleView != null)
				{
					this.Control.SetSelectionFromTop(middlePosition, middleView.Top - speed);
				}
			}

			// ImageView の表示や位置を更新
			if (mDragImageView.Height < 0)
			{
				mDragImageView.Visibility = ViewStates.Invisible;
			}
			else
			{
				mDragImageView.Visibility = ViewStates.Visible;
			}
			updateLayoutParams((int)ev.RawY); // ここだけスクリーン座標を使う
			getWindowManager().UpdateViewLayout(mDragImageView, mLayoutParams);
			if (mDragListener != null)
			{
				mPositionFrom = mDragListener.onDuringDrag(mPositionFrom,
						this.Control.PointToPosition(x, y));
			}
			return true;
		}

		/// <summary>
		/// ドラッグ終了 
		/// </summary>
		/// <param name="ev"></param>
		/// <param name="isDrop"></param>
		/// <returns></returns>
		private bool stopDrag(MotionEvent ev, bool isDrop)
		{
			if (!mDragging)
			{
				return false;
			}
			if (isDrop && mDragListener != null)
			{
				mDragListener.onStopDrag(mPositionFrom, evToPosition(ev));
			}
			mDragging = false;
			if (mDragImageView != null)
			{
				getWindowManager().RemoveView(mDragImageView);
				mDragImageView = null;
				// リサイクルするとたまに死ぬけどタイミング分からない by vvakame
				// mDragBitmap.recycle();
				mDragBitmap = null;

				mActionDownEvent.Recycle();
				mActionDownEvent = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 指定インデックスのView要素を取得する 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private Android.Views.View getChildByIndex(int index)
		{
			return this.Control.GetChildAt(index - this.Control.FirstVisiblePosition);
		}

		/// <summary>
		/// WindowManager の取得 
		/// </summary>
		/// <returns></returns>
		protected IWindowManager getWindowManager()
		{
			return Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
		}

		/// <summary>
		/// ImageView 用 LayoutParams の初期化 
		/// </summary>
		protected void initLayoutParams()
		{
			mLayoutParams = new WindowManagerLayoutParams();
			mLayoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
			mLayoutParams.Height = WindowManagerLayoutParams.WrapContent;
			mLayoutParams.Width = WindowManagerLayoutParams.WrapContent;
			mLayoutParams.Flags = WindowManagerFlags.NotFocusable
					| WindowManagerFlags.NotTouchable
					| WindowManagerFlags.KeepScreenOn
					| WindowManagerFlags.LayoutNoLimits;
			mLayoutParams.Format = Format.Translucent;
			mLayoutParams.WindowAnimations = 0;
			mLayoutParams.X = Left;		// ?
			mLayoutParams.Y = Top;		// ?
		}

		/// <summary>
		/// ImageView 用 LayoutParams の座標情報を更新 
		/// </summary>
		/// <param name="rawY"></param>
		protected void updateLayoutParams(int rawY)
		{
			mLayoutParams.Y = rawY - 32;
		}

		/// <summary>
		/// ドラッグイベントリスナーインターフェース 
		/// </summary>
		public interface DragListener
		{
			/// <summary>
			/// ドラッグ開始時の処理
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			int onStartDrag(int position);

			/// <summary>
			/// ドラッグ中の処理
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			int onDuringDrag(int positionFrom, int positionTo);

			/// <summary>
			/// ドラッグ終了（ドロップ時）の処理
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			bool onStopDrag(int positionFrom, int positionTo);
		}

		/// <summary>
		/// ドラッグイベントリスナー実装
		/// </summary>
		public class SimpleDragListener : DragListener
		{
			/// <summary>
			/// ドラッグ開始時の処理
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			public virtual int onStartDrag(int position)
			{
				return position;
			}

			/// <summary>
			/// ドラッグ中の処理
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			public virtual int onDuringDrag(int positionFrom, int positionTo)
			{
				return positionFrom;
			}

			/// <summary>
			/// ドラッグ終了＝ドロップ時の処理
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			public virtual bool onStopDrag(int positionFrom, int positionTo)
			{
				return positionFrom != positionTo && positionFrom >= 0
						|| positionTo >= 0;
			}
		}

		class InnerActivity
		{
			public InnerActivity(SortableListViewRenderer renderer)
			{
				this.mRenderer = renderer;
				this.mListView = renderer.Control;
				this.mListDragListener = new ListDragListener(this);
			}

			public ListDragListener mListDragListener;

			private SortableListViewRenderer mRenderer;
			public ICollection mList
			{
				get
				{
					return (ICollection)this.mRenderer.Element.ItemsSource;
				}
			}
			public Android.Widget.ListView mListView;
			public int mDraggingPosition = -1;

			public class ListDragListener : SimpleDragListener
			{
				private InnerActivity Activity;

				public ListDragListener(InnerActivity a)
				{
					this.Activity = a;
				}

				public override int onStartDrag(int position)
				{
					this.Activity.mDraggingPosition = position;
					this.Activity.mListView.InvalidateViews();
					return position;
				}
				
				public override int onDuringDrag(int positionFrom, int positionTo)
				{
					if (positionFrom < 0 || positionTo < 0
							|| positionFrom == positionTo)
					{
						return positionFrom;
					}
					int i;
					if (positionFrom < positionTo)
					{
						IList list = (IList)this.Activity.mList;
						int min = positionFrom;
						int max = positionTo < list.Count ? positionTo : list.Count - 1;
						object data = list[min];
						i = min;
						while (i < max)
						{
							list[i] = list[++i];
						}
						list[max] = data;
					}
					else if (positionFrom > positionTo)
					{
						IList list = (IList)this.Activity.mList;
						int min = positionTo;
						int max = positionFrom < list.Count ? positionFrom : list.Count - 1;
						object data = list[max];
						i = max;
						while (i > min)
						{
							list[i] = list[--i];
						}
						list[min] = data;
					}
					this.Activity.mDraggingPosition = positionTo;
					this.Activity.mListView.InvalidateViews();
					return positionTo;
				}
				
				public override bool onStopDrag(int positionFrom, int positionTo)
				{
					this.Activity.mDraggingPosition = -1;
					this.Activity.mListView.InvalidateViews();
					return base.onStopDrag(positionFrom, positionTo);
				}
			}
		}

	}
}