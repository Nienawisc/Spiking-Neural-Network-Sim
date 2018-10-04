using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testy
{
    class SpikingPerceptron
    {
        private bool[] input;
        private float[] weight;
        private bool output;
        private float potentialOnTheMembrane = 0.75f;
        private float timeOfLastTrue = 1;

        public bool[] Input { get => input; set => input = value; }
        public float[] Weight { get => weight; set => weight = value; }
        public bool Output { get => output; set => output = value; }
        public float PotentialOnTheMembrane { get => potentialOnTheMembrane; set => potentialOnTheMembrane = value; }
        public float TimeOfLastTrue { get => timeOfLastTrue; set => timeOfLastTrue = value; }

        public SpikingPerceptron()
        { 
            input = new bool[2];
        }
        public SpikingPerceptron(int NOinput)
        {
            input = new bool[NOinput];
            Weight = new float[NOinput];
        }
        public void Start()
        {
            Output = Activate();
        }
        private float sum()
        {
            float s = 0;
            for (int i = 0; i < input.Length; i++) s += (Convert.ToInt32(input[i]) * weight[i]);
            return s;
        }
        private bool Activate()
        {
            if (sum() < PotentialOnTheMembrane)
            {
                for (int i = 0; i < weight.Length; i++)
                {
                    PotentialOnTheMembrane += (weight[i] / timeOfLastTrue) / 8;
                }
                TimeOfLastTrue += 0.1f;

                return false;
            }
            else
            {
                PotentialOnTheMembrane = 0.75f;
                TimeOfLastTrue = 1;
                return true;
            }
        }
        public void weightGen(Random rand)
        {
            weight = new float[input.Length];
            for (int i = 0; i < weight.Length; i++)
            {
                    int ran = rand.Next(1, (int)(potentialOnTheMembrane*1.1f* 10000f));
                    weight[i] = (float)((ran) / 10000f);
            }
        }
    }
}
