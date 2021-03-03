using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaReRu_Instance : MonoBehaviour
{

    public static string clinicName="";
    public static string patientId;
    public static string contentName;
    public static string patientID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(clinicName);
        }
    }

    public void ClinicName()
    {
       
    }
}
