using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{

    public Sprite[] contentsImages;
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
                break;
            case 1:
                sceneName = "地下鉄";
                break;
            case 2:
                sceneName = "新幹線";
                break;
            case 3:
                sceneName = "バス";
                break;
            case 4:
                sceneName = "雷";
                break;
            case 5:
                sceneName = "高所";
                break;
            case 6:
                sceneName = "スピーチ";
                break;
            case 7:
                sceneName = "面接";
                break;
        }
    }

    public void onClick()
    {
        Debug.Log(sceneName + "の動画を再生");
    }
}
