using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerStart
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();

            while (true)
            {
                ServerRoot.Instance.Update();
            }
        }
    }
}
