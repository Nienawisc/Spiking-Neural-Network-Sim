using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace Testy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Random rand = new Random();
        SpikingPerceptron perceptron;
        bool[] input;
        float[] weight;
        int licznik_wag = 0;
        int licznik_cykli = 0;
        Thread wizualizacja = null;
        bool czy_losowac = true;
        bool stworz_series = false;
        List<int> tablica = new List<int>();
        List<SpikingPerceptron[]> siec = new List<SpikingPerceptron[]>();
        TextBox[] textboxes = new TextBox[1000];

        private void wizualizuj()
        {
            while(true)
            {
                string append = "";
                licznik_cykli++;
                append += string.Format("Cykl {0}:\n", licznik_cykli);
                int freq = 1;

                bool[] table1 = { false, false, false, true };
                bool[] table2 = { false, false, true, true };
                bool[] table3 = { false, true, true, true };

                if (radioButton1.Checked == true) freq = 0;
                if (radioButton2.Checked == true) freq = 1;
                if (radioButton3.Checked == true) freq = 2;

                for (int j = 0; j < input.Length; j++)
                {
                    switch (freq)
                    {
                        case 0:
                            input[j] = Convert.ToBoolean(table1[rand.Next(0, 4)]);
                            break;
                        case 1:
                            input[j] = Convert.ToBoolean(table2[rand.Next(0, 4)]);
                            break;
                        case 2:
                            input[j] = Convert.ToBoolean(table3[rand.Next(0, 4)]);
                            break;
                    }
                }
                perceptron.Input = input;
                perceptron.Weight = weight;
                perceptron.Start();
                if (stworz_series == false)
                {
                    for (int j = 2; j < input.Length+1; j++)
                    {
                        chart3.Series.Add("Wejście " + j);
                        chart3.Series["Wejście " + j].ChartType = SeriesChartType.Line;
                        chart3.Series["Wejście " + j].BorderWidth = 3;
                    }
                    stworz_series = true;
                }
                for (int j = 0; j < input.Length; j++)
                {
                    append += string.Format("Input: {0} – {1}\n", perceptron.Input[j], perceptron.Weight[j]);
                    AddPointToChart_wejscia(input[j]);
                }   
                append += string.Format("Output: {0} \n", perceptron.Output);
                append += string.Format("Potencjał na membranie: {0} \n", perceptron.PotentialOnTheMembrane);
                append += "=================\n";
                AppendrichTextBox1(append);
                AddPointToChart_wyjscia(perceptron.Output);
                AddPointToChart_membrana(perceptron.PotentialOnTheMembrane);
                //siec
                for (int i = 0; i < tablica[0]; i++)
                {
                    siec.ElementAt(0)[i].Input = input;
                    if(i!=0)
                    {
                        siec.ElementAt(0)[i].weightGen(rand);
                    }
                    else siec.ElementAt(0)[i].Weight = weight;
                }
                 for (int i = 1; i < Int32.Parse(textBox2.Text); i++)
                 {
                     for (int j = 0; j < tablica[i]; j++)
                     {
                         bool[] oldinput = new bool[tablica[i - 1]];
                         for (int z = 0; z < tablica[i - 1]; z++)
                         {
                             oldinput[z] = siec[i - 1].ElementAt(z).Output;
                         }
                         siec[i].ElementAt(j).Input = oldinput;
                        siec[i].ElementAt(j).weightGen(rand);
                     }
                 }
                int txt = 0;
                foreach (var warstwa in siec)
                {
                    foreach(var neuron in warstwa)
                    {
                        neuron.Start();
                        Appendtextbox(neuron.Output ? "1" : "0", txt);
                        txt++;
                    }
                }
                AddPointToChart_Wyjście(siec.ElementAt(siec.Count-1)[0].Output);
                if (czy_losowac)losuj();
                Thread.Sleep(1000);
            }
        }

        public void Appendtextbox(string value, int index)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string,int>(Appendtextbox), new object[] { value, index });
                return;
            }
            textboxes[index].Text = value;
        }
        public void AppendrichTextBox1(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendrichTextBox1), new object[] { value });
                return;
            }
            richTextBox1.Text += value;
        }
        public void SetrichTextBox2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(SetrichTextBox2), new object[] { value });
                return;
            }
            richTextBox2.Text = value;
        }

        public void AddPointToChart_wyjscia(bool Y)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(AddPointToChart_wyjscia), new object[] { Y });
                return;
            }
            chart1.Series["Wyjście"].Points.AddY(Y);
        }
        public void AddPointToChart_membrana(float Y)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<float>(AddPointToChart_membrana), new object[] { Y });
                return;
            }
            chart2.Series["Potencjał"].Points.AddY(Y);
        }

        int licznik_wejscia=1;

        public void AddPointToChart_wejscia(bool Y)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(AddPointToChart_wejscia), new object[] { Y });
                return;
            }
            chart3.Series["Wejście "+licznik_wejscia].Points.AddY(Y);
            licznik_wejscia++;
            if (licznik_wejscia > input.Length) licznik_wejscia = 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            perceptron = new SpikingPerceptron(Int32.Parse(textBox1.Text));
            input = new bool[Int32.Parse(textBox1.Text)];
            weight = new float[Int32.Parse(textBox1.Text)];
            textBox4.Text = "1";
        }
        private bool start = false;
        private void button1_Click_1(object sender, EventArgs e)
        {
            if(start)
            {
                button1.Text = "Start";
                button7.Text = "Start";
                wizualizacja.Abort();
            }
            else
            {
                button1.Text = "Stop";
                button7.Text = "Stop";
                wizualizacja = new Thread(wizualizuj);
                wizualizacja.Start();
            }
            start = !start;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            czy_losowac = true;
            losuj();
        }
        private void losuj()
        {
            string setText = "";
            int maxi = 100000;
            int current = Math.Max((int)(100000 * perceptron.PotentialOnTheMembrane * 1.1), 100000);
            for (int i = 0; i < weight.Length; i++)
            {
                if (i + 1 != weight.Length)
                {
                    int ran = rand.Next(1, current);
                    weight[i] = (float)(ran) / (float)maxi;
                    current -= ran;
                    setText += weight[i].ToString() + "\n";
                }
                else
                {
                    weight[i] = (float)current / (float)maxi;
                    setText += weight[i].ToString();
                }
            }
            SetrichTextBox2(setText);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            czy_losowac = false;
            if (licznik_wag<weight.Length)
            {
                if (textBox5.Text == "") textBox5.Text = "0";
                textBox5.Text = textBox5.Text.Replace('.', ',');
                
                weight[licznik_wag] =float.Parse(textBox5.Text);
                richTextBox2.Text += weight[licznik_wag].ToString() + "\n";
                textBox4.Text = (licznik_wag + 1).ToString();
                licznik_wag++;      
            }
            else
            {
                richTextBox2.Text = "";
                licznik_wag = 0;
                button3_Click(sender, e);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz zakończyć program?", "Spiking perceptron",
    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(wizualizacja!=null)if(wizualizacja.IsAlive)wizualizacja.Abort();
            }
            else
            {
                e.Cancel = true;
            }
        }
        private void wejściaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
        }
        private void neuronToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
        }

        PictureBox[] boxes = new PictureBox[1000];
        int warstwa = 1;
        int ilosc_warstw;

        private void button5_Click(object sender, EventArgs e)
        {
            ilosc_warstw = Int32.Parse(textBox2.Text);
        }

        private void siećNeuronowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
        }

        PictureBox[] boxes2 = new PictureBox[1000];
        int location1 = 12;
        int location2 = 124;
        int UstawianaWarstwa = 0;
        int poprzedniaIloscneuronow = 0;
        int sumaNeuronow = 0;

        private void button6_Click(object sender, EventArgs e)
        {
            if (warstwa < Int32.Parse(textBox2.Text) + 1)
            {
                int ilosc_neuronow = Int32.Parse(textBox3.Text);
                for(int i=sumaNeuronow;i<ilosc_neuronow+sumaNeuronow;i++)
                {
                    textboxes[i] = new TextBox();
                    textboxes[i].Location = new System.Drawing.Point(location1 + 30, location2 + 30);
                    textboxes[i].Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
                    textboxes[i].Name = "textBox" + (5 + Int32.Parse(textBox2.Text) + 1);
                    textboxes[i].Size = new System.Drawing.Size(30, 33);
                    panel2.Controls.Add(textboxes[i]);

                    boxes2[i] = new PictureBox();
                    boxes2[i].Image = global::Testy.Properties.Resources._367725c65f62e46d7d9c4fdf5d1c8b3d;
                    boxes2[i].Location = new System.Drawing.Point(location1, location2);
                    boxes2[i].Name = "pictureBox" + (3 + Int32.Parse(textBox2.Text) + 1);
                    boxes2[i].Size = new System.Drawing.Size(89, 88);        
                    boxes2[i].Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
                    boxes2[i].TabStop = false;
                    panel2.Controls.Add(boxes2[i]);

                    location2 += 100;
                }
                tablica.Add(Int32.Parse(textBox3.Text));

                if(UstawianaWarstwa==0)
                { 
                    siec.Add(new SpikingPerceptron[tablica[0]]);

                    for (int i = 0; i < tablica[0]; i++)
                    {
                        siec.ElementAt(0)[i] = new SpikingPerceptron(tablica[0]);
                    }
                }
                else
                {
                    siec.Add(new SpikingPerceptron[tablica[UstawianaWarstwa]]);

                    for (int i = 0; i < tablica[UstawianaWarstwa]; i++)
                    {
                        siec.ElementAt(UstawianaWarstwa)[i] = new SpikingPerceptron(tablica[UstawianaWarstwa - 1]);
                    }
                }
                poprzedniaIloscneuronow = ilosc_neuronow;
                UstawianaWarstwa++;
                sumaNeuronow += ilosc_neuronow;
                if (ilosc_warstw-1 < UstawianaWarstwa)
                {
                    UstawianaWarstwa = 0;
                }
                warstwa++;
                location2 = 124;
                location1 += 100;
                label3.Text = "Podaj ilość neuronów warstwy nr " + (UstawianaWarstwa + 1).ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button1_Click_1(sender, e);
        }

        public void AddPointToChart_Wyjście(bool Y)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(AddPointToChart_Wyjście), new object[] { Y });
                return;
            }
            chart4.Series["Wyjście"].Points.AddY(Y);
        }
    }
}
