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
        /// 線と点の距離をもとめる
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
