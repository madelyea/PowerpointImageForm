using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PowerpointImageForm
{
    //This form displays the generated images and saves the users checked choices.
    public class DisplayPictures : Form
    {
        public List<Picture> pictureList;

        public Form1 Form { get; set; }

        public DisplayPictures(Form1 form)
        {
            pictureList = new List<Picture>();
            Form = form;
        }

        DisplayPictures(List<Picture> pictures, Form1 form)
        {
            pictureList = pictures;
            Form = form;
        }

        public void CheckedPictures(object sender, EventArgs e)
        {
            List<Picture> chosenPictures = new List<Picture>();
            foreach(Picture picture in pictureList)
            {
                if (picture.pictureCheckBox.Checked)
                {
                    chosenPictures.Add(picture);
                }
            }
            Form.ChooseImages(chosenPictures);

        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DisplayPictures
            // 
            this.ClientSize = new System.Drawing.Size(764, 409);
            this.Name = "DisplayPictures";
            this.ResumeLayout(false);

        }
    }
}