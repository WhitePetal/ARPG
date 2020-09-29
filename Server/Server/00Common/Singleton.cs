/*********************************************************
	文件：Singleton
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:13:52
	功能：待定
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Singleton<T> where T : new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null) instance = new T();
                return instance;
            }
        }
    }
}
