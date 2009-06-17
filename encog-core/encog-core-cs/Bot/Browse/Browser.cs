﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using Encog.Util.HTTP;
using Encog.Bot.Browse.Range;
using System.Net;

namespace Encog.Bot.Browse
{
    /// <summary>
    /// The main class for web browsing. This class allows you to navigate to a
    /// specific URL. Once you navigate to one URL, you can naviage to any URL
    /// contained on the page.
    /// </summary>
    public class Browser
    {

        /// <summary>
        /// The page that is currently being browsed.
        /// </summary>
        private WebPage currentPage;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Browser));

        /// <summary>
        /// The page currently being browsed.
        /// </summary>
        public WebPage CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;
            }
        }

        /// <summary>
        /// Navigate to the specified form by performing a submit of that form.
        /// </summary>
        /// <param name="form">The form to be submitted.</param>
        public void Navigate(Form form)
        {
            Navigate(form, null);
        }

        /// <summary>
        /// Navigate based on a form. Complete and post the form.
        /// </summary>
        /// <param name="form">The form to be posted.</param>
        /// <param name="submit">The submit button on the form to simulate clicking.</param>
        public void Navigate(Form form, Input submit)
        {

            try
            {

                if (this.logger.IsInfoEnabled)
                {
                    this.logger.Info("Posting a form");
                }

                Stream istream;
                Stream ostream;
                Uri targetURL;
                WebRequest http = null;

                if (form.Method == Form.FormMethod.GET)
                {
                    ostream = new MemoryStream();
                }
                else
                {
                    http = HttpWebRequest.Create(form.Action.Url);
                    http.Timeout = 30000;
                    http.ContentType = "application/x-www-form-urlencoded";
                    http.Method = "POST";
                    ostream = http.GetRequestStream();
                }

                // add the parameters if present
                FormUtility formData = new FormUtility(ostream, null);
                foreach (DocumentRange dr in form.Elements)
                {
                    if (dr is FormElement)
                    {
                        FormElement element = (FormElement)dr;
                        if ((element == submit) || element.AutoSend)
                        {
                            String name = element.Name;
                            String value = element.Value;
                            if (name != null)
                            {
                                if (value == null)
                                {
                                    value = "";
                                }
                                formData.Add(name, value);
                            }
                        }
                    }
                }

                // now execute the command
                if (form.Method == Form.FormMethod.GET)
                {
                    String action = form.Action.Url.ToString();
                    ostream.Close();
                    action += "?";
                    action += ostream.ToString();
                    targetURL = new Uri(action);
                    http = HttpWebRequest.Create(targetURL);
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                    istream = response.GetResponseStream();
                }
                else
                {
                    targetURL = form.Action.Url;
                    ostream.Close();
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                    istream = response.GetResponseStream();
                }

                Navigate(targetURL, istream);
                istream.Close();
            }
            catch (IOException e)
            {
                throw new BrowseError(e);
            }
        }

        /// <summary>
        /// Navigate to a new page based on a link.
        /// </summary>
        /// <param name="link">The link to navigate to.</param>
        public void Navigate(Link link)
        {

            Address address = link.Target;

            if (address.Url != null)
            {
                Navigate(address.Url);
            }
            else
            {
                Navigate(address.Original);
            }

        }

        /// <summary>
        /// Navigate based on a string URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void Navigate(String url)
        {
            Navigate(new Uri(url));

        }

        /// <summary>
        /// Navigate to a page based on a URL object. This will be an HTTP GET
        /// operation.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void Navigate(Uri url)
        {
            try
            {
                if (this.logger.IsInfoEnabled)
                {
                    this.logger.Info("Navigating to page:" + url);
                }
                WebRequest http = HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                Stream istream = response.GetResponseStream();
                Navigate(url, istream);
                istream.Close();
                response.Close();
            }
            catch (IOException e)
            {
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Exception", e);
                }
                throw new BrowseError(e);
            }
        }

        /// <summary>
        /// Navigate to a page and post the specified data.
        /// </summary>
        /// <param name="url">The URL to post the data to.</param>
        /// <param name="istream">The data to post to the page.</param>
        public void Navigate(Uri url, Stream istream)
        {
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("POSTing to page:" + url);
            }
            LoadWebPage load = new LoadWebPage(url);
            this.currentPage = load.Load(istream);
        }

    }
}
