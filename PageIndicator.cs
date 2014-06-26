using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.Interop;
using System;
using System.Linq;

namespace Creuna.MonoDroid.PageHelper
{
    public class PageIndicator : View
    {
        public const int NoActiveDot = -1;

        public enum DotTypes
        {
            Single = 0,
            Multiple = 1
        }

        private const int MinDotCount = 1;
        
        private static readonly Rect _inRect = new Rect();
        private static readonly Rect _outRect = new Rect();

        private GravityFlags _gravityFlag;
        private int _dotSpacing;
        private Drawable _dotDrawable;
        private int _dotCount;
        private DotTypes _dotType;
        private int _activeDot;
        private int[] _extraState;
        private readonly bool _initializing;

        public PageIndicator(Context context) : this(context, null)
        {   
        }

        public PageIndicator(IntPtr intPtr, JniHandleOwnership jniHandleOwnership) : base(intPtr, jniHandleOwnership)
        {
        }

        public PageIndicator(Context context, IAttributeSet attributeSet) : this(context, attributeSet, Resource.Attribute.gdPageIndicatorStyle)
        {
        }

        public PageIndicator(Context context, IAttributeSet attributeSet, int defStyle) : base(context, attributeSet, defStyle)
        {
            InitPageIndicator();
            _initializing = true;

            var a = context.ObtainStyledAttributes(attributeSet, Resource.Styleable.PageIndicator, defStyle, 0);

            DotCount = a.GetInt(Resource.Styleable.PageIndicator_dotCount, DotCount);
            ActiveDot = a.GetInt(Resource.Styleable.PageIndicator_activeDot, _activeDot);
            DotDrawable = a.GetDrawable(Resource.Styleable.PageIndicator_dotDrawable);
            DotSpacing = a.GetDimensionPixelSize(Resource.Styleable.PageIndicator_dotSpacing, _dotSpacing);
            GravityFlag = (GravityFlags) a.GetInt(Resource.Styleable.PageIndicator_gravity, (int) _gravityFlag);
            DotType = (DotTypes) a.GetInt(Resource.Styleable.PageIndicator_dotType, (int) _dotType);

            a.Recycle();

            _initializing = false;

        }

        private void InitPageIndicator()
        {
            DotCount = MinDotCount;
            _gravityFlag = GravityFlags.Center;
            _activeDot = 0;
            _dotSpacing = 0;
            _dotType = DotTypes.Single;

            _extraState = OnCreateDrawableState(1);
            var setAsInt = SelectedStateSet.OfType<int>().ToArray();
            MergeDrawableStates(_extraState, setAsInt);
        }

        public int DotCount
        {
            get { return _dotCount; }
            set
            {
                if (value < MinDotCount)
                    value = MinDotCount;
                if (_dotCount != value)
                {
                    _dotCount = value;
                    RequestLayout();
                    Invalidate();
                }
            }
        }

        public int ActiveDot
        {
            get { return _activeDot; }
            set
            {
                if (value < 0)
                {
                    value = NoActiveDot;
                }

                switch (_dotType)
                {
                    case DotTypes.Single:
                        if (value > _dotCount - 1)
                        {
                            value = NoActiveDot;
                        }
                        break;

                    case DotTypes.Multiple:
                        if (value > _dotCount)
                        {
                            value = NoActiveDot;
                        }
                        break;
                }
                _activeDot = value;
                Invalidate();
            }
        }

        public Drawable DotDrawable
        {
            get { return _dotDrawable; }
            set
            {
                if (value != _dotDrawable)
                {
                    if (_dotDrawable != null)
                    {
                        _dotDrawable.SetCallback(null);
                    }
                }
                
                _dotDrawable = value;

                if (value != null)
                {
                    if (value.IntrinsicHeight == -1 || value.IntrinsicWidth == -1)
                    {
                        return;
                    }
                    value.SetBounds(0, 0, value.IntrinsicWidth, value.IntrinsicHeight);
                    value.SetCallback(this);
                    if (value.IsStateful)
                    {
                        value.SetState(GetDrawableState());
                    }
                }
                RequestLayout();
                Invalidate();
            }
        }

        public int DotSpacing
        {
            get { return _dotSpacing; }
            set
            {
                if (value != _dotSpacing)
                {
                    _dotSpacing = value;
                    RequestLayout();
                    Invalidate();
                }
            }
        }

        public GravityFlags GravityFlag
        {
            get { return _gravityFlag; }
            set
            {
                if (_gravityFlag != value)
                {
                    _gravityFlag = value;
                    Invalidate();
                }
            }
        }

