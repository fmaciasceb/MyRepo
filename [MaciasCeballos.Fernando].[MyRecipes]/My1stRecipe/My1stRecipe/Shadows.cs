using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My1stRecipe
{
    class Shadows
    {
       
        // Function that applys a shadow to a panel
        public void ApplyShadows(Panel OB, Panel  Contenedor)
        {
            PictureBox T = new PictureBox();
            PictureBox L = new PictureBox();
            PictureBox R = new PictureBox();
            PictureBox B = new PictureBox();
            PictureBox TL = new PictureBox();
            PictureBox TR = new PictureBox();
            PictureBox BL = new PictureBox();
            PictureBox BR = new PictureBox();

            T.Tag = "sombra";
            L.Tag = "sombra";
            R.Tag = "sombra";
            B.Tag = "sombra";
            TL.Tag = "sombra";
            TR.Tag = "sombra";
            BL.Tag = "sombra";
            BR.Tag = "sombra";

            T.BackgroundImageLayout = ImageLayout.Stretch;
            L.BackgroundImageLayout = ImageLayout.Stretch;
            R.BackgroundImageLayout = ImageLayout.Stretch;
            B.BackgroundImageLayout = ImageLayout.Stretch;

            TR.SizeMode = PictureBoxSizeMode.AutoSize;
            TL.SizeMode = PictureBoxSizeMode.AutoSize;
            BR.SizeMode = PictureBoxSizeMode.AutoSize;
            BL.SizeMode = PictureBoxSizeMode.AutoSize;

            T.BackgroundImage = Properties.Resources.T_WHITE;
            L.BackgroundImage = Properties.Resources.L_WHITE;
            R.BackgroundImage = Properties.Resources.R_WHITE; 
            B.BackgroundImage = Properties.Resources.B_WHITE;
            TL.Image = Properties.Resources.TL_WHITE; 
            TR.Image = Properties.Resources.TR_WHITE; 
            BL.Image = Properties.Resources.BL_WHITE; 
            BR.Image = Properties.Resources.BR_WHITE; 

            T.Width = 6;
            T.Height = 6;
            B.Width = 6;
            B.Height = 6;
            L.Width = 6;
            L.Height = 6;
            R.Width = 6;
            R.Height = 6;


          
            try
            {
                Contenedor.Controls.Add(T);
                Contenedor.Controls.Add(L);
                Contenedor.Controls.Add(R);
                Contenedor.Controls.Add(B);
                Contenedor.Controls.Add(TR);
                Contenedor.Controls.Add(TL);
                Contenedor.Controls.Add(BL);
                Contenedor.Controls.Add(BR);

            }
            catch 
            {

            }



           

            T.Top = OB.Top - T.Height;
            T.Left = OB.Left;
            T.Width = OB.Width;


            B.Top = OB.Top + OB.Height;
            B.Left = OB.Left;
            B.Width = OB.Width;

            L.Top = OB.Top;
            L.Left = OB.Left - L.Width;
            L.Height = OB.Height;
           
            R.Top = OB.Top;
            R.Left = OB.Left + OB.Width;
            R.Height = OB.Height;

            TL.Top = OB.Top - TL.Height;
            TL.Left = OB.Left - TL.Width;

            BL.Top = OB.Top + OB.Height;
            BL.Left = OB.Left - BL.Width;

            TR.Top = OB.Top - TR.Height;
            TR.Left = OB.Left + OB.Width;

            BR.Top = OB.Top + OB.Height;
            BR.Left = OB.Left + OB.Width;
        }



    }
}
