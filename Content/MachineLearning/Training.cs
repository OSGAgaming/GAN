
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace ArmourGan.MachineLearning
{
    public class Trainer
    {
        public double[] inputs;
        public float[] kerneledInputs;
        public float[] answer;
        public List<float> term = new List<float>();
        public double[] input;
        public Trainer(double[] input, float[] a)
        {
            this.input = input;
            answer = a;
        }
    }
}