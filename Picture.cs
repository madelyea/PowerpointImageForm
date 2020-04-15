using System.Windows.Forms;

namespace PowerpointImageForm
{

    //Each Picture object is a picturebox with URL and checkbox
    public class Picture : PictureBox

    {
        public Picture()
        {
            pictureCheckBox = new CheckBox();
        }

        public string URL { get; set; }
        public CheckBox pictureCheckBox { get; set; }

    }
}