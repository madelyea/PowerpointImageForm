﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Google.Apis.Customsearch.v1;
using System.Web.Script.Serialization;
using System.Collections;
using Newtonsoft.Json;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace PowerpointImageForm
{
    public partial class Form1 : Form
    {
        //A list of imageLinks used to generate PictureBoxes
        public List<string> imageLinks = new List<string>();
        public List<Picture> pictureList = new List<Picture>();
        public List<Picture> chosenPictures = new List<Picture>();

        public string boldWords = "";
        public string slideTitle = "";
        public string slideText = "";

        public Form1()
        {
            InitializeComponent();
        }

        //This button generates image urls using Google's custom search API
        //from the inputted title and bold words in the text box.
        private void button1_Click(object sender, EventArgs e)
        {
            slideTitle = textBox1.Text;
            slideText = richTextBox1.Text;

            if (slideTitle == "")
            {
                MessageBox.Show("Please enter a title to generate images.");
                return;
            }

            //Include Title and bold words
            string words = textBox1.Text + boldWords;
            string query = words.Replace(" ", "+");

            string apiKey = "AIzaSyCgBWimb_nboKD9quJzvs_Oa008dc8VWnc";
            string cx = "015675127486788194367:dg6ulavfzom";

               //Read in JSON object from Google Api
               WebClient webClient = new WebClient();
               webClient.Headers.Add("user-agent", "Only a test!");
               var results = webClient.DownloadString(String.Format("https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&searchtype=image&alt=json", apiKey, cx, query));
               // web.Dispose();
              // Debug.WriteLine(results);

               //Deserialize JSON object
               var obj = JsonConvert.DeserializeObject<JObject>(results);
            //   Debug.WriteLine(obj["items"]);
               
               //Get the image links (Thumbnails are used currently, the actual images use cse_image instead
               foreach (JObject item in obj["items"])
                {
                    Debug.WriteLine("Title: {0}", item["title"]);
                
                    if (item["pagemap"] != null)
                    {
                        var newitem = item["pagemap"];

                        if(newitem["cse_thumbnail"] != null)
                        {   
                            var finalItem = newitem["cse_thumbnail"];
                          //  Debug.WriteLine("imagelink: " + finalItem[0]["src"]);
                            string imgLink = finalItem[0]["src"].ToString();
                          //  Debug.WriteLine("imglink: " + imgLink);

                            //Add image link to list
                            imageLinks.Add(imgLink);
                        
                        }
                    }
                }

            //Create list of picturebox objects
            pictureList = CreatePictures(imageLinks);

            //Display pictures
            DisplayPictures dp = new DisplayPictures(this);
            dp.pictureList = pictureList;
            dp.Show();
            dp.Location = new System.Drawing.Point(10, 10);

            int x = 20;
            int y = 20;

            //Generate image selections in new form with checkboxes
            foreach (Picture picture in pictureList)
            {
                picture.Location = new System.Drawing.Point(x, y);
                picture.ImageLocation = picture.URL;
                picture.Size = new Size(150, 150);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.pictureCheckBox.Location = new System.Drawing.Point(x + 75, y + 150);
                dp.Controls.Add(picture);
                dp.Controls.Add(picture.pictureCheckBox);

                x += 160;
                if (x >= 700){
                    y += 180;
                    x = 20;
                }
            }

            
        }

        //Takes in a list of imageurls and returns a list of pictureboxes
        private List<Picture> CreatePictures(List<string> imageLinks)
        {

            foreach (string item in imageLinks) {

                Picture picture = new Picture();
                picture.URL = item;
                pictureList.Add(picture);
            }

            Debug.WriteLine(pictureList);
            return pictureList;
        }

        public void ChooseImages(List<Picture> pictures)
        {
            chosenPictures = pictures;
        }

        //This button allows the user to format the text entered.
        //Bold text will be included in the image search parameters.
        private void button2_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionFont = fd.Font;   //SelectionFont allows the selected text to be formatted.
                if (richTextBox1.SelectionFont.Bold)
                {
                    boldWords += " " + richTextBox1.SelectedText;
                }
            }
        }

        //This button generates the Powerpoint slide from the inputted text and images.
        private void button3_Click(object sender, EventArgs e)
        {
                //Modified from https://stackoverflow.com/questions/26372020/how-to-programmatically-create-a-powerpoint-from-a-list-of-images

                string pictureFileName = "C:\\temp\\test.jpg";

                var pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();

                Microsoft.Office.Interop.PowerPoint.Slides slides;
                Microsoft.Office.Interop.PowerPoint._Slide slide;
                Microsoft.Office.Interop.PowerPoint.TextRange objText;

                // Create the Presentation File
                Presentation pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);

                Microsoft.Office.Interop.PowerPoint.CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText];

                // Create new Slide
                slides = pptPresentation.Slides;
                slide = slides.AddSlide(1, customLayout);

                // Add title
                objText = slide.Shapes[1].TextFrame.TextRange;
                objText.Text = slideTitle;
                objText.Font.Name = "Arial";
                objText.Font.Size = 32;

                objText = slide.Shapes[2].TextFrame.TextRange;
                objText.Text = slideText;

                if (chosenPictures != null)
                {
                    foreach (Picture picture in chosenPictures)
                    {
                        Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[2];
                        slide.Shapes.AddPicture(pictureFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);
                    }
                }

                pptPresentation.SaveAs(@"c:\temp\test.pptx", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
                //pptPresentation.Close();
                //pptApplication.Quit();
            }

        //This button clears the current search parameters and saved images to start over.
        private void button4_Click(object sender, EventArgs e)
        {
            boldWords = "";
            slideText = "";
            slideTitle = "";

            textBox1.Clear();
            richTextBox1.Clear();

            imageLinks.Clear();
            pictureList.Clear();
            chosenPictures.Clear();
        }
    }
}
