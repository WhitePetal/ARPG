/*********************************************************
	文件：AudioSev
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 16:13:10
	功能：声音播放服务
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSev : ServiceBase<AudioSev>
{
    private AkEvent bgAudioEvent;
    private AkEvent uiAudioEvent;

    public override void InitSev()
    {
        bgAudioEvent = GameRoot.Instance.transform.Find("BGAudio").GetComponent<AkEvent>();
        uiAudioEvent = GameRoot.Instance.transform.Find("UIAudio").GetComponent<AkEvent>();
        Debug.Log("Init AudioSev...");
    }

    public void PlayBGM(string name)
    {
        AkSoundEngine.SetSwitch("BGMSwitch", name, bgAudioEvent.gameObject);
    }
    public void StopBGM()
    {
        AkSoundEngine.SetSwitch("BGMSwitch", "Nothing", bgAudioEvent.gameObject);
    }

    public void PlayUIAudio(string name)
    {
        AkSoundEngine.SetSwitch("UIAudioSwitch", name, uiAudioEvent.gameObject);
        uiAudioEvent.HandleEvent(uiAudioEvent.gameObject);
    }
}
