using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TransformataHougha.Service;

namespace TransformataHougha.UI.Views
{
    public partial class MainView : Form
    {
        private DataPicture dataPicture;
        private LoadImage loadImage;
        private Hough hough;
        private StatisticData statisticData;

        private Bitmap sobelPictureOriginal;

        private int lineDetect = 1;
        private int distribution = 0;
        private int deviation = 1;
        private bool allLine = true;

        private bool isNoise = false;
        private bool isCleanPictureHoughCompleted = false;
        private bool isNoisePictureHoughCompleted = false;

        private decimal probability = 15;

        public MainView()
        {
            InitializeComponent();

            this.dataPicture = new DataPicture();
            this.loadImage = new LoadImage();
        }


        private async void sobelDetection_Completed(object sender, EventArgs e)
        {
            await HandleAction(() => 
            {
                var image = sender as Bitmap;

                if (image != null)
                {
                    this.sobelPictureOriginal = new Bitmap(image);
                    this.statisticData = new StatisticData(image.Width, image.Height);

                    processingImage.Image = image;
                }
                else
                {
                    MessageBox.Show("Błąd, przy wykrywaniu krawędzi");
                }

                FinishTask();

                return StartAddCleanPointsAsync();
            });
        }

        private async Task StartAddCleanPointsAsync()
        {
            await Task.Run(() =>
                {
                    var h = new Hough(sobelPictureOriginal, System.Drawing.Imaging.PixelFormat.Format24bppRgb, allLine);
                    h.GetPoint += h_GetPoint;
                    h.Completed += h_Completed;
                    h.Transform(lineDetect);
                });
        }

        /// <summary>
        /// Event fire when hough is completed in clean picture 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void h_Completed(object sender, EventArgs e)
        {
            this.isCleanPictureHoughCompleted = true;

            GenerateRaportCompleted();
        }

        /// <summary>
        /// Get point clean picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void h_GetPoint(object sender, EventArgs e)
        {
            var p = sender as Point?;

            if (p.HasValue && statisticData != null)
            {
                statisticData.AddCleanPoint(p.Value);
            }
        }

        private void wczytajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                String filter = "JPEG files (*.jpg)|*.jpg| Bitmaps (*.bmp)|*.bmp";

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                if (filter.Length > 0) { dlg.Filter = filter; }

