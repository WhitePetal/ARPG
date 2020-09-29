/*********************************************************
	文件：NETCommon
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 11:01:05
	功能：服务端/客户端共用工具类
***********************************************************/

using PENet;
using Protocol;

public enum NETLogLevel
{
    Log = 0,
    Warning = 1,
    Error = 2,
    Info = 3
}

public class NETCommon
{
    public static void Log(string msg = "", NETLogLevel lv = NETLogLevel.Log)
    {
        LogLevel level = (LogLevel)lv;
        PETool.LogMsg(msg, level);
    }

    public static int GetFightByProps(PlayerData pd)
    {
        return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
    }

    public static int GetPowerLimit(int lv)
    {
        return ((lv - 1) / 10) * 150 + 150;
    }

    public static int GetExpUpValByLv(int lv)
    {
        return 100 * lv * lv;
    }

    public static void CalcExp(PlayerData pd, int addExp)
    {
        int curLv = pd.lv;
        int curExp = pd.exp;
        int addRestExp = addExp;
        while (true)
        {
            int upNeedExp = NETCommon.GetExpUpValByLv(curLv) - curExp;
            if (addRestExp >= upNeedExp)
            {
                curLv += 1;
                curExp = 0;
                addRestExp -= upNeedExp;
            }
            else
            {
                pd.lv = curLv;
                pd.exp = curExp + addRestExp;
                break;
            }
        }
    }

    public const int PowerAddSpace = 5; // 分钟
    public const int PowerAddCount = 2;
}
