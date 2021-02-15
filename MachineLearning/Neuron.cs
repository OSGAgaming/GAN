
using System.Collections.Generic;
using System.Diagnostics;


namespace ArmourGan.MachineLearning
{
    public class Neuron
    {
        public List<float> finalLayer = new List<float>();

        public List<List<float>> finalLayerHolder = new List<List<float>>();

        public List<float> answerLayer = new List<float>();

        public List<List<float>> answerHolder = new List<List<float>>();

        public List<float> errors = new List<float>();
    }
}