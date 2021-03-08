using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneSelector : MonoBehaviour
{

    public Sprite[] contentsImages;

    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    public Button startButton;
    public static int sceneNum;
    public static string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneSelect(int num)
    {
        sceneNum = num;
        gameObject.GetComponent<Image>().sprite = contentsImages[num];

        switch (num)
        {
            case 0:
                sceneName = "飛行機";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[0];
                break;
            case 1:
                sceneName = "地下鉄";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[1];
                break;
            case 2:
                sceneName = "新幹線";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[2];
                break;
            case 3:
                sceneName = "バス";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[3];
                break;
            case 4:
                sceneName = "雷";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[4];
                break;
            case 5:
                sceneName = "高所";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[5];
                break;
            case 6:
                sceneName = "スピーチ";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[6];
                break;
            case 7:
                sceneName = "面接";
                videoPlayer.Stop();
                videoPlayer.clip = videoClips[7];
                break;
        }
    }

    public void onClick()
    {
        Debug.Log(sceneName + "の動画を再生");
    }
}
