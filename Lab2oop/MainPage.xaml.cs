using System;
using System.Text; 
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Lab2oop
{
    public interface IXmlProcessingStrategy
    {
        string ProcessXml(XDocument xmlDoc, string searchText);
    }

    public class SaxProcessingStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument xmlDoc, string searchText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlReader reader = xmlDoc.CreateReader())
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "author")
                    {
                        XElement authorElement = XElement.ReadFrom(reader) as XElement;
                        if (authorElement != null && authorElement.Value.ToLower().Contains(searchText.ToLower()))
                        {
                            stringBuilder.AppendLine(authorElement.ToString());
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }


    public class DomProcessingStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument xmlDoc, string searchText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            IEnumerable<XElement> authors = xmlDoc.Descendants("author");
            foreach (var author in authors)
            {
                if (author.Value.ToLower().Contains(searchText.ToLower()))
                {
                    stringBuilder.AppendLine(author.ToString());
                }
            }
            return stringBuilder.ToString();
        }
    }


    public class LinqToXmlProcessingStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument xmlDoc, string searchText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var results = xmlDoc.Descendants("author")
                                .Where(author => author.Value.ToLower().Contains(searchText.ToLower()));

            foreach (var author in results)
            {
                stringBuilder.AppendLine(author.ToString());
            }
            return stringBuilder.ToString();
        }
    }


    public partial class MainPage : ContentPage
    {
        private IXmlProcessingStrategy xmlProcessingStrategy;
    }


    public partial class MainPage : ContentPage
    {
        XDocument xmlDoc;
        public MainPage()
        {
            InitializeComponent();

            SaxRadioButton.IsChecked = true;
        }

        private void LoadXmlDocument()
        {
            string xmlFilePath = "C:\\Users\\Денис\\source\\repos\\Lab2oop\\Lab2oop\\Teachers.xml";

            xmlDoc = XDocument.Load(xmlFilePath);
        }


        private async void OnSelectXmlFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "public.xml" } },
            { DevicePlatform.Android, new[] { "text/xml" } },
            { DevicePlatform.WinUI, new[] { ".xml" } },
            { DevicePlatform.Tizen, new[] { "*/*" } },
            { DevicePlatform.macOS, new[] { "xml" } },
        });

                var options = new PickOptions
                {
                    PickerTitle = "Будь ласка, виберіть XML файл",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.PickAsync(options);

                if (result != null)
                {
                    xmlDoc = XDocument.Load(result.FullPath);

                  
var stringBuilder = new StringBuilder();
                    foreach (var author in xmlDoc.Descendants("author"))
                    {
                        stringBuilder.AppendLine(author.Element("name")?.Value);
                        stringBuilder.AppendLine(author.Element("materialTitle")?.Value);
                        stringBuilder.AppendLine(author.Element("faculty")?.Value);
                        stringBuilder.AppendLine(author.Element("department")?.Value);
                        stringBuilder.AppendLine(author.Element("materialType")?.Value);
                        stringBuilder.AppendLine(author.Element("volume")?.Value);
                        stringBuilder.AppendLine(author.Element("creationDate")?.Value);
                        stringBuilder.AppendLine();
                    }

                    outputEditor.Text = stringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Помилка при відкритті файлу: {ex.Message}", "OK");
            }
        }

        private void OnSaxAnalyzeClicked(object sender, EventArgs e)
        {
            xmlProcessingStrategy = new SaxProcessingStrategy();
            var result = xmlProcessingStrategy.ProcessXml(xmlDoc, SearchEntry.Text?.ToLower());
            outputEditor.Text = result;
        }

        private void OnDomAnalyzeClicked(object sender, EventArgs e)
        {
            xmlProcessingStrategy = new DomProcessingStrategy();
            var result = xmlProcessingStrategy.ProcessXml(xmlDoc, SearchEntry.Text?.ToLower());
            outputEditor.Text = result;
        }

        private void OnLinqToXmlAnalyzeClicked(object sender, EventArgs e)
        {
            xmlProcessingStrategy = new LinqToXmlProcessingStrategy();
            var result = xmlProcessingStrategy.ProcessXml(xmlDoc, SearchEntry.Text?.ToLower());
            outputEditor.Text = result;
        }



        private void OnTransformToHtmlClicked(object sender, EventArgs e)
        {

            if (xmlDoc == null)
            {
                DisplayAlert("Помилка", "Будь ласка, спочатку виберіть XML файл.", "OK");
                return;
            }
            try
            {
                string xmlFilePath = "C:\\Users\\Денис\\source\\repos\\Lab2oop\\Lab2oop\\Teachers.xml";

                XDocument xmlDoc = XDocument.Load(xmlFilePath);

                string htmlFilePath = "C:\\Users\\Денис\\source\\repos\\Lab2oop\\Lab2oop\\Teachers.html";

             
XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
     <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
         <xsl:output method=""html"" encoding=""utf-8"" indent=""yes""/>
         <xsl:template match=""/"">
             <html>
                 <head>
                     <title>Teachers</title>
                 </head>
                 <body>
                     <table border=""1"">
                         <tr>
                             <th>Name</th>
                             <th>Material Title</th>
                             <th>Faculty</th>
                             <th>Department</th>
                             <th>Material Type</th>
                             <th>Volume</th>
                             <th>Creation Date</th>
                         </tr>
                         <xsl:for-each select=""academicMaterials/author"">
                             <tr>
                                 <td><xsl:value-of select=""name""/></td>
                                 <td><xsl:value-of select=""materialTitle""/></td>
                                 <td><xsl:value-of select=""faculty""/></td>
                                 <td><xsl:value-of select=""department""/></td>
                                 <td><xsl:value-of select=""materialType""/></td>
                                 <td><xsl:value-of select=""volume""/></td>
                                 <td><xsl:value-of select=""creationDate""/></td>
                             </tr>
                         </xsl:for-each>
                     </table>
                 </body>
             </html>
         </xsl:template>
     </xsl:stylesheet>"
                )));

                using (FileStream htmlStream = new FileStream(htmlFilePath, FileMode.Create))
                {
                    transform.Transform(xmlDoc.CreateReader(), null, htmlStream);
                }
                outputEditor.Text = $"HTML-файл з результатом трансформації збережено за шляхом: {htmlFilePath}";
            }
            catch (Exception ex)
            {
                outputEditor.Text = $"Помилка при трансформації в HTML: {ex.Message}";
            }
        }



        private void OnClearClicked(object sender, EventArgs e)
        {
            outputEditor.Text = string.Empty;
            SearchEntry.Text = string.Empty;

            AuthorRadioButton.IsChecked = false;
            FacultyRadioButton.IsChecked = false;
            DepartmentRadioButton.IsChecked = false;
            YearRadioButton.IsChecked = false;
            DomRadioButton.IsChecked = false;
            LinqRadioButton.IsChecked = false;
            SaxRadioButton.IsChecked = true;
        }
        private void OnSearchClicked(object sender, EventArgs e)
        {
            if (xmlDoc == null)
            {
                DisplayAlert("Помилка", "Будь ласка, спочатку виберіть XML файл.", "OK");
                return;
            }
            var searchText = SearchEntry.Text?.ToLower();
            var stringBuilder = new StringBuilder();
            IEnumerable<XElement> searchResults = Enumerable.Empty<XElement>();

         
if (!string.IsNullOrWhiteSpace(searchText))
            {
                if (AuthorRadioButton.IsChecked)
                {
                    searchResults = xmlDoc.Descendants("author")
                        .Where(author => author.Element("name").Value.ToLower().Contains(searchText));
                }
                else if (FacultyRadioButton.IsChecked)
                {
                    searchResults = xmlDoc.Descendants("author")
                        .Where(author => author.Element("faculty").Value.ToLower().Contains(searchText));
                }
                else if (DepartmentRadioButton.IsChecked)
                {
                    searchResults = xmlDoc.Descendants("author")
                        .Where(author => author.Element("department").Value.ToLower().Contains(searchText));
                }
                else if (YearRadioButton.IsChecked)
                {
                    searchResults = xmlDoc.Descendants("author")
                        .Where(author => author.Element("creationDate").Value.ToLower().Contains(searchText));
                }
                foreach (var author in searchResults)
                {
                    stringBuilder.AppendLine(author.Element("name").Value);
                    stringBuilder.AppendLine(author.Element("materialTitle").Value);
                    stringBuilder.AppendLine(author.Element("faculty").Value);
                    stringBuilder.AppendLine(author.Element("department").Value);
                    stringBuilder.AppendLine(author.Element("materialType").Value);
                    stringBuilder.AppendLine(author.Element("volume").Value);
                    stringBuilder.AppendLine(author.Element("creationDate").Value);
                    stringBuilder.AppendLine();
                }
                outputEditor.Text = stringBuilder.ToString();
            }
            else
            {
                outputEditor.Text = "Будь ласка, введіть текст для пошуку.";
            }
        }

        private async void OnLeaveClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (answer)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}