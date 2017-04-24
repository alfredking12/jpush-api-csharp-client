using System;
using System.Text;

namespace cn.jpush.api.util
{
    class Base64
    {
        public static String getBase64Encode(String str)
        {
#if COREFX
            byte[] bytes = Encoding.ASCII.GetBytes(str);
#else
            byte[] bytes = Encoding.Default.GetBytes(str);
#endif

            //
            return Convert.ToBase64String(bytes);
        }
    }
}
