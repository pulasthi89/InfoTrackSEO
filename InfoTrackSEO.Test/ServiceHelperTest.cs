using NUnit.Framework;
using InfoTrackSEO.Domain.Helpers;
using System.Collections.Generic;

namespace InfoTrackSEO.Test
{
    public class Tests
    {
        string sampleHtmlDoc1;
        string sampleHtmlDoc1_Header;
        string sampleHtmlDoc1_Body;
        string url1 = "http://www.mygifs.com/CoverImage.gif";
        string url2 = "https://www.dummies.com/";

        string[] paragraphs;

        [SetUp]
        public void Setup()
        {
            //adding the test data
            InitTestData();
        }

        private void InitTestData()
        {
            sampleHtmlDoc1_Header = "<head><title>Enter a title, displayed at the top of the window.</title></head>";

            paragraphs = new string[6];
            paragraphs[0] = "<p>Be <b>bold</b> in stating your key points. Put them in a list: </p>";
            paragraphs[1] = "<p>Improve your image by including an image. </p>";
            paragraphs[2] = "<p><img src=\"" + url1 + "\" alt=\"A Great HTML Resource\"></p>";
            paragraphs[3] = "<p>Add a link to your favorite <a href=\"" + url2 + "\">Web site</a>. Break up your page with a horizontal rule or two. </p>";
            paragraphs[4] = "<p>Finally, link to <a href=\"page2.html\">another page</a> in your own Web site.</p>";
            paragraphs[5] = "<p>&#169; Wiley Publishing, 2011</p>";
            sampleHtmlDoc1_Body = "<body><h1>Enter the main heading, usually the same as the title.</h1>" + paragraphs[0] + "<ul><li>The first item in your list</li><li>The second item; <i>italicize</i> key words</li></ul>" + paragraphs[1] + paragraphs[2] + paragraphs[3] + "<hr>" + paragraphs[4] + " <!-- And add a copyright notice.--> " + paragraphs[5] + "</body>";
            sampleHtmlDoc1 = "<html>" + sampleHtmlDoc1_Header + "<!-- The information between the BODY and /BODY tags is displayed.-->" + sampleHtmlDoc1_Body + "</html>";

        }


        [Test]
        public void SectionExtractionTest_Body_FinishTagIncluded()
        {
            //Given a HTML document,
            //When the extraction is performed from the body start tag to the body finish tag included,
            //Then the result should be exact to the body section
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "</body>", 0, true);
            Assert.IsNotNull(bodySection);
            Assert.AreEqual(sampleHtmlDoc1_Body, bodySection.SectionBody);
        }

        [Test]
        public void SectionExtractionTest_Body_FinishTagExcluded()
        {
            //Given a HTML document,
            //When the extraction is performed from the body start tag to the html finish tag exluded,
            //Then the extracted section should be same as the body section 
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "</html>", 0, false);
            Assert.IsNotNull(bodySection);
            Assert.AreEqual(sampleHtmlDoc1_Body, bodySection.SectionBody);
        }

        [Test]
        public void SectionExtractionTest_Body_SubSectionExtraction()
        {
            //Given a HTML document,
            //When the extraction is performed from the body start tag to the body finish tag included and the paragraphs within the body are then extracted,
            //Then we should be able to extract 6 paragraphs from the body
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "</body>", 0, true);
            Assert.IsNotNull(bodySection);
            Assert.AreEqual(sampleHtmlDoc1_Body, bodySection.SectionBody);
            List<string> extractedPara = new List<string>();
            int scanIdx = 0;
            while (true)
            {
                var paraSection = ServiceHelper.ExtractSection(bodySection.SectionBody, "<p>", "</p>", scanIdx, true);
                if (paraSection == null)
                {
                    break;
                }
                extractedPara.Add(paraSection.SectionBody);
                scanIdx = paraSection.FinishingIdx;
            }

            Assert.AreEqual(extractedPara.Count, 6);
            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(paragraphs[i], extractedPara[i]);
            }

        }

        [Test]
        public void SectionExtractionTest_FailedExtraction()
        {
            //Given a HTML document,
            //When the extraction is performed from the body start tag to the body start tag
            //Then the extraction should fail and return nothing
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "<body>", 0, true);
            Assert.IsNull(bodySection);
        }

        [Test]
        public void UrlExtractionTest()
        {
            //Given a HTML section which contains URLs,
            //When the urls extraction is performed
            //Then all urls in the section should be extracted 
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "</body>", 0, true);
            Assert.IsNotNull(bodySection);
            var urlSections = ServiceHelper.ExtractUrlFromSection(bodySection, "href");
            Assert.IsNotNull(urlSections);
            Assert.AreEqual(urlSections.Count, 2);
        }

        [Test]
        public void UrlOccurrencesTest_WithinSetPatterns()
        {
            //Given a HTML section which contains multiple URLs within sub sections that follow a certain pattern,
            //When the GetUrlOccurrences method is invoked
            //Then all urls in the section should be extracted and reported as occurrences
            var bodySection = ServiceHelper.ExtractSection(sampleHtmlDoc1, "<body>", "</body>", 0, true);
            Assert.IsNotNull(bodySection);
            int sectionTotal;
            var urlOccurrences = ServiceHelper.GetUrlOccurrences(bodySection, "<p>", "</p>", "https://www.dummies.com/", 1, 0, out sectionTotal, "href");
            Assert.IsNotNull(urlOccurrences);
            Assert.AreEqual(urlOccurrences.Count, 1);
        }


    }
}