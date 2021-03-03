using System;
using UnityEngine;
using UnityEngine.UI;
using AwesomeCharts;

namespace GoogleSheetsForUnity
{
    /* 
        This example will create a number of buttons on the scene, with self describing actions.
        It introduces to basic operations to handle spreadsheets with the API to make CRUD operations on:
        tables (worksheets) with fields (column headers), as well as objects (rows) on those tables.
    */
    public class SpreadsheetsExample : MonoBehaviour
    {

        public InputField memoField;

        private DateTime TodayNow;

        private static bool isNewTable; 

        // Simple struct for the example.
        [Serializable]
        public struct PlayerInfo
        {
            public string clinic;
            public string patient;
            public string contents;
            public string time;
            public string sads;
            public string datetime;
        }


        //Create an example object.
        private PlayerInfo _playerData = new PlayerInfo { clinic = "魔法アプリ", patient = "A0017KFX", contents="", time = "", sads = "",datetime="" };

        public void sadsstring(string sadsdata)
        {
            int minutes = Mathf.FloorToInt(ChartController.timer / 60F);
            int seconds = Mathf.FloorToInt(ChartController.timer - minutes * 60);
            _playerData.sads += string.Format("{0:00}:{1:00}", minutes, seconds)+" " + sadsdata +" "+memoField.text + "\n";
        }
        // For the table to be created and queried.
        private string _tableName;


        private void OnEnable()
        {
            // Suscribe for catching cloud responses.
            Drive.responseCallback += HandleDriveResponse;
        }

        private void OnDisable()
        {
            // Remove listeners.
            Drive.responseCallback -= HandleDriveResponse;
        }

        void Update()
        {
            if(isNewTable == true){
                isNewTable = false;
                CreatePlayerTable();
            }
        }

        public static void newTable()
        {
            isNewTable = true;
        }



        //ログをスプレッドシートに送る
        public void SendLog()
        {
            //クリニック名を送信
            _playerData.clinic = NaReRu_Instance.clinicName;
            _playerData.patient = NaReRu_Instance.patientID;
            _playerData.contents =SceneSelector.sceneName;

            //コンテンツの利用時間を送信
            int minutes = Mathf.FloorToInt(ChartController.timer / 60F);
            int seconds = Mathf.FloorToInt(ChartController.timer - minutes * 60);
            _playerData.time = string.Format("{0:00}:{1:00}", minutes, seconds);

            //現在時刻を送信
            TodayNow = DateTime.Now;
            _playerData.datetime = TodayNow.Year.ToString() + "/" + TodayNow.Month.ToString() + "/" + TodayNow.Day.ToString() + " " + DateTime.Now.ToLongTimeString();


            // オブジェクトのjson文字列を取得します。
            string jsonPlayer = JsonUtility.ToJson(_playerData);
            _tableName = _playerData.clinic;
            Debug.Log("<color=yellow>Sending following player to the cloud: \n</color>" + jsonPlayer);

            // クラウド上のオブジェクトを、オブジェクトタイプのように呼ばれるテーブルに保存します。
            Drive.CreateObject(jsonPlayer, _tableName, true);
        }

        public void CreatePlayerTable()
        {
            Debug.Log("<color=yellow>Creating a table in the cloud for players data.</color>");
            _tableName = NaReRu_Instance.clinicName;

            // Creating a string array for field names (table headers) .
            string[] fieldNames = new string[5];
            fieldNames[0] = "clinic";
            fieldNames[1] = "patient";
            fieldNames[2] = "contents";
            fieldNames[3] = "time";
            fieldNames[4] = "sads";
            fieldNames[5] = "datetime";

            // Request for the table to be created on the cloud.
            Drive.CreateTable(fieldNames, _tableName, true);

            //SendLog();
        }



        private void SavePlayer()
        {
            // Get the json string of the object.
            string jsonPlayer = JsonUtility.ToJson(_playerData);

            Debug.Log("<color=yellow>Sending following player to the cloud: \n</color>" + jsonPlayer);

            // Save the object on the cloud, in a table called like the object type.
            Drive.CreateObject(jsonPlayer, _tableName, true);
        }

        private void UpdatePlayer(bool create)
        {
            Debug.Log("<color=yellow>Updating cloud data: player called Mithrandir will be level 100.</color>");

            // Get the json string of the object.
            string jsonPlayer = JsonUtility.ToJson(_playerData);

            // Look in the 'PlayerInfo' table, for an object of name as specified, and overwrite with the current obj data.
            Drive.UpdateObjects(_tableName, "name", _playerData.clinic, jsonPlayer, create, true);
        }

        private void RetrievePlayer()
        {
            Debug.Log("<color=yellow>Retrieving player of name Mithrandir from the Cloud.</color>");

            // Get any objects from table 'PlayerInfo' with value 'Mithrandir' in the field called 'name'.
            Drive.GetObjectsByField(_tableName, "name", _playerData.clinic, true);
        }

        private void GetAllPlayers()
        {
            Debug.Log("<color=yellow>Retrieving all players from the Cloud.</color>");

            // Get all objects from table 'PlayerInfo'.
            Drive.GetTable(_tableName, true);
        }

        private void GetAllTables()
        {
            Debug.Log("<color=yellow>Retrieving all data tables from the Cloud.</color>");

            // Get all objects from table 'PlayerInfo'.
            Drive.GetAllTables(true);
        }

        // Processes the data received from the cloud.
        public void HandleDriveResponse(Drive.DataContainer dataContainer)
        {
            Debug.Log("datacontainermsg:" + dataContainer.msg);

            // First check the type of answer.
            if (dataContainer.QueryType == Drive.QueryType.getTable)
            {
                string rawJSon = dataContainer.payload;
                Debug.Log(rawJSon);

                // Check if the type is correct.
                if (string.Compare(dataContainer.objType, _tableName) == 0)
                {
                    // Parse from json to the desired object type.
                    PlayerInfo[] players = JsonHelper.ArrayFromJson<PlayerInfo>(rawJSon);

                    string logMsg = "<color=yellow>" + players.Length.ToString() + " objects retrieved from the cloud and parsed:</color>";
                    for (int i = 0; i < players.Length; i++)
                    {
                        logMsg += "\n" +
                            "<color=blue>Name: " + players[i].clinic + "</color>\n" +
                            "Level: " + players[i].patient + "\n" +
                            "Health: " + players[i].time + "\n" +
                            "Role: " + players[i].sads + "\n";
                    }
                    Debug.Log(logMsg);
                }
            }

            // First check the type of answer.
            if (dataContainer.QueryType == Drive.QueryType.getAllTables)
            {
                string rawJSon = dataContainer.payload;

                // The response for this query is a json list of objects that hold tow fields:
                // * objType: the table name (we use for identifying the type).
                // * payload: the contents of the table in json format.
                Drive.DataContainer[] tables = JsonHelper.ArrayFromJson<Drive.DataContainer>(rawJSon);

                // Once we get the list of tables, we could use the objTypes to know the type and convert json to specific objects.
                // On this example, we will just dump all content to the console, sorted by table name.
                string logMsg = "<color=yellow>All data tables retrieved from the cloud.\n</color>";
                for (int i = 0; i < tables.Length; i++)
                {
                    logMsg += "\n<color=blue>Table Name: " + tables[i].objType + "</color>\n" + tables[i].payload + "\n";
                }
                Debug.Log("logMsg" + logMsg);
            }
        }

    }
}