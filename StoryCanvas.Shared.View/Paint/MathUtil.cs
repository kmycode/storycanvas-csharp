using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint
{
    static class MathUtil
    {
        /// <summary>
        /// 2つの座標から１次方程式を取得する
        /// </summary>
        /// <param name="x1">座標１のX</param>
        /// <param name="y1">座標１のY</param>
        /// <param name="x2">座標２のX</param>
        /// <param name="y2">座標２のY</param>
        /// <returns>１次方程式</returns>
        public static LinearEquation GetLinearEquation(float x1, float y1, float x2, float y2)
        {
            // y = ax + c の形にする
            var b = 1.0f;
            var a = (y2 - y1) / (x2 - x1);
            var c = y1 - a * x1;

            return new LinearEquation
            {
                A = a,
                B = -b,
                C = c,
            };
        }

        /// <summary>
        /// 直線と点の距離をもとめる
        /// （点から線に垂線をおろしたときの垂線の長さ）
        /// 結果は2乗されて返ってくるので注意すること
        /// </summary>
        /// <param name="equation">１次方程式</param>
        /// <param name="x">点の座標X</param>
        /// <param name="y">点の座標Y</param>
        /// <returns>距離</returns>
        public static float GetDistanceBetweenLineAndPosition(LinearEquation equation, float x, float y)
        {
            // http://www004.upp.so-net.ne.jp/s_honma/urawaza/distance.htm
            // d^2 = (a * x + b * y + c)^2 / (a^2 + b^2)
            var abc = equation.A * x + equation.B * y + equation.C;
            var k = abc * abc / (equation.A * equation.A + equation.B * equation.B);
            k = (float)Math.Sqrt(k);
            return abc * abc / (equation.A * equation.A + equation.B * equation.B);
        }

        /// <summary>
        /// 線分と点の距離を求める
        /// 結果は2乗されて返ってくるので注意すること
        /// </summary>
        /// <remarks>
        /// thanks http://qiita.com/yellow_73/items/bcd4e150e7caa0210ee6
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static float GetDistanceBetweenLimitedLineAndPosition(float x, float y, float x1, float y1, float x2, float y2)
        {
            var a = x2 - x1;
            var b = y2 - y1;
            var a2 = a * a;
            var b2 = b * b;
            var r2 = a2 + b2;
            var tt = -(a * (x1 - x) + b * (y1 - y));
            if (tt < 0)
            {
                return (x1 - x) * (x1 - x) + (y1 - y) * (y1 - y);
            }
            if (tt > r2)
            {
                return (x2 - x) * (x2 - x) + (y2 - y) * (y2 - y);
            }
            var f1 = a * (y1 - y) - b * (x1 - x);
            return (f1 * f1) / r2;
        }
    }

    /// <summary>
    /// １次方程式を表現するオブジェクト
    /// ax + by + c = 0 のかたちであらわす
    /// -by = ax + c となる
    /// </summary>
    public struct LinearEquation
    {
        /// <summary>
        /// Xの係数
        /// </summary>
        public float A { get; set; }

        /// <summary>
        /// Yの係数
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float C { get; set; }

        public float GetX(float y)
        {
            // x = (-by - c) / a
            return (-this.B * y - this.C) / this.A;
        }

        public float GetY(float x)
        {
            // y = (-ax - c) / b
            return (-this.A * x - this.C) / this.B;
        }
    }
}