                if (dlg.ShowDialog(this) != DialogResult.Cancel)
                {
                    if (dlg.FileName != null)
                    {
                        loadImage.LoadImageFromPath(dlg.FileName);
                        dataPicture.LoadPicture(loadImage.GetPicture, loadImage.GetPixelFormat);
                        originalImage.Image = loadImage.GetPicture;

                        this.isNoise = false;
                        this.isCleanPictureHoughCompleted = false;
                        this.isNoisePictureHoughCompleted = false;
                    }
                }
            });
        }

        /// <summary>
        /// Click event to start edge detection Sobel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void detekcjaSobelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var sobelDetection = new SobelDetection(this.originalImage.Image);
            sobelDetection.Completed += sobelDetection_Completed;

            await HandleAction(() =>
            {
                StartTask();
                return Task.Run(() => sobelDetection.SobelEdgeDetection());
            });
        }

        private void transferButton_Click(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                if (processingImage.Image == null)
                {
                    MessageBox.Show("Nie udało się przenieść obrazku.", "Warning");
                }
                else
                {
                    this.originalImage.Image = new Bitmap(this.processingImage.Image);
                    this.processingImage.Image = default(Bitmap);
                }
            });
        }

        /// <summary>
        /// Click event to start Hough transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void transformataHoughaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await HandleAction(() =>
            {
                this.hough = new Hough(originalImage.Image as Bitmap, System.Drawing.Imaging.PixelFormat.Format24bppRgb, this.allLine);

                StartTask();

                hough.Completed += hough_Completed;
                hough.GetPoint += hough_GetPoint;

                return Task.Run(() => hough.Transform(lineDetect));
            });
        }

        /// <summary>
        /// Get draw points from noise picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void hough_GetPoint(object sender, EventArgs e)
        {
            var p = sender as Point?;

            if (p.HasValue && statisticData != null /*&& isNoise*/)
            {
                statisticData.AddNoisePoint(p.Value);
            }
        }

        /// <summary>
        /// Hough completed transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hough_Completed(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                var list = sender as List<Bitmap>;

                this.isNoisePictureHoughCompleted = true;

                FinishTask();

                if (list != null)
                {
                    originalImage.Image = list.ElementAt(1);
                    processingImage.Image = list.ElementAt(0);
                }

                GenerateRaportCompleted();
            });
        }

        /// <summary>
        /// When 2 hough process is complete
        /// </summary>
        private void GenerateRaportCompleted()
        {
            if (this.isNoisePictureHoughCompleted && this.isCleanPictureHoughCompleted && statisticData != null)
            {
                this.raportProgressPanel.Invoke(new MethodInvoker(() => { this.raportProgressPanel.Visible = false; }));
                
                this.tpLabel.Invoke(new MethodInvoker( () => { this.tpLabel.Text = statisticData.GetTP.ToString();}));
                this.fpLabel.Invoke(new MethodInvoker(() => { this.fpLabel.Text = statisticData.GetFP.ToString(); }));
                this.fnLabel.Invoke(new MethodInvoker(() => { this.fnLabel.Text = statisticData.GetFN.ToString(); }));
                this.tnLabel.Invoke(new MethodInvoker(() => { this.tnLabel.Text = statisticData.GetTN.ToString(); }));

                var sensitivity = statisticData.GetTP.ToDouble() / (statisticData.GetTP + statisticData.GetFN).ToDouble();
                var specyficity = statisticData.GetTN.ToDouble() / (statisticData.GetTN + statisticData.GetFP).ToDouble();

                this.sensitivityLabel.Invoke(new MethodInvoker(() => { this.sensitivityLabel.Text = sensitivity.ToString(); }));
                this.specyficityLabel.Invoke(new MethodInvoker(() => { this.specyficityLabel.Text = specyficity.ToString(); }));

                this.isNoisePictureHoughCompleted = this.isCleanPictureHoughCompleted = false;
            }
        }

        /// <summary>
        /// Start progress bar
        /// </summary>
        private void StartTask()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new MethodInvoker(() => { progressBar.Visible = true; }));
            }
            else
            {
                progressBar.Visible = true;
            }

            if (numericUpDown2.InvokeRequired)
            {
                numericUpDown2.Invoke(new MethodInvoker(() => { numericUpDown2.Enabled = false; }));
            }
            else
            {
                numericUpDown2.Enabled = false;
            }
        }

        /// <summary>
        /// Finish progress bar
        /// </summary>
        private void FinishTask()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new MethodInvoker(() => { progressBar.Visible = false; }));
            }
            else
            {
                progressBar.Visible = false;
            }

            if (numericUpDown2.InvokeRequired)
            {
                numericUpDown2.Invoke(new MethodInvoker(() => { numericUpDown2.Enabled = true; }));
            }
            else
            {
                numericUpDown2.Enabled = true;
            }
        }

        /// <summary>
        /// About author click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Wykonawca: Mateusz Mazurek\nEmail: mateusz.mazurek@gmail.com", "O autorze");
        }

        /// <summary>
        /// Open propertis window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ustawieniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                this.propertisPanel.BringToFront();
                this.propertisPanel.Visible = true;
            });
        }

        /// <summary>
        /// Close propertis window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closePropertisBut_Click(object sender, EventArgs e)
        {
            this.propertisPanel.Visible = false;
        }

        /// <summary>
        /// Event when value changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void amountLinesSet_ValueChanged(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                var obj = sender as NumericUpDown;

                if (obj != null)
                {
                    switch (obj.Tag.ToString())
                    {
                        case "amountLines":
                            this.lineDetect = Convert.ToInt32(obj.Value);
                            break;
                        case "distribution":
                            this.distribution = Convert.ToInt32(obj.Value);
                            break;
                        case "deviation":
                            this.deviation = Convert.ToInt32(obj.Value);
                            break;
                        case "probability":
                            this.probability = obj.Value;
                            break;
                        default:
                            MessageBox.Show("Wystąpił błąd", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Start Make Noise Gauss
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void gaussaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await HandleAction(() =>
            {
                var gaussNoise = new GaussNoise(this.originalImage.Image);

                StartTask();
                gaussNoise.Completed += gaussNoise_Completed;

                this.isNoise = true;

                return Task.Run(() => gaussNoise.MakeNoise(distribution, deviation));

                //return Task.Run(() =>
                //    {
                //       var img = ClassRandom.GaussNoiseImage(this.originalImage.Image);
                //       this.processingImage.Image = img;
                //    }
                //    );
            });
        }

        /// <summary>
        /// Completed make noise Gauss
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gaussNoise_Completed(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                SendImage(sender);
            });
        }

        /// <summary>
        /// Make noise random Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void losowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await HandleAction(() =>
            {
                var randomNoise = new RandomNoise(this.originalImage.Image, probability);

                StartTask();
                randomNoise.Completed += randomNoise_Completed;

                this.isNoise = true;

                return Task.Run(() => randomNoise.MakeNoise());
            });
        }

        /// <summary>
        /// Make Noise random Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void randomNoise_Completed(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                SendImage(sender);
            });
        }

        /// <summary>
        /// Handle Completed event
        /// </summary>
        /// <param name="sender"></param>
        private void SendImage(object sender)
        {
            var obj = sender as Bitmap;

            if (obj != null)
            {
                FinishTask();

                this.processingImage.Image = obj;
            }
        }


        private void onlyLine_CheckedChanged(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                var obj = sender as CheckBox;

                if (obj != null)
                {
                    this.allLine = obj.Checked;
                }
            });
        }

        /// <summary>
        /// Handle event, catch exceptions
        /// </summary>
        /// <param name="action"></param>
        private void HandleAction(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task HandleAction(Func<Task> func)
        {
            Task t = null;

            try
            {
                t = func();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return t;
        }

        /// <summary>
        /// Generate raport Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(() =>
            {
                if (this.sobelPictureOriginal == null)
                {
                    MessageBox.Show("Musisz przeprowadzić operację sobla na obrazie.");
                }
                else
                {
                    this.percentLabel.Text = isNoise ? probability.ToString(): "0";
                    this.lineLabel.Text = lineDetect.ToString();

                    this.raportPanel.BringToFront();
                    this.raportPanel.Visible = true;
                }
            });
        }

        private void closeRaport_Click(object sender, EventArgs e)
        {
            HandleAction(() => 
            {
                this.raportPanel.Visible = false;
            });
        }
    }
}
