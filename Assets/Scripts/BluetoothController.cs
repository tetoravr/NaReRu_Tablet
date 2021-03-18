using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SVSBluetooth;
using System.Text;
using System;

public class BluetoothController : MonoBehaviour
{
    public Image BluetoothImage; // a picture that displays the status of the bluetooth adapter upon request
    public Image VRImage; // a picture that displays the status of the bluetooth adapter upon request
    public Image TabletImage; // a picture that displays the status of the bluetooth adapter upon request
    public Text textField; // field for displaying messages and events
    const string MY_UUID = "0b7062bc-67cb-492b-9879-09bf2c7012b2"; // UUID constant which is set via script
    //00001101-0000-1000-8000-00805F9B34FB for arduino

    BluetoothForAndroid.BTDevice[] devices;
    string lastConnectedDeviceAddress;

    private float span = 10.0f;
    private float currentTime = 0f;

    public Camera camerarot;
    string[] camRot;
    float cameraRotx;
    float cameraRoty;
    float cameraRotz;

    bool btEnable;
    public AudioClip connectSound;
    private AudioSource audioSource;

    private void Start()
    {
        Initialize();
        EnableBT();

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = connectSound;

        Invoke("GetBluetoothStatus", 0.5f);

    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > span)
        {
        GetBluetoothStatus();
            currentTime = 0f;
        }
    }


    // subscription and unsubscribe from events. You can read more about events in Documentation.pdf
    private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        BluetoothForAndroid.ReceivedFloatMessage += PrintVal2;
        BluetoothForAndroid.ReceivedStringMessage += PrintVal3;
        BluetoothForAndroid.ReceivedByteMessage += PrintVal4;

        BluetoothForAndroid.BtAdapterEnabled += PrintEvent1;
        BluetoothForAndroid.BtAdapterDisabled += PrintEvent2;
        BluetoothForAndroid.DeviceConnected += PrintEvent3;
        BluetoothForAndroid.DeviceDisconnected += PrintEvent4;
        BluetoothForAndroid.ServerStarted += PrintEvent5;
        BluetoothForAndroid.ServerStopped += PrintEvent6;
        BluetoothForAndroid.AttemptConnectToServer += PrintEvent7;
        BluetoothForAndroid.FailConnectToServer += PrintEvent8;

        BluetoothForAndroid.DeviceSelected += PrintDeviceData;
    }
    private void OnDisable()
    {
        BluetoothForAndroid.ReceivedFloatMessage -= PrintVal2;
        BluetoothForAndroid.ReceivedStringMessage -= PrintVal3;
        BluetoothForAndroid.ReceivedByteMessage -= PrintVal4;


        BluetoothForAndroid.BtAdapterEnabled -= PrintEvent1;
        BluetoothForAndroid.BtAdapterDisabled -= PrintEvent2;
        BluetoothForAndroid.DeviceConnected -= PrintEvent3;
        BluetoothForAndroid.DeviceDisconnected -= PrintEvent4;
        BluetoothForAndroid.ServerStarted -= PrintEvent5;
        BluetoothForAndroid.ServerStopped -= PrintEvent6;
        BluetoothForAndroid.AttemptConnectToServer -= PrintEvent7;
        BluetoothForAndroid.FailConnectToServer -= PrintEvent8;

        BluetoothForAndroid.DeviceSelected -= PrintDeviceData;
    }

    // Initially, always initialize the plugin.
    public void Initialize()
    {
        BluetoothForAndroid.Initialize();
    }

    public void bluetoothReset()
    {
        BluetoothForAndroid.Initialize();
        ConnectToServer();
    }

    // ブルートゥースを制御し、その状態を取得する
    public void GetBluetoothStatus()
    {
        if (BluetoothForAndroid.IsBTEnabled())
        {
            BluetoothImage.color = Color.green;
            BluetoothImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OK";

            if(btEnable == false)
            {
                audioSource.Play();
                btEnable = true;
                Invoke("ConnectToServer", 1.0f);
            }
        }
        else 
        {
            BluetoothImage.color = Color.red;
            BluetoothImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "NG";
            btEnable = false;
        }
    }
    public void EnableBT()
    {
        BluetoothForAndroid.EnableBT();
        TabletImage.color = Color.green;
        TabletImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OK";
        //audioSource.Play();
    }
    public void DisableBT()
    {
        BluetoothForAndroid.DisableBT();
        TabletImage.color = Color.red;
        TabletImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "NG";
        btEnable = false;
    }

    // サーバの作成、停止、サーバへの接続、切断の方法
    public void CreateServer()
    {
        BluetoothForAndroid.CreateServer(MY_UUID);
    }
    public void StopServer()
    {
        BluetoothForAndroid.StopServer();
    }
    public void ConnectToServer()
    {
        BluetoothForAndroid.ConnectToServer(MY_UUID);

    }
    public void Disconnect()
    {
        BluetoothForAndroid.Disconnect();
    }
    public void ConnectToServerByAddress()
    {
        if (devices != null)
        {
            if (devices[0].address != "none") BluetoothForAndroid.ConnectToServerByAddress(MY_UUID, devices[0].address);
        }
    }
    public void ConnectToLastServer()
    {
        if (lastConnectedDeviceAddress != null) BluetoothForAndroid.ConnectToServerByAddress(MY_UUID, lastConnectedDeviceAddress);
    }

    // methods for sending messages of various types
    public void ControlVideo(int num)
    {
        BluetoothForAndroid.WriteMessage(num);
    }

    // 受信メッセージ表示手段

    //バッテリー残量取得
    void PrintVal2(float val)
    {
        VRImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = val.ToString() + "%";
    }

    //カメラ回転タブレットへ送信
    void PrintVal3(string val)
    {
        camRot = val.Split(',');
        cameraRotx = float.Parse(camRot[0]);
        cameraRoty = float.Parse(camRot[1]);
        cameraRotz = float.Parse(camRot[2]);
        camerarot.transform.rotation = Quaternion.Euler(cameraRotx, cameraRoty, cameraRotz);
    }


    void PrintVal4(byte[] val)
    {
        foreach (var item in val)
        {
            textField.text += item;
        }
        textField.text += "\n";
    }
    public void GetBondedDevices()
    {
        devices = BluetoothForAndroid.GetBondedDevices();
        if (devices != null)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                textField.text += devices[i].name + "   ";
                textField.text += devices[i].address;
                textField.text += "\n";
            }
        }
    }

    // methods for displaying events on the screen
    void PrintEvent1()
    {
        textField.text += "Adapter enabled" + "\n";
    }
    void PrintEvent2()
    {
        textField.text += "Adapter disabled" + "\n";
    }
    void PrintEvent3()
    {
        VRImage.color = Color.green;
        VRImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OK";
        audioSource.Play();
        textField.text += "The device is connected" + "\n";
    }
    void PrintEvent4()
    {
        VRImage.color = Color.red;
        VRImage.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "NG";
        textField.text += "Device lost connection" + "\n";
    }
    void PrintEvent5()
    {
        textField.text += "Server is running" + "\n";
    }
    void PrintEvent6()
    {
        textField.text += "Server stopped" + "\n";
    }
    void PrintEvent7()
    {
        textField.text += "Attempt to connect to server" + "\n";
    }
    void PrintEvent8()
    {
        textField.text += "Connection to the server failed" + "\n";
    }
    void PrintDeviceData(string deviceData)
    {
        string[] btDevice = deviceData.Split(new char[] { ',' });
        textField.text += btDevice[0] + "   ";
        textField.text += btDevice[1] + "\n";
        lastConnectedDeviceAddress = btDevice[1];
    }

    // method for cleaning the log
    public void ClearLog()
    {
        textField.text = "";
    }

 
}
