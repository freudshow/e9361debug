using System;

namespace E9361App.Common
{
    public class Common
    {
        private static readonly string MainVersion = "0";
        private static readonly string SubVersion = "1";
        private static readonly string PatchVersion = "0";

        public static readonly string Version = $"{MainVersion}.{SubVersion}.{PatchVersion}";

        /// <summary>
        /// 验证一个整型值是否在
        /// enum中被定义
        /// </summary>
        /// <param name="t">enum类型</param>
        /// <param name="i">欲验证的整型值</param>
        /// <returns></returns>
        public static bool IsInEnum(Type t, int i)
        {
            if (t == null)
            {
                return false;
            }

            return t.IsEnum && Enum.IsDefined(t, i);
        }

        /// <summary>
        /// 将字符串转为目标枚举类型
        /// </summary>
        /// <typeparam name="T">目标枚举类型</typeparam>
        /// <param name="s">枚举类型的成员名</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetEnumByString<T>(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new Exception("argument can not be null or empty");
            }

            return (T)Enum.Parse(typeof(T), s, false);
        }
    }
}