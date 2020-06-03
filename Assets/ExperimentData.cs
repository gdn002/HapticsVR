using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

public class ExperimentData
{
    public static ExperimentData DataFile { get; private set; }

    struct DataLine
    {
        public string sourceObject;
        public float requiredForce;
        public float hitForce;
        public float forceDelta;
        public float amplitude;
    }

    private string experiment;
    private List<DataLine> data;

    public ExperimentData(string experiment)
    {
        this.experiment = experiment;
        data = new List<DataLine>();

        DataFile = this;
    }

    public void AddData(string sourceObject, float requiredForce, float hitForce, float amplitude)
    {
        DataLine line;
        line.sourceObject = sourceObject;
        line.requiredForce = requiredForce;
        line.hitForce = hitForce;
        line.forceDelta = hitForce - requiredForce;
        line.amplitude = amplitude;

        data.Add(line);
    }

    public void WriteToFile()
    {
        StringBuilder fileData = new StringBuilder();
        fileData.AppendLine("Hit Sequence;Source Object;Required Force;Hit Force;Force Delta;Amplitude");

        for (int i = 0; i < data.Count; i++)
        {
            DataLine line = data[i];
            fileData.AppendLine(i + ";" + line.sourceObject + ";" + line.requiredForce + ";" + line.hitForce + ";" + line.forceDelta + ";" + line.amplitude);
        }

        Debug.Log("Saving experiment data...");
        Debug.Log(fileData);

        string filepath = "./" + experiment + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".csv";
        StreamWriter outStream = System.IO.File.CreateText(filepath);
        outStream.WriteLine(fileData);
        outStream.Close();
    }
}
