using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GoogleSheetsForUnity
{
    /* 
        This example will create a number of buttons on the scene, with self describing actions.
        It introduces to basic operations to handle spreadsheets with the API to make CRUD operations on:
        tables (worksheets) with fields (column headers), as well as objects (rows) on those tables.
    */
    public class Login : MonoBehaviour
    {
        public InputField clinicidField;
        public InputField patientidField;
        public InputField passwordField;

        [Serializable]
        public struct LoginInfo
        {
            public string Clinicid;
            public string Password;
            public string clinicName;
        }


        [Serializable]
        public struct PatientInfo
        {
            public string patient;
        }


        public GameObject ErrorText;

        public Toggle clinicToggle;
        public Toggle passwordToggle;

        private static bool error;
        private bool clinicID,patientID,password;


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


        void Start()
        {
            //IDもパスワードも保存していない
            if ((PlayerPrefs.GetInt("CHECK") == 0))
            {
                clinicToggle.isOn = false;
                passwordToggle.isOn = false;
            }

            //IDは保存しているがパスワードは保存していない
            if ((PlayerPrefs.GetInt("CHECK")==1))
            {
                clinicToggle.isOn = true;
                passwordToggle.isOn = false;
                clinicidField.text = (PlayerPrefs.GetString("ID"));
            }

            //IDは保存していないがパスワードは保存している
            if ((PlayerPrefs.GetInt("CHECK") == 2))
            {
                clinicToggle.isOn = false;
                passwordToggle.isOn = true;
                passwordField.text = (PlayerPrefs.GetString("PASSWORD"));
            }

            //IDもパスワードも保存している
            if ((PlayerPrefs.GetInt("CHECK") == 3))
            {
                clinicToggle.isOn = true;
                passwordToggle.isOn = true;
                clinicidField.text = (PlayerPrefs.GetString("ID"));
                passwordField.text = (PlayerPrefs.GetString("PASSWORD"));
            }

        }
        void Update()
        {
            if (clinicToggle.isOn == false)
            {
                if(passwordToggle.isOn == false)
                {
                    PlayerPrefs.SetInt("CHECK", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("CHECK", 2);
                    PlayerPrefs.SetString("PASSWORD",passwordField.text);
                }
            }
            else //IDは保存している
            {
                if (passwordToggle.isOn == false)
                {
                    PlayerPrefs.SetInt("CHECK", 1);
                    PlayerPrefs.SetString("ID", clinicidField.text);
                }
                else
                {
                    PlayerPrefs.SetInt("CHECK", 3);
                    PlayerPrefs.SetString("ID", clinicidField.text);
                    PlayerPrefs.SetString("PASSWORD", passwordField.text);
                }
            }

            //アカウント情報にエラーがある場合
            if (error == true)
            {
                GameObject.Find("Button_Login").gameObject.GetComponent<Button>().interactable = true;
                errorAlert();
            }

            //クリニック　患者　パスワードがすべて合致するとシーン遷移
            if (clinicID == true && patientID == true && password == true)
            {
                SceneManager.LoadScene("Player");
            }
        }

        public void LoginMethod()
        {
            ErrorText.SetActive(false);

            string id = clinicidField.text;
            string pw = passwordField.text;
            string patientid2 = patientidField.text;
            Drive.GetObjectsByField("ログイン", "clinicid", id, true);
            Drive.GetObjectsByField("患者ID", "patient", patientid2, true);


            if (clinicToggle.isOn == false)
            {
                if (passwordToggle.isOn == false)
                {
                    PlayerPrefs.SetInt("CHECK", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("CHECK", 2);
                }
            }
            else //IDは保存している
            {
                if (passwordToggle.isOn == false)
                {
                    PlayerPrefs.SetInt("CHECK", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("CHECK", 3);
                }
            }

        }

        public static void accounterror()
        {
            Debug.Log("アカウントが違うよ");
            error = true;
           

        }

        public void errorAlert()
        {
            error = false;
            ErrorText.SetActive(true);
        
        }

        // Processes the data received from the cloud.
        public void HandleDriveResponse(Drive.DataContainer dataContainer)
        {
            Debug.Log("datacontainermsg:" + dataContainer.msg);

            // First check the type of answer.
            if (dataContainer.QueryType == Drive.QueryType.getObjectsByField)
            {
                string rawJSon = dataContainer.payload;
                Debug.Log("参照:" + rawJSon);


                // Check if the type is correct.
                if (string.Compare(dataContainer.objType, "ログイン") == 0)
                {
                    // Parse from json to the desired object type.
                    LoginInfo[] clinicData = JsonHelper.ArrayFromJson<LoginInfo>(rawJSon);

                    string pw = passwordField.text;
                    NaReRu_Instance.clinicName = clinicData[0].clinicName;

                    if (pw == clinicData[0].Password)
                    {
                        clinicID = true;
                        password = true;

                    }
                    else
                    {
                        Debug.Log("パスワードが違うよ！");
                        errorAlert();
                    }
                }


                //患者ID参照
                if (string.Compare(dataContainer.objType, "患者ID") == 0)
                {
                    // Parse from json to the desired object type.
                    PatientInfo[] patientData = JsonHelper.ArrayFromJson<PatientInfo>(rawJSon);
                    string patient = patientidField.text;

                    NaReRu_Instance.patientID = patient;

                    if (patient == patientData[0].patient)
                    {
                        Debug.Log(patientData[0].patient);
                        patientID = true;
                        Debug.Log("成功！");
                    }
                    else
                    {
                        Debug.Log(patientData[0].patient);
                        Debug.Log("患者IDが違うよ2！");
                        errorAlert();

                    }

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