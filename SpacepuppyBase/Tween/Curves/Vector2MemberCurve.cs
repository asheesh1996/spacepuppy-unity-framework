﻿using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Tween.Curves
{

    [CustomMemberCurve(typeof(Vector2))]
    public class Vector2MemberCurve : MemberCurve, ISupportRedirectToMemberCurve
    {
        
        #region Fields

        private Vector2 _start;
        private Vector2 _end;
        private bool _useSlerp;

        #endregion

        #region CONSTRUCTOR

        protected Vector2MemberCurve()
        {

        }

        public Vector2MemberCurve(string propName, float dur, Vector2 start, Vector2 end)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector2MemberCurve(string propName, float dur, Vector2 start, Vector2 end, bool slerp)
            : base(propName, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        public Vector2MemberCurve(string propName, Ease ease, float dur, Vector2 start, Vector2 end)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = false;
        }

        public Vector2MemberCurve(string propName, Ease ease, float dur, Vector2 start, Vector2 end, bool slerp)
            : base(propName, ease, dur)
        {
            _start = start;
            _end = end;
            _useSlerp = slerp;
        }

        protected override void ReflectiveInit(System.Type memberType, object start, object end, object option)
        {
            _start = ConvertUtil.ToVector2(start);
            _end = ConvertUtil.ToVector2(end);
            _useSlerp = ConvertUtil.ToBool(option);
        }

        void ISupportRedirectToMemberCurve.ConfigureAsRedirectTo(System.Type memberType, float totalDur, object current, object start, object end, object option)
        {
            _useSlerp = ConvertUtil.ToBool(option);
            
            if (_useSlerp)
            {
                var c = ConvertUtil.ToVector2(current);
                var s = ConvertUtil.ToVector2(start);
                var e = ConvertUtil.ToVector2(end);
                _start = c;
                _end = e;

                var at = Vector2.Angle(s, e);
                if ((System.Math.Abs(at) < MathUtil.EPSILON))
                {
                    this.Duration = 0f;
                }
                else
                {
                    var ap = Vector2.Angle(c, e);
                    this.Duration = totalDur * ap / at;
                }
            }
            else
            {
                var c = ConvertUtil.ToVector2(current);
                var s = ConvertUtil.ToVector2(start);
                var e = ConvertUtil.ToVector2(end);
                _start = c;
                _end = e;

                c -= e;
                s -= e;
                if (VectorUtil.NearZeroVector(s))
                {
                    this.Duration = 0f;
                }
                else
                {
                    this.Duration = totalDur * Vector3.Dot(c, s.normalized) / Vector3.Dot(s, c.normalized);
                }
            }
        }

        #endregion

        #region Properties

        public Vector2 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector2 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public bool UseSlerp
        {
            get { return _useSlerp; }
            set { _useSlerp = value; }
        }

        #endregion

        #region MemberCurve Interface

        protected override object GetValueAt(float dt, float t)
        {
            if (this.Duration == 0f) return _end;
            t = this.Ease(t, 0f, 1f, this.Duration);
            return (_useSlerp) ? VectorUtil.Slerp(_start, _end, t) : Vector2.LerpUnclamped(_start, _end, t);
        }

        #endregion

    }
}
