using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Reflection;

namespace My1stRecipe
{
    public partial class frmMain : Form
    {

        int posX;
        int PosY;
        int Pos;
        int EndPos;
        
        Color NormalCol = Color.FromArgb(234, 120, 25); // Orange Color
        Color HighLightedColor = Color.FromArgb(233, 168, 135); // Orange Color mouse over
        Color OverButtons = Color.FromArgb(196, 90, 90); // Red color mouse over 
        Shadows f = new Shadows(); //shadows of the cards
        int UpOffSet = 10, LeftOffSet = 10; //left and top margin
        int CardHeight = 40;
        Thread hilo;

        private const string MicrosoftUrl = "https://channel9.msdn.com/feeds/rss";

        ArrayList All, Starred, Read;

        string Where;

        Point AboutMeVisible = new Point(50, 70);


        public frmMain()
        {
            InitializeComponent();
        }

     
        #region Loading nodes
                
        private void LoadNodes()
        {
            RecipeBook.RecipeBook Book = new RecipeBook.RecipeBook();
            ArrayList ListadoNodos = Book.GiveMeArrayList(MicrosoftUrl,true);
            if (ListadoNodos == null) return;
            
            ListadoNodos.Sort(new RecipeBook.RecipeBook.ItemComparer()); //Sort by Publish date
            ListadoNodos.Reverse();                                     // Order by publish date desc
          
            this.Starred = new ArrayList();// ArrayList containing starred feeds
            this.Read = new ArrayList();  //Arraylist containing read feeds.
            this.All = ListadoNodos;      // ArrayList containing all feeds.
            

            if (ListadoNodos != null)
            {
                for (int i=0;i<ListadoNodos.Count;i++)
                {
                    RecipeBook.RecipeBook.Item itm =(RecipeBook.RecipeBook.Item)ListadoNodos[i];
                    Panel card = CreateCard(itm);
                    card.Location = new Point(LeftOffSet, UpOffSet * (i + 1) + (CardHeight * i));

                    // Control pnlNodes is not accesible by this subprocess so got to create new invoker 
                    Invoke(new MethodInvoker(delegate
                    {
                        this.pnlNodes.Controls.Add(card);

                        f.ApplyShadows(card, this.pnlNodes);
                        Application.DoEvents(); //paint the card
                    }));
             
                   
                }


            }

            // Control lblUnread is not accesible by this subprocess so got to create new invoker
            Invoke(new MethodInvoker(delegate
            {
                if (ListadoNodos == null)
                    this.lblUnread.Text = "0";
                else
                    this.lblUnread.Text = ListadoNodos.Count.ToString();
                }));


        }
        private void LoadNodes(ArrayList nodos)
        {
            this.pnlNodes.Controls.Clear();
            if (nodos != null)
            {
                for (int i = 0; i < nodos.Count; i++)
                {
                    RecipeBook.RecipeBook.Item itm = (RecipeBook.RecipeBook.Item)nodos[i];
                    Panel card = CreateCard(itm);
                    card.Location = new Point(LeftOffSet, UpOffSet * (i + 1) + (CardHeight * i));


                    Invoke(new MethodInvoker(delegate
                    {
                        this.pnlNodes.Controls.Add(card);
                        f.ApplyShadows(card, this.pnlNodes);
                        Application.DoEvents(); //paint the card
                    }));



                }




            }
        }

        #endregion

        #region Style

        private void ApplyColor(Panel pan)
        {
            Color col = this.NormalCol;
            pan.BackColor = col;
            // 
            // f.ApplyShadows(this.pnlBottom, this);
        }

