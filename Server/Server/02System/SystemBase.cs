/*********************************************************
	文件：SystemBase
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/20 14:47:31
	功能：待定
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SystemBase<T> : Singleton<T> where T : new()
    {
        protected CacheService cacheSev;

        public virtual void Init()
        {
            cacheSev = CacheService.Instance;
            NETCommon.Log(this.GetType().Name + " Init");
        }
    }
}
