/*********************************************************
	文件：Constans
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 14:52:29
	功能：常量配置
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Constans
{
    public static StringBuilder sb = new StringBuilder(60000);
    private static char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    public static string ColorStr(string str, TxtColor color)
    {
        switch (color)
        {
            case TxtColor.Red:
                sb.Append(ColorRed);
                sb.Append(str);
                sb.Append(ColorEnd);
                break;
            case TxtColor.Green:
                sb.Append(ColorGreen);
                sb.Append(str);
                sb.Append(ColorEnd);
                break;
            case TxtColor.Blue:
                sb.Append(ColorBlue);
                sb.Append(str);
                sb.Append(ColorEnd);
                break;
            case TxtColor.Yellow:
                sb.Append(ColorYellow);
                sb.Append(str);
                sb.Append(ColorEnd);
                break;
        }
        string res = sb.ToString();
        sb.Clear();
        return res;
    }
    public static string SizeStr(string str, int size)
    {
        sb.Append("<size=");
        sb.Append(size);
        sb.Append('>');
        sb.Append(str);
        sb.Append("</size>");
        string res = sb.ToString();
        sb.Clear();
        return res;
    }
    #region Old StrConnect
    //public static string ConnectStr(this string str1, string str2)
    //{
    //    sb.Append(str1);
    //    sb.Append(str2);
    //    string str = sb.ToString();
    //    sb.Clear();
    //    return str;
    //}
    //public static string ConnectStr(this string str1, int num)
    //{
    //    sb.Append(str1);
    //    if (num < 0)
    //    {
    //        num = -num;
    //        sb.Append('-');
    //    }
    //    int len = 1;
    //    int t = 10;
    //    while (num / t > 0)
    //    {
    //        t *= 10;
    //        ++len;
    //    }
    //    sb.Length += len;
    //    int index = sb.Length - 1;
    //    do
    //    {
    //        sb[index] = chars[num % 10];
    //        --index;
    //    }
    //    while ((num /= 10) > 0);
    //    string str = sb.ToString();
    //    sb.Clear();
    //    return str;
    //}
    //public static string ConnectStr(this int num, string str)
    //{
    //    if (num < 0)
    //    {
    //        num = -num;
    //        sb.Append('-');
    //    }
    //    int len = 1;
    //    int t = 10;
    //    while (num / t > 0)
    //    {
    //        t *= 10;
    //        ++len;
    //    }
    //    sb.Length += len;
    //    int index = sb.Length - 1;
    //    do
    //    {
    //        sb[index] = chars[num % 10];
    //        --index;
    //    }
    //    while ((num /= 10) > 0);
    //    sb.Append(str);
    //    string res = sb.ToString();
    //    sb.Clear();
    //    return res;
    //}
    #endregion
    public static StringBuilder ConnectStr(this string str1, string str2)
    {
        if (sb.Length > 0) sb.Clear(); 
        sb.Append(str1);
        sb.Append(str2);
        return sb;
    }
    public static StringBuilder ConnectStr(this string str1, int num)
    {
        if (sb.Length > 0) sb.Clear();
        sb.Append(str1);
        if (num < 0)
        {
            num = -num;
            sb.Append('-');
        }
        int len = 1;
        int t = 10;
        while (num / t > 0)
        {
            t *= 10;
            ++len;
        }
        sb.Length += len;
        int index = sb.Length - 1;
        do
        {
            sb[index] = chars[num % 10];
            --index;
        }
        while ((num /= 10) > 0);
        return sb;
    }
    public static StringBuilder ConnectStr(this string str1, char c)
    {
        if (sb.Length > 0) sb.Clear();
        sb.Append(str1);
        sb.Append(c);
        return sb;
    }
    public static StringBuilder ConnectStr(this int num, string str)
    {
        if (sb.Length > 0) sb.Clear();

        SBAppednNum(num);
        return sb.Append(str);
    }
    public static StringBuilder ConnectStr(this int num, char c)
    {
        if (sb.Length > 0) sb.Clear();
        SBAppednNum(num);
        return sb.Append(c);
    }
    public static StringBuilder ConnectStr(this StringBuilder _sb, string str)
    {
        return _sb.Append(str);
    }
    public static StringBuilder ConnectStr(this StringBuilder _sb, int num)
    {
        if (num < 0)
        {
            num = -num;
            _sb.Append('-');
        }
        int len = 1;
        int t = 10;
        while (num / t > 0)
        {
            t *= 10;
            ++len;
        }
        _sb.Length += len;
        int index = _sb.Length - 1;
        do
        {
            _sb[index] = chars[num % 10];
            --index;
        }
        while ((num /= 10) > 0);
        return _sb;
    }
    public static StringBuilder ConnectStr(this StringBuilder _sb, char c)
    {
        return _sb.Append(c);
    }
    public static string EndConnectStr(this StringBuilder _sb)
    {
        string res = _sb.ToString();
        _sb.Clear();
        return res;
    }

    private static void SBAppednNum(int num)
    {
        if (num < 0)
        {
            num = -num;
            sb.Append('-');
        }
        int len = 1;
        int t = 10;
        while (num / t > 0)
        {
            t *= 10;
            ++len;
        }
        sb.Length += len;
        int index = sb.Length - 1;
        do
        {
            sb[index] = chars[num % 10];
            --index;
        }
        while ((num /= 10) > 0);
    }

    // 场景名称
    public const string SceneLogin = "SceneLogin";
    public const string SceneMainCity = "SceneMainCity";
    public const int MainCityID = 10000;

    // NPCId
    public const int NpcWiseMan = 0;
    public const int NpcGeneral = 1;
    public const int NpcArtisan = 2;
    public const int NpcTrader = 3;
    // NPC Name
    public const string WiseManName = "南山老者";
    public const string GeneralName = "城防将军";
    public const string ArtisanName = "铁匠张";
    public const string TraderName = "易千金";
    public const string GuidName = "精灵";

    // 音效名称
    // BGM音效
    public const string LoginBGM = "Login";
    public const string MainCityBGM = "MainCity";
    public const string HuangYeBGM = "bgHuangYe";
    // 常规UI点击音效
    public const string UILoginBtnAudio = "uiLoginBtn";// 登录按钮音效
    public const string UIClickBtnAudio = "uiClickBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItem = "FBItem";
    // 玩家角色音效
    public const string playerHit = "Hit";

    // 屏幕标准宽高
    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;
    public const float ScreenStandardRate = (float)ScreenStandardWidth / ScreenStandardHeight;

    // 遥感点标准距离
    public const float HandlePointDis = 98f;
    public const float SqrHandlePointDis = 9604f;

    // 移动速度
    public const float PlayerMoveSpeed = 6f;
    public const float MonsterMoveSpeed = 3f;

    // 运动平滑速度
    public const float AcceleSpeed = 5f;

    public const float HpAcceleSpeed = 0.8f;

    #region Action
    public const int ActionDefault = -1;
    #endregion

    // 死亡后物体销毁时间
    public const int DieDestoryTime = 5000;

    // 普攻连击间隔时间
    public const int ComboSpace = 800;
}


public enum TxtColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public enum EntityType
{
    None,
    Player,
    Monster
}

public enum EntityState
{
    None,
    BatiState // 霸体状态：不受控制
}
