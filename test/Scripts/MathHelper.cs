using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulate
{
    public static class MathHelper
    {
        public const float Pi = 3.14159274F;

        public static Random random = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字
        
        /// <summary>
        /// 产生截断的正态分布随机数
        /// </summary>
        /// <param name="multi">multi值越大，曲线越平，相当于截取其中部分</param>
        /// <param name="center">中心</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomNormal(float multi, float center, float min, float max)
        {
            double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0;
            while (s > 1 || s == 0)
            {
                u1 = random.NextDouble();
                u2 = random.NextDouble();
                v1 = 2 * u1 - 1;
                v2 = 2 * u2 - 1;
                s = v1 * v1 + v2 * v2;
            }
            z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
            z1 *= multi;
            z1 += +center;
            if (z1 < min || z1 > max) z1 = RandomNormal(multi, center, min, max);
            //Debug.Log(z1);
            return (float)z1; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        }

        /// <summary>
        /// 从随机数0到1产生指数分布随机数,有最大值最小值
        /// </summary>
        /// <param name="multi">multi值越大，曲线越平，相当于截取其中部分</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomEXP(int multi, int min, int max)
        {
            double u1, S1 = 0;
            u1 = random.NextDouble();
            S1 = -1 * Math.Log(1 - u1);

            int k = random.Next(0, 2) == 1 ? -1 : +1;
            S1 *= multi * k;
            if (S1 > max) S1 = RandomEXP(multi, min, max);
            if (S1 < min) S1 = RandomEXP(multi, min, max);
            //Debug.Log(S1);
            return (float)S1;
        }


        /**
         * <summary>Computes the length of a specified two-dimensional vector.
         * </summary>
         *
         * <param name="vector">The two-dimensional vector whose length is to be
         * computed.</param>
         * <returns>The length of the two-dimensional vector.</returns>
         */
        public static float abs(Vector2 vector)
        {
            return sqrt(absSq(vector));
        }

        /**
         * <summary>Computes the squared length of a specified two-dimensional
         * vector.</summary>
         *
         * <returns>The squared length of the two-dimensional vector.</returns>
         *
         * <param name="vector">The two-dimensional vector whose squared length
         * is to be computed.</param>
         */
        public static float absSq(Vector2 vector)
        {
            return vector * vector;
        }

        internal static float sqrt(float scalar)
        {
            return (float)Math.Sqrt(scalar);
        }
    }

}

