
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ArmourGan.MachineLearning
{
    public class Handwriting : NeuralNetwork
    {
        public override int sizeOfData => 40000;
        public override int IMAGEHEIGHT => 18;
        public override int IMAGEWIDTH => 20;
        public override int SIZEOFINPUTS => 72;
        public int SIZEOFUNPROCESSEDINPUTS => IMAGEHEIGHT * IMAGEWIDTH;
        public override int NumberOfKernels => 2;
        public override int NumberOfClassifications => 10;
        public override float LearningRate => 0.01f;

        public override string PerceptronSavePath => $@"\Mod Sources\EEMod\MachineLearning\PerceptronData.TrivBadMod";

        public int percDoneWithLoading;

        public float Prediction;
        public float Answer;
        double[] UIInputs;
        public List<float> Vals = new List<float>();
        public float[] GotoVals = new float[10];
        public void Clear()
        {
            for (int i = 0; i < UIInputs.Length; i++)
            {
                UIInputs[i] = 0;
            }
        }
        public void Draw()
        {
            int PixelSize = 13;
            Vector2 StartPoint = Vector2.Zero;
            Rectangle MouseSquare = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 2, 2);
            /*if(Main.LocalPlayer.controlHook)
            {
                SerliazeCurrentPerceptron();
                Main.NewText("ObjectSaved!");
            }
            if(EEMod.Train.JustPressed)
            {
                Clear();
            }
            */
            for (int i = 0; i < IMAGEWIDTH; i++)
            {
                for (int j = 0; j < IMAGEHEIGHT; j++)
                {
                    float a = (float)objTrainer[1].input[i*IMAGEHEIGHT + j] / 255;
                    Utils.DrawBoxFill(new Vector2(i* 5, j * 5), 10, 10, new Color(a, a, a));
                }
            }

            for (int i = 0; i < UIInputs.Length; i++)
            {
                Rectangle box = new Rectangle((int)StartPoint.X + (i % IMAGEWIDTH) * PixelSize, (int)StartPoint.Y + (i / IMAGEHEIGHT) * PixelSize, PixelSize, PixelSize);
                if (MouseSquare.Intersects(box) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    UIInputs[i] = 1;
                    if (i - IMAGEWIDTH > 0)
                    {
                        UIInputs[i - IMAGEWIDTH] += 0.01f;
                    }
                    if (i - 1 > 0)
                    {
                        UIInputs[i - 1] += 0.01f;
                    }
                    if (i + 1 < SIZEOFUNPROCESSEDINPUTS)
                    {
                        UIInputs[i + 1] += 0.01f;
                    }
                    if (i + IMAGEWIDTH < SIZEOFUNPROCESSEDINPUTS)
                    {
                        UIInputs[i + IMAGEWIDTH] += 0.01f;
                    }
                }
                UIInputs[i] = Math.Min(UIInputs[i], 1);
               // Main.spriteBatch.Draw(TextureCache.pixel, box, Color.Lerp(Color.Black, Color.White, (float)UIInputs[i]));
            }
            for (int i = 0; i < Vals.Count; i++)
            {
                GotoVals[i] += (Vals[i] - GotoVals[i]) / 64f;
                int Seperation = 20;
                int Length = 500;
                Rectangle box = new Rectangle((int)StartPoint.X + IMAGEWIDTH * PixelSize, (int)StartPoint.Y + i * Seperation, (int)(Length * GotoVals[i]), 20);
                //EEMod.UIText(i.ToString(), Color.White, new Vector2((int)StartPoint.X + IMAGEWIDTH * PixelSize - 10, (int)StartPoint.Y + i * Seperation), 1);
               // Main.spriteBatch.Draw(TextureCache.pixel, box, Color.Lerp(Color.Red, Color.Green, GotoVals[i]));
            }
            try
            {
                if (File.Exists(PerceptronSavePath))
                {
                    Vals = PredictPicture(UIInputs, DeserializeSavedPerceptron());
                }
            }
            catch
            {
            }
        }
        int[] convolutionalFilter1 =
        {
        -1, 0, 1,
        -1, 0, 1,
        -1, 0, 1
        };

        int[] convolutionalFilter2 =
        {
         -1,-1,-1,
          0, 0, 0,
          1, 1, 1
        };
        public override void OnUpdate()
        {
            if (isActive)
            {
                MoveToNext();
                FeedForward();
                Error();
                Train();
                Prediction = MainPerceptron.firstHiddenLayer.IndexOf(MainPerceptron.firstHiddenLayer.Max());
                float maxValue = objTrainer[CurrentData].answer.Max();
                Answer = objTrainer[CurrentData].answer.ToList().IndexOf(maxValue);
                Layers.errors.Add(ERROR);
            }
        }
        List<float> PredictPicture(double[] inputs, Perceptron ptron)
        {
            float[] KerneledInputs;
            float[][] Kernels =
               {
                 KernelMultidimensionalArray3x3<float>(inputs, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter1,1),
                 KernelMultidimensionalArray3x3<float>(inputs, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter2,1)
               };

            float[][] Pools =
              {
                 MaxPoolMultiDimensionalArray<float>(Kernels[0], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2),
                 MaxPoolMultiDimensionalArray<float>(Kernels[1], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2)
              };

            KerneledInputs = Flatten<float>(Pools);
            return ptron.FeedForward(KerneledInputs);
        }
        public override void OnInitialize()
        {
            UIInputs = new double[IMAGEWIDTH * IMAGEHEIGHT];
            isActive = true;
            MainPerceptron = new Perceptron(SIZEOFINPUTS * NumberOfKernels, SIZEOFINPUTS, SIZEOFINPUTS / 2, NumberOfClassifications, 0.01f);
            CreateNewDataSet();
            //CreateNewDataSet($@"C:\Users\tafid\source\repos\ArmourGan\ArmourGan\GAN\MachineLearning\DataSet\mnist_test");
        }
        internal string GANPath => Environment.ExpandEnvironmentVariables(@$"%UserProfile%\source\repos\ArmourGan\ArmourGan\GAN\Content\MachineLearning\DataSet\");
        public void CreateNewDataSet()
        {
            objTrainer = new Trainer[sizeOfData];
            string path = $"{GANPath}DalantiniumGreathelm.png";
            System.Drawing.Bitmap BM = new System.Drawing.Bitmap(path);
            double[] input = new double[BM.Width * BM.Height];
            for (int i = 0; i<BM.Width; i++)
            {
                for (int j = 0; j < BM.Height; j++)
                {
                    input[j + i * BM.Width] = (BM.GetPixel(i, j).R + BM.GetPixel(i, j).G + BM.GetPixel(i, j).B)/3f;
                }
            }
            float[] answers = new float[NumberOfClassifications];
           
            float[][] Kernels =
                {
                       KernelMultidimensionalArray3x3<float>(input, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter1,1),
                       KernelMultidimensionalArray3x3<float>(input, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter2,1)
                     };

            float[][] Pools =
            {
                       MaxPoolMultiDimensionalArray<float>(Kernels[0], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2),
                       MaxPoolMultiDimensionalArray<float>(Kernels[1], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2)
                      };
            for (int i = 0; i < sizeOfData; i++)
            {
                objTrainer[i] = new Trainer(input, answers);
                objTrainer[i].kerneledInputs = Flatten<float>(Pools);

            }
        }
        
        public void CreateNewDataSet(string TrainingInputsExcel)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(TrainingInputsExcel);
            Excel._Worksheet xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            dynamic[,] excelArray = xlRange.Value2 as dynamic[,];
            objTrainer = new Trainer[sizeOfData];
            CurrentData = 0;
            for (int i = 0; i < objTrainer.Length; i++)
            {
                percDoneWithLoading = i;
                int currentRow = Main.rand.Next(2, xlRange.Rows.Count - 1);
                double[] inputs = new double[SIZEOFUNPROCESSEDINPUTS];
                for (int a = 0; a < SIZEOFUNPROCESSEDINPUTS; a++)
                {
                    inputs[a] = excelArray[currentRow, a + 1] / (double)255;
                }
                float[] answers = new float[NumberOfClassifications];
                answers[(int)excelArray[currentRow, 1]] = 1;
                objTrainer[i] = new Trainer(inputs, answers);

                float[][] Kernels =
                {
                       KernelMultidimensionalArray3x3<float>(inputs, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter1,1),
                       KernelMultidimensionalArray3x3<float>(inputs, IMAGEWIDTH, IMAGEHEIGHT, convolutionalFilter2,1)
                     };

                float[][] Pools =
                {
                       MaxPoolMultiDimensionalArray<float>(Kernels[0], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2),
                       MaxPoolMultiDimensionalArray<float>(Kernels[1], IMAGEWIDTH - 2,IMAGEHEIGHT - 2,2,2)
                      };

                objTrainer[i].kerneledInputs = Flatten<float>(Pools);
            }
            Console.WriteLine("Done!");
            xlWorkbook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkbook);
            Marshal.ReleaseComObject(xlApp);
        }
    }

}


