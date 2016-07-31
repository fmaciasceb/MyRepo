using System;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Net;
using System.Data;


//*******************************************************************************************************
//******** My Recipe Book *******************************************************************************
//******** A simple library that receives an URL containing RSS feeds and returns the parsed information
//******** Author: Fernando Macías Ceballos *************************************************************
//******** Date: 2016-07-28 *****************************************************************************
//*******************************************************************************************************

namespace RecipeBook
{
    public class RecipeBook
    {
        private const int TimeOut = 20000; // limit 20s of loading

        // Structure of each node of the XML
        public class Item
        {
            public string ID;
            public string Link;
            public string Tittle;
            public string Summary;
            public DateTimeOffset LastUpdate;
            public DateTimeOffset PublishDate;
            public bool Unread;



            //Overload the equal operator to compare nodes

            public override bool Equals(object o)
            {
                if (o == null || o.GetType() != typeof(Item))
                    return false;
                Item ca = (Item)o;
                return ca.ID.Equals(this.ID);
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }

            public Item(string id, Collection<SyndicationLink> lks, string tittle, string sum, DateTimeOffset dateUpdate, DateTimeOffset pubdate, bool unread)
            {
                this.ID = id;
                if (lks.Count > 0)
                {
                    this.Link = lks[0].Uri.AbsoluteUri.ToString();
                }
                this.Tittle = tittle;
                this.Summary = sum;
                this.LastUpdate = dateUpdate;
                this.PublishDate = pubdate;
                this.Unread = unread;
            }


        }

        //Comparer to sort items by publish date
        public class ItemComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Item)x).PublishDate.CompareTo(((Item)y).PublishDate);
            }


        }

        // Public function that returns nodes in an ArrayList
        

        /// <summary>
        /// Function that receives a rss url and returns an ArrayList of Items 
        /// </summary>
        /// <param name="Url">Url of RSS Feeds</param>
        /// <param name="ShowErrors">Optional parameter.Display or not errors</param>
        /// <remarks></remarks>
        /// 
        public ArrayList GiveMeArrayList(string url, bool ShowErrors = false)
        {
            ArrayList Array = null;
            string errors = "";

            if (url.Trim() == "")
                return null;
            else
            {
                try
                {


                    WebRequest request = WebRequest.Create(url);
                    request.Timeout = TimeOut;

                    using (WebResponse response = request.GetResponse())
                    using (XmlReader lct = XmlReader.Create(response.GetResponseStream()))
                    {
                        SyndicationFeed feed = SyndicationFeed.Load(lct);
                        lct.Close();
                        Array = new ArrayList();
                        foreach (SyndicationItem item in feed.Items)
                        {
                            Item it = new Item(item.Id, item.Links, item.Title.Text, item.Summary.Text, item.LastUpdatedTime, item.PublishDate, true);
                            Array.Add(it);
                        }
                    }
                    return Array;

                }
                catch
                {
                    errors += "Not valid URL or timeout exceeded";
                }
                if ((ShowErrors) && (errors != ""))
                    MessageBox.Show(errors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return Array;

            }
        }

        /// <summary>
        /// Function that receives a rss url and returns a Datatable 
        /// </summary>
        /// <param name="Url">Url of RSS Feeds</param>
        /// <param name="ShowErrors">Optional parameter.Display or not errors</param>
        /// <remarks></remarks>
        /// 
        public DataTable GiveMeDataTable(string url, bool ShowErrors = false)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Link");
            dt.Columns.Add("Tittle");
            dt.Columns.Add("LastUpdate");
            dt.Columns.Add("PublishDate");

            string errors = "";

            if (url.Trim() == "")
                return dt;
            else
            {
                try
                {

                    WebRequest request = WebRequest.Create(url);
                    request.Timeout = TimeOut;

                    using (WebResponse response = request.GetResponse())
                    using (XmlReader lct = XmlReader.Create(response.GetResponseStream()))
                    {
                        SyndicationFeed feed = SyndicationFeed.Load(lct);
                        lct.Close();
                        foreach (SyndicationItem item in feed.Items)
                        {

                            Item it = new Item(item.Id, item.Links, item.Title.Text, item.Summary.Text, item.LastUpdatedTime, item.PublishDate, true);

                            DataRow row = dt.NewRow();
                            row["ID"] = it.ID;
                            row["Link"] = it.Link;
                            row["Tittle"] = it.Tittle;
                            row["LastUpdate"] = it.LastUpdate;
                            row["PublishDate"] = it.PublishDate;
                            dt.Rows.Add(row);

                        }
                    }
                    return dt;

                }
                catch
                {
                    errors += "Not valid URL or timeout exceeded";
                }
                if ((ShowErrors) && (errors != ""))
                    MessageBox.Show(errors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return dt;



            }


        }


    }


}