        //Mouse enter inside the card
        private void card_enter(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Panel))
            {
                Panel pnl = (Panel)sender;
                pnl.BackColor = HighLightedColor;
            }
            else if (sender.GetType() == typeof(Label))
            {
                Label lab = (Label)sender;
                lab.Parent.BackColor = HighLightedColor;
            }
            else if (sender.GetType() == typeof(PictureBox))
            {
                PictureBox pct = (PictureBox)sender;
                pct.Parent.BackColor = HighLightedColor;
            }
        }
      
        //mouse leave the card
        private void card_leave(object sender, EventArgs e)
        {

            if (sender.GetType() == typeof(Panel))
            {
                Panel pnl = (Panel)sender;
                pnl.BackColor = Color.White;
            }
            else if (sender.GetType() == typeof(Label))
            {
                Label lab = (Label)sender;
                lab.Parent.BackColor = Color.White;
            }
            else if (sender.GetType() == typeof(PictureBox))
            {
                PictureBox pct = (PictureBox)sender;
                pct.Parent.BackColor = Color.White;
            }
        }
        
        //mouse enter inside a star
        private void star_enter(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(PictureBox))
            {
                PictureBox pct = (PictureBox)sender;
                pct.Image = Properties.Resources.star_16; 
            }
        }
        
        //mouse leave the star
        private void star_leave(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(PictureBox))
            {
              
                PictureBox pct = (PictureBox)sender;
                if (!(this.Starred.Contains((RecipeBook.RecipeBook.Item)pct.Tag)))
                    pct.Image = Properties.Resources.star16_white;
            }
        }

        //click on a star
        private void star_click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(PictureBox))
            {

                PictureBox pct = (PictureBox)sender;
                if (!(this.Starred.Contains((RecipeBook.RecipeBook.Item)pct.Tag)))
                {
                    pct.Image = Properties.Resources.star_16;
                    this.Starred.Add(pct.Tag);
                }
                else
                {
                    pct.Image = Properties.Resources.star16_white;
                    this.Starred.Remove(pct.Tag);
                }
            }
        }
        
        //function that receive an Item and returns a Card
        private Panel CreateCard(RecipeBook.RecipeBook.Item itm)
        {
            //Main card
            Panel pan = new Panel();
            pan.Width = this.pnlNodes.Width - 40; // Minus 20 because of the shadows and the scrollbar
            pan.Height = CardHeight;
            pan.Cursor = Cursors.Hand;
            pan.Tag = itm;
            //Image of the rss icon
            PictureBox pct = new PictureBox();
            pct.Image = Properties.Resources.rss_24;
            pct.Size = new Size(24, 24);
            pct.Location = new Point(5, 6);
            pct.MouseEnter += new EventHandler(card_enter);
            pct.MouseClick += new MouseEventHandler(OpenUrl);
            pct.MouseLeave += new EventHandler(card_leave);
            pct.Tag = itm;

            pan.Controls.Add(pct);
            
            //Star
            PictureBox fav = new PictureBox();
            fav.Size = new Size(16, 16);
            fav.Location = new Point(pan.Width - 18, pan.Height - 18); //2 more pixels for margin

            if (this.Starred.Contains(itm))
                fav.Image = Properties.Resources.star_16;
            else
                fav.Image = Properties.Resources.star16_white;

            fav.Tag = itm;
            fav.MouseEnter += new EventHandler(star_enter);
            fav.MouseLeave += new EventHandler(star_leave);
            fav.MouseClick += new MouseEventHandler(star_click);

            pan.Controls.Add(fav);
            
            //Summary of the node
            Label lbl = new Label();
            lbl.AutoSize = false;
            lbl.Size = new Size(pan.Width - 30, pan.Height - 4); // Minus 30 because of the rss icon
            lbl.Location = new Point(40, 4);
            lbl.Text = itm.Tittle;

            if (this.Read.Contains(itm))
                lbl.Font = new Font("Verdana", 9, FontStyle.Regular); //item already read
            else
                lbl.Font = new Font("Verdana", 9, FontStyle.Bold);  // item unread

            lbl.Tag = itm;
            lbl.MouseEnter += new EventHandler(card_enter);
            lbl.MouseLeave += new EventHandler(card_leave);
            lbl.MouseClick += new MouseEventHandler(OpenUrl);
            lbl.Tag = itm;

            pan.Controls.Add(lbl);
            
            pan.MouseEnter += new EventHandler(card_enter);
            pan.MouseLeave += new EventHandler(card_leave);
            pan.MouseClick += new MouseEventHandler(OpenUrl);
            return pan;
        }
        
        //mouse leave Tab ALL button
        private void pctAll_MouseLeave(object sender, EventArgs e)
        {
            if (this.Where != "ALL")
                this.pctAll.Image = Properties.Resources.all;
        }

        //mouse enter inside Tab Unread button
        private void lblUnread_MouseEnter(object sender, EventArgs e)
        {
            this.lblUnread.ForeColor = OverButtons;
        }

        //mouse leave Tab Unread button
        private void lblUnread_MouseLeave(object sender, EventArgs e)
        {
            if (this.Where != "UNREAD")
                this.lblUnread.ForeColor = Color.White;
        }

        //mouse enter inside Tab Starred button
        private void pctStar_MouseEnter(object sender, EventArgs e)
        {
            this.pctStar.Image = Properties.Resources.star;
        }

        //mouse leave Tab Starred button
        private void pctStar_MouseLeave(object sender, EventArgs e)
        {
            if (this.Where != "STARRED")
                this.pctStar.Image = Properties.Resources.white_star2;
        }

        //mouse enter inside Upgrade button
        private void lblUpgrade_MouseEnter(object sender, EventArgs e)
        {
            this.lblUpgrade.BackColor = OverButtons;
        }

        //mouse leave upgrade button
        private void lblUpgrade_MouseLeave(object sender, EventArgs e)
        {
            this.lblUpgrade.BackColor = this.pnlTop.BackColor;
        }

        //mouse enter inside goback button
        private void pctBack_MouseEnter(object sender, EventArgs e)
        {
            this.pctBack.BackColor = OverButtons;
        }

        //mouse leave goback button
        private void pctBack_MouseLeave(object sender, EventArgs e)
        {
            this.pctBack.BackColor = Color.White;
        }

        //mouse leave close button
        private void pctClose_MouseLeave(object sender, EventArgs e)
        {
            this.pctClose.BackColor = this.NormalCol;
        }

        //mouse enter inside close button
        private void pctClose_MouseEnter_1(object sender, EventArgs e)
        {
            PictureBox pct = (PictureBox)sender;
            pct.BackColor = this.HighLightedColor;
        }

        //Animation to Show or hide cards
        private void ShowHireCards(bool visible)
        {

            for (int i = 0; i < this.pnlNodes.Controls.Count; i++)
            {
                if (pnlNodes.Controls[i].GetType() == typeof(Panel))    //cards
                {
                    Panel pan = (Panel)pnlNodes.Controls[i];
                    if (pan.Name != "pnlMe")  // Skip panel About Me
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            pan.Visible = visible;
                            Thread.Sleep(30);
                            Application.DoEvents();
                        }));
                    }
                }
                else if (pnlNodes.Controls[i].GetType() == typeof(PictureBox))   //shadows
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        PictureBox pct = (PictureBox)pnlNodes.Controls[i];
                        pct.Visible = visible;
                        Application.DoEvents();
                    }));

                }
            }
        }


        #endregion

        #region Behaviour
        //Aux functions
        private void fadeIN_tick(object sender, System.EventArgs e)
        {
            this.Opacity += 0.03;
            if (Pos < EndPos)
            {
                Pos = EndPos;
            }
            else
            {
                this.Top = Pos;
                Pos = Pos - 1;
            }

            if (this.Opacity == 1 & this.Top == EndPos)
            {
                ((System.Windows.Forms.Timer)sender).Stop();
                this.TopMost = true;
            }
        }
        private void fadeOUT_tick(object sender, System.EventArgs e)
        {
            this.Opacity -= 0.03;
            if (Pos > EndPos)
            {
                Pos = EndPos;
            }
            else
            {
                this.Top = Pos;
                Pos = Pos + 1;
            }
            this.Top = Pos;
            if (this.Opacity == 0 & this.Top >= EndPos)
            {
                ((System.Windows.Forms.Timer)sender).Stop();
                this.Hide();

            }
        }

        //Show the form
        public void AnimateIn()
        {
            Pos = PosY + 30;
            EndPos = PosY;
            this.Top = Pos;
            this.Left = posX;
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1;
            tmr.Start();
            tmr.Tick += fadeIN_tick;
        }

        //Hide the form
        public void AnimateOut()
        {
            Pos = PosY;
            EndPos = PosY + 30;
            this.Top = Pos;
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1;
            tmr.Start();
            tmr.Tick += fadeOUT_tick;
        }

        //Put frmMain in the right down corner of the primary screen
        private void PutMeInTheCorner()
        {
            this.TopMost = false;
            this.Opacity = 0;
            dynamic wkX = Screen.PrimaryScreen.WorkingArea.Width - this.Width;

            dynamic wkY = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            posX = wkX;
            PosY = wkY;
            this.Left = posX;
            this.Top = PosY;

        }

        //close form
        private void pctClose_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            AnimateOut();
            this.WindowState = FormWindowState.Minimized;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void pctClose_MouseEnter(object sender, EventArgs e)
        {
            this.pctClose.Image = Properties.Resources.cross_red;
        }

        private void pctReload_MouseEnter(object sender, EventArgs e)
        {
            this.pctReload.Image = Properties.Resources.reload_red;

        }

        private void pctReload_MouseLeave(object sender, EventArgs e)
        {
            this.pctReload.Image = Properties.Resources.reload;
        }

        private void pctClose_MouseLeave_1(object sender, EventArgs e)
        {
            this.pctClose.Image = Properties.Resources.cross;
        }

        private void pctAll_MouseEnter(object sender, EventArgs e)
        {
            this.pctAll.Image = Properties.Resources.all_64;
        }

        //Activate the main form
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

            this.Show();
            this.WindowState = FormWindowState.Normal;
            AnimateIn();
        }

        //Go to tab unread
        private void lblUnread_Click(object sender, EventArgs e)
        {
            if (this.pnlMe.Visible) return;
            this.pnlNodes.Controls.Clear();
            ArrayList Aux = GetUnread();
            if (Aux != null)
            {
                this.lblUnread.Text = Aux.Count.ToString();
                LoadNodes(Aux);
            }
            this.Where = "UNREAD";
            this.lblUnread.ForeColor = OverButtons;

            this.pctAll.Image = Properties.Resources.all;
            this.pctStar.Image = Properties.Resources.white_star2;

        }

        //Go to tab all
        private void pctAll_Click(object sender, EventArgs e)
        {
            if (this.pnlMe.Visible) return;
            this.pnlNodes.Controls.Clear();
            this.lblUnread.ForeColor = Color.White;
            LoadNodes(this.All);
        }

        //Go to tab starred
        private void pctStar_Click(object sender, EventArgs e)
        {
            if (this.pnlMe.Visible) return;
            this.Where = "STARRED";
            this.lblUnread.ForeColor = Color.White;

            this.pnlNodes.Controls.Clear();
            LoadNodes(this.Starred);
            this.pctStar.Image = Properties.Resources.star;
            this.pctAll.Image = Properties.Resources.all;
            this.lblUnread.ForeColor = Color.White;
        }

        //Show about me
        private void lblUpgrade_Click(object sender, EventArgs e)
        {
            if (this.pnlMe.Visible) return;

            this.pnlMe.Visible = true;
            this.pnlMe.Location = this.AboutMeVisible;

            ThreadStart aboutme = new ThreadStart(() => ShowHireCards(false));
            Thread upgrade = new Thread(aboutme);
            upgrade.Start();

        }

        //Go to Linkedin
        private void pctLinkedIn_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://es.linkedin.com/in/fernando-mac%C3%ADas-ceballos-4905b773");
            }
            catch
            {
                MessageBox.Show("Error, could not access to linkedin :(");
            }
        }

        private void pctBack_Click(object sender, EventArgs e)
        {
            this.pnlMe.Visible = false;

            ThreadStart aboutme = new ThreadStart(() => ShowHireCards(true));
            Thread upgrade = new Thread(aboutme);
            upgrade.Start();


        }

        //show menu for open app or exit
        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon1, null);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        //show app
        private void showAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            AnimateIn();
        }

        //end app
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        
        //click on reload
        private void pctReload_Click(object sender, EventArgs e)
        {
            if (this.pnlMe.Visible) return;

            this.pnlNodes.Controls.Clear();

            if (this.hilo != null)      //Still loading? kill it
                this.hilo.Abort();

            ThreadStart main = new ThreadStart(LoadNodes);
            this.hilo = new Thread(main);
            this.hilo.Start();
            Application.DoEvents();
            this.Where = "ALL";
            this.lblUnread.ForeColor = Color.White;
            this.pctAll.Image = Properties.Resources.all_64;
            this.pctStar.Image = Properties.Resources.white_star2;
        }

        #endregion

        #region Functions
        private void OpenUrl(object sender, EventArgs e)
        {
            string lnk = "";
            RecipeBook.RecipeBook.Item itm = null;

            if (sender.GetType() == typeof(Panel))
            {
                Panel pnl = (Panel)sender;
                itm = (RecipeBook.RecipeBook.Item)pnl.Tag;
                lnk = itm.Link;
                for (int i = 0; i < pnl.Controls.Count; i++)
                    if (pnl.Controls[i].GetType() == typeof(Label))
                        pnl.Controls[i].Font = new Font("Verdana", 9, FontStyle.Regular);

            }
            else if (sender.GetType() == typeof(Label))
            {
                Label lab = (Label)sender;
                itm = (RecipeBook.RecipeBook.Item)lab.Tag;
                lnk = itm.Link;
                lab.Font = new Font("Verdana", 9, FontStyle.Regular);

            }
            else if (sender.GetType() == typeof(PictureBox))
            {
                PictureBox pct = (PictureBox)sender;
                itm = (RecipeBook.RecipeBook.Item)pct.Tag;
                lnk = itm.Link;
                Panel father = (Panel)pct.Parent;
                for (int i = 0; i < father.Controls.Count; i++)
                    if (father.Controls[i].GetType() == typeof(Label))
                        father.Controls[i].Font = new Font("Verdana", 9, FontStyle.Regular);


            }


            if ((itm != null) && (!(this.Read.Contains(itm))))
            {
                this.Read.Add(itm);
                this.lblUnread.Text = (this.All.Count - this.Read.Count).ToString();
            }

            try
            {
                System.Diagnostics.Process.Start(lnk);
            }
            catch
            {
                MessageBox.Show("Not valid url", "Inf", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }
        //Return items unread comparing All and Read ArrayList
        private ArrayList GetUnread()
        {
            if (this.All == null) return null;

            ArrayList Aux = new ArrayList();
            RecipeBook.RecipeBook.Item itm;
            for (int i = 0; i < this.All.Count; i++)
            {
                itm = (RecipeBook.RecipeBook.Item)this.All[i];
                if (!(this.Read.Contains(itm)))
                    Aux.Add(itm);

            }


            return Aux;

        }


        #endregion
        
        private void Form1_Load(object sender, EventArgs e)
        {
           
            ApplyColor(this.pnlBottom);
            ApplyColor(this.pnlTop);

            PutMeInTheCorner();
          
            AnimateIn();

            this.Where = "ALL"; //initially show all

            ThreadStart main = new ThreadStart(LoadNodes);
            this.hilo  = new Thread(main);
            this.hilo.Start();
            Application.DoEvents();
            
       
        }
                       
       
    }
}
