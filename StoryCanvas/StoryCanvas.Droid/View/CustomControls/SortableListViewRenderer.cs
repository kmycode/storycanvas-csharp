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
		/// ������
		/// </summary>
		/// <param name="context"></param>
		public SortableListViewRenderer()
		{
		}

		/// <summary>
		/// ������
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
		/// �h���b�O�C�x���g���X�i�̐ݒ�
		/// </summary>
		/// <param name="listener"></param>
		public void setDragListener(DragListener listener)
		{
			mDragListener = listener;
		}

		/// <summary>
		/// �\�[�g���[�h�̐ؑ� 
		/// </summary>
		/// <param name="sortable"></param>
		public void setSortable(bool sortable)
		{
			this.mSortable = sortable;
		}

		/// <summary>
		/// �\�[�g���A�C�e���̔w�i�F��ݒ�
		/// </summary>
		/// <param name="color"></param>
		public void setBackgroundColor(int color)
		{
			mBitmapBackgroundColor = color;
		}

		/// <summary>
		/// �\�[�g���[�h�̐ݒ� 
		/// </summary>
		/// <returns></returns>
		public bool getSortable()
		{
			return mSortable;
		}

		/// <summary>
		/// MotionEvent ���� position ���擾���� 
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		private int evToPosition(MotionEvent ev) {
			return this.Control.PointToPosition((int) ev.GetX(), (int) ev.GetY());
		}

		/// <summary>
		/// �^�b�`�C�x���g���� 
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
		/// ���X�g�v�f�������C�x���g���� 
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
		/// ACTION_DOWN ���� MotionEvent ���v���p�e�B�Ɋi�[ 
		/// </summary>
		/// <param name="ev"></param>
		private void storeMotionEvent(MotionEvent ev)
		{
			mActionDownEvent = MotionEvent.Obtain(ev); // �������Ȃ��ƒl������ɕς��
		}

		/// <summary>
		/// �h���b�O�J�n 
		/// </summary>
		/// <returns></returns>
		private bool startDrag()
		{
			// �C�x���g���� position ���擾
			mPositionFrom = evToPosition(mActionDownEvent);

			// �擾���� position �� 0�������͈͊O�̏ꍇ�̓h���b�O���J�n���Ȃ�
			if (mPositionFrom < 0)
			{
				return false;
			}
			mDragging = true;

			// View, Canvas, WindowManager �̎擾�E����
			Android.Views.View view = getChildByIndex(mPositionFrom);
			Canvas canvas = new Canvas();
			IWindowManager wm = getWindowManager();

			// �h���b�O�Ώۗv�f�� View �� Canvas �ɕ`��
			mDragBitmap = Bitmap.CreateBitmap(view.Width, view.Height,
					DRAG_BITMAP_CONFIG);
			canvas.SetBitmap(mDragBitmap);
			view.Draw(canvas);

			// �O��g�p���� ImageView ���c���Ă���ꍇ�͏����i�O�̂��߁H�j
			if (mDragImageView != null)
			{
				wm.RemoveView(mDragImageView);
			}

			// ImageView �p�� LayoutParams �����ݒ�̏ꍇ�͐ݒ肷��
			if (mLayoutParams == null)
			{
				initLayoutParams();
			}

			// ImageView �𐶐��� WindowManager �� addChild ����
			mDragImageView = new ImageView(this.Control.Context);
			mDragImageView.SetBackgroundColor(new Android.Graphics.Color(mBitmapBackgroundColor));
			mDragImageView.SetImageBitmap(mDragBitmap);
			wm.AddView(mDragImageView, mLayoutParams);
			
			// �h���b�O�J�n
			if (mDragListener != null)
			{
				mPositionFrom = mDragListener.onStartDrag(mPositionFrom);
			}
			return duringDrag(mActionDownEvent);
		}

		/// <summary>
		/// �h���b�O���� 
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

			// �X�N���[�����x�̌���
			int speed;
			int fastBound = height / 9;
			int slowBound = height / 4;
			if (ev.EventTime - ev.DownTime < 500)
			{
				// �h���b�O�̊J�n����500�~���b�̊Ԃ̓X�N���[�����Ȃ�
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

			// �X�N���[������
			if (speed != 0)
			{
				// �������͂Ƃ肠�����l���Ȃ�
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

			// ImageView �̕\����ʒu���X�V
			if (mDragImageView.Height < 0)
			{
				mDragImageView.Visibility = ViewStates.Invisible;
			}
			else
			{
				mDragImageView.Visibility = ViewStates.Visible;
			}
			updateLayoutParams((int)ev.RawY); // ���������X�N���[�����W���g��
			getWindowManager().UpdateViewLayout(mDragImageView, mLayoutParams);
			if (mDragListener != null)
			{
				mPositionFrom = mDragListener.onDuringDrag(mPositionFrom,
						this.Control.PointToPosition(x, y));
			}
			return true;
		}

		/// <summary>
		/// �h���b�O�I�� 
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
				// ���T�C�N������Ƃ��܂Ɏ��ʂ��ǃ^�C�~���O������Ȃ� by vvakame
				// mDragBitmap.recycle();
				mDragBitmap = null;

				mActionDownEvent.Recycle();
				mActionDownEvent = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// �w��C���f�b�N�X��View�v�f���擾���� 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private Android.Views.View getChildByIndex(int index)
		{
			return this.Control.GetChildAt(index - this.Control.FirstVisiblePosition);
		}

		/// <summary>
		/// WindowManager �̎擾 
		/// </summary>
		/// <returns></returns>
		protected IWindowManager getWindowManager()
		{
			return Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
		}

		/// <summary>
		/// ImageView �p LayoutParams �̏����� 
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
		/// ImageView �p LayoutParams �̍��W�����X�V 
		/// </summary>
		/// <param name="rawY"></param>
		protected void updateLayoutParams(int rawY)
		{
			mLayoutParams.Y = rawY - 32;
		}

		/// <summary>
		/// �h���b�O�C�x���g���X�i�[�C���^�[�t�F�[�X 
		/// </summary>
		public interface DragListener
		{
			/// <summary>
			/// �h���b�O�J�n���̏���
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			int onStartDrag(int position);

			/// <summary>
			/// �h���b�O���̏���
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			int onDuringDrag(int positionFrom, int positionTo);

			/// <summary>
			/// �h���b�O�I���i�h���b�v���j�̏���
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			bool onStopDrag(int positionFrom, int positionTo);
		}

		/// <summary>
		/// �h���b�O�C�x���g���X�i�[����
		/// </summary>
		public class SimpleDragListener : DragListener
		{
			/// <summary>
			/// �h���b�O�J�n���̏���
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			public virtual int onStartDrag(int position)
			{
				return position;
			}

			/// <summary>
			/// �h���b�O���̏���
			/// </summary>
			/// <param name="positionFrom"></param>
			/// <param name="positionTo"></param>
			/// <returns></returns>
			public virtual int onDuringDrag(int positionFrom, int positionTo)
			{
				return positionFrom;
			}

			/// <summary>
			/// �h���b�O�I�����h���b�v���̏���
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