        public DotTypes DotType
        {
            get { return _dotType; }
            set
            {
                if (value == DotTypes.Single || value == DotTypes.Multiple)
                {
                    if (_dotType != value)
                    {
                        _dotType = value;
                        Invalidate();
                    }
                }
            }
        }

        public override void RequestLayout()
        {
            if (!_initializing)
            {
                base.RequestLayout();                
            }
        }

        public override void Invalidate()
        {
            if (!_initializing)
            {
                base.Invalidate();
            }
        }

        protected override bool VerifyDrawable(Drawable who)
        {
            return base.VerifyDrawable(who) || who == _dotDrawable;
        }

        protected override void DrawableStateChanged()
        {
            base.DrawableStateChanged();
            _extraState = OnCreateDrawableState(1);
            var setAsInt = SelectedStateSet.OfType<int>().ToArray();
            MergeDrawableStates(_extraState, setAsInt);
            Invalidate();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var d = _dotDrawable;

            int width = 0;
            int height = 0;
            if (d != null)
            {
                width = _dotCount*(d.IntrinsicWidth + _dotSpacing) - _dotSpacing;
                height = d.IntrinsicHeight;
            }

            width += PaddingRight + PaddingLeft;
            height += PaddingBottom + PaddingTop;

            SetMeasuredDimension(ResolveSize(width, widthMeasureSpec), ResolveSize(height, heightMeasureSpec));
        }

        protected override void OnDraw(Canvas canvas)
        {
            var d = _dotDrawable;
            if (d != null)
            {
                var count = _dotType == DotTypes.Single ? _dotCount : _activeDot;

                if (count <= 0) return;

                int h = d.IntrinsicHeight;
                int w = Math.Max(0, count*(d.IntrinsicWidth + _dotSpacing) - _dotSpacing);

                int pRight = PaddingRight;
                int pLeft = PaddingLeft;
                int pTop = PaddingTop;
                int pBottom = PaddingBottom;

                _inRect.Set(pLeft, pTop, Width - pRight, Height - pBottom);
                Gravity.Apply(_gravityFlag, w, h, _inRect, _outRect);

                canvas.Save();
                canvas.Translate(_outRect.Left, _outRect.Top);
                for (int i = 0; i < count; i++)
                {
                    if (d.IsStateful)
                    {
                        int[] state = GetDrawableState();
                        if (_dotType == DotTypes.Multiple || i == _activeDot)
                        {
                            state = _extraState;
                        }
                        d.SetCallback(null);
                        d.SetState(state);
                        d.SetCallback(this);
                    }
                    d.Draw(canvas);
                    canvas.Translate(_dotSpacing + d.IntrinsicWidth, 0);
                }
                canvas.Restore();
            }
        }

        protected class SavedState : BaseSavedState
        {
            public int ActiveDot { get; set; }

            public SavedState(IParcelable superState) : base(superState)
            {
            }

            public SavedState(Parcel into) : base(into)
            {
                ActiveDot = into.ReadInt();
            }

            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(ActiveDot);
            }

            private static readonly GenericParcelableCreator<SavedState> _creator = new GenericParcelableCreator<SavedState>((parcel => new SavedState(parcel)));

            [ExportField("CREATOR")]
            public static GenericParcelableCreator<SavedState> GetCreator()
            {
                return _creator;
            } 

            public SavedState() : base(EmptyState)
            {
                
            }
        }

        public sealed class GenericParcelableCreator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object, new()
        {
            private readonly Func<Parcel, T> _createFunc;
 
            /// <summary>
            /// Initializes a new instance of the <see cref="ParcelableDemo.GenericParcelableCreator`1"/> class.
            /// </summary>
            /// <param name='createFromParcelFunc'>
            /// Func that creates an instance of T, populated with the values from the parcel parameter
            /// </param>
            public GenericParcelableCreator(Func<Parcel, T> createFromParcelFunc)
            {
                _createFunc = createFromParcelFunc;
            }
 
            #region IParcelableCreator Implementation
 
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return _createFunc(source);
            }
 
            public Java.Lang.Object[] NewArray(int size)
            {
                return new T[size];
            }
 
            #endregion
        }

        protected override IParcelable OnSaveInstanceState()
        {
            var superState = base.OnSaveInstanceState();

            var ss = new SavedState(superState) {ActiveDot = _activeDot};

            return ss;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            var ss = (SavedState) state;
            base.OnRestoreInstanceState(ss.SuperState);

            _activeDot = ss.ActiveDot;
        }
    }
}