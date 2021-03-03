using UnityEngine;

namespace AwesomeCharts {
    public class ChartController : MonoBehaviour {

        public LineChart lineChart;
        private LineDataSet set = new LineDataSet();
        static public float timer;

        private void Start () {
           
        }

        public void AddChartData (int sads) {

            timer = Time.realtimeSinceStartup;
            // Add entries to data set
            set.AddEntry(new LineEntry(Time.realtimeSinceStartup, sads));

            // Configure line
            set.LineColor = Color.white;
            set.FillColor = new Color(0.0f, 0.18f, 0.1f, 0.3f);
            set.LineThickness = 4;
            // Add data set to chart data
            lineChart.GetChartData().DataSets.Add(set);
            // Refresh chart after data change
            lineChart.SetDirty();
        }
    }
}