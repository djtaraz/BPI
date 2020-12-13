using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using System.IO;
using System.Net.Http;
using System.Threading;


namespace ScrapePop
{
    public partial class Form1 : Form
    {

        public string[] QueryTerms { get; } = { "Billie Eilish Therefore I Am", "Ariana Grande Positions", "Shawn Mendes Monster" };

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var response = await fetchDoc(this.QueryTerms[0]);
            var results = this.scrapeDoc(response);
            this.textBox1.Text = string.Join("\n", results.ToArray());
        }

        private async Task<IHtmlDocument> fetchDoc(string query)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 9; A60Pro) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.136 Mobile Safari/537.36");
            HttpResponseMessage response = await httpClient.GetAsync($"https://google.com/search?q={query}");
            Stream stream = await response.Content.ReadAsStreamAsync();
            HtmlParser parser = new HtmlParser();
            return parser.ParseDocument(stream);
        }


        private List<string> scrapeDoc(IHtmlDocument doc)
        {
            var rawResults = doc.GetElementsByClassName("g");
            var links = rawResults.Select(z => z.GetElementsByTagName("a").First().GetAttribute("href"));
           
            Console.WriteLine(links.Count());

            return new List<string>();
        }

        private void GetScrapeResults(IHtmlDocument document)
        {
            IEnumerable<IElement> pageLink = null;

            foreach (var term in QueryTerms)
            {
                pageLink = document.All.Where(x =>
                    x.ClassName == "LC20lb DKV0Md" &&
                    (x.ParentElement.InnerHtml.Contains(term) || x.ParentElement.InnerHtml.Contains(term.ToLower()))).Skip(1);
            }

            if (pageLink.Any())
            {
                PrintResults(pageLink);
            }
        }
        public void PrintResults(IEnumerable<IElement> pageLink)
        {
            foreach (var element in pageLink)
            {
                CleanUpResults(element);

                //resultsTextbox.Text = $"{Title} - {Url}{Environment.NewLine}";
            }
        }

        private void CleanUpResults(IElement result)
        {
            string htmlResult = result.InnerHtml.ReplaceFirst("        <span class=\"field-content\"><div><a href=\"", "https://www.google.com/");
            htmlResult = htmlResult.ReplaceFirst("\">", "*");
            htmlResult = htmlResult.ReplaceFirst("</a></div>\n<div class=\"article-title-top\">", "-");
            htmlResult = htmlResult.ReplaceFirst("</div>\n<hr></span>  ", "");

            SplitResults(htmlResult);
        }
        private void SplitResults(string htmlResult)
        {
            string[] splitResults = htmlResult.Split('*');
            // Url = splitResults[0];
            // Title = splitResults[1];
        }
    }
}
