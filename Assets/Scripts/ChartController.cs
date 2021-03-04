using UnityEngine;
using UnityEngine.UI;

namespace AwesomeCharts {
    public class ChartController : MonoBehaviour {

        public LineChart lineChart;
        private LineDataSet set = new LineDataSet();
        static public string timer;
        public Text timeText;
        public Texture fillTexture;


        [SerializeField]
        private int minute;
        [SerializeField]
        private float seconds;
        //�@�O��Update�̎��̕b��
        private float oldSeconds;
        //�@�^�C�}�[�\���p�e�L�X�g
        public Text timerText;


        private void Start () {
            // Configure line
            set.LineColor = Color.white;
            set.FillColor = new Color(0.0f, 0.7f, 0.95f, 0.7f);
            set.FillTexture = fillTexture;
            set.LineThickness = 10;
            // Add data set to chart data
            lineChart.GetChartData().DataSets.Add(set);
            // Refresh chart after data change
            lineChart.SetDirty();
        }

        private void Update()
        {
            seconds += Time.deltaTime;
            if (seconds >= 60f)
            {
                minute++;
                seconds = seconds - 60;
            }
            //�@�l���ς�����������e�L�X�gUI���X�V
            if ((int)seconds != (int)oldSeconds)
            {
                timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
            }
            oldSeconds = seconds;
            timer = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }

        public void AddChartData (int sads) {
            set.AddEntry(new LineEntry(Time.realtimeSinceStartup, sads));
            lineChart.SetDirty();
        }
    }
}