using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
//using RTE.Convertor.PDF;
//using iTextSharp.text;
//using iTextSharp.text.html.simpleparser;
//using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace Econocom.Helper.File
{
    public class PdfHelper
    {
        //public static void HTMLToPdf(string html, string FilePath, HttpResponseBase response)
        //{
        //    //try
        //    //{
        //    //    var document = new Document();
        //    //    var writer = PdfWriter.GetInstance(document,new FileStream("results/walden1.pdf", FileMode.Create, FileAccess.Write));
        //    //    document.Open();
        //    //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, null, null, Encoding.UTF8);//(writer, document, html);
        //    //    document.close();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    throw;
        //    //}
        //}

        //private static void ShowPdf(HttpResponseBase Response, string s)
        //{
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.AddHeader("Content-Disposition", "inline;filename=" + s);
        //    Response.ContentType = "application/pdf";
        //    Response.WriteFile(s);
        //    Response.Flush();
        //    Response.Clear();
        //}

        //public static void GeneratePdf(string html)
        //{
        //    ////reset response
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.ContentType = "application/pdf";

        //    ////define pdf filename
        //    string strFileName = "newFile.pdf";
        //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + strFileName);

        //    //get html
        //    string outXml = html;


        //    //Generate PDF
        //    var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);

        //    //define output control HTML
        //    MemoryStream memStream = new MemoryStream();
        //    TextReader xmlString = new StringReader(outXml);

        //    PdfWriter writer = PdfWriter.GetInstance(document, memStream);

        //    //open doc
        //    document.Open();

        //    HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
        //    htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
        //    //htmlContext.SetImageProvider(new ImageProvider());

        //    ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
        //    //cssResolver.AddCssFile(Server.MapPath("~/css/style.css"), true);

        //    IPipeline pipeline = new CssResolverPipeline(cssResolver,
        //                                                 new HtmlPipeline(htmlContext,
        //                                                                  new PdfWriterPipeline(document, writer)));
        //    XMLWorker worker = new XMLWorker(pipeline, true);
        //    XMLParser xmlParse = new XMLParser(true, worker);
        //    xmlParse.Parse(xmlString);
        //    xmlParse.Flush();

        //    document.Close();

        //    HttpContext.Current.Response.BinaryWrite(memStream.ToArray());


        //    HttpContext.Current.Response.End();
        //    HttpContext.Current.Response.Flush();
        //}


        //public static void GETPDF(string title, string html)
        //{
        //    try
        //    {


        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}

        //public static void genPdf(string html)
        //{

        //    Text2PDF(html);
            
        //}

        //private static void DownLoadPdf(string PDF_FileName)
        //{
        //    WebClient client = new WebClient();
        //    Byte[] buffer = client.DownloadData(PDF_FileName);            
        //    HttpContext.Current.Response.ContentType = "application/pdf";
        //    HttpContext.Current.Response.AddHeader("content-length", buffer.Length.ToString());
        //    HttpContext.Current.Response.BinaryWrite(buffer);

        //}
        //protected static void Text2PDF(string PDFText)
        //{
        //    Document document = new Document(PageSize.A4);
        //    try
        //    {
        //        HttpContext.Current.Response.Clear();
        //        HttpContext.Current.Response.ContentType = "application/pdf";

        //        ////define pdf filename
        //        string strFileName = "newFile.pdf";
        //        HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + strFileName);

        //        //get html
        //        string outXml = PDFText;

        //        StringReader reader = new StringReader(outXml);

        //    //Create PDF document 
           
        //    HTMLWorker parser = new HTMLWorker(document);

        //    MemoryStream memStream = new MemoryStream();
        //    TextReader xmlString = new StringReader(PDFText);


        //    PdfWriter.GetInstance(document, memStream);
        //    document.Open();

           
        //        parser.Parse(reader);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Display parser errors in PDF. 
        //        Paragraph paragraph = new Paragraph("Error!" + ex.Message);
        //        Chunk text = paragraph.Chunks[0] as Chunk;
        //        if (text != null)
        //        {
        //            text.Font.Color = BaseColor.RED;
        //        }
        //        document.Add(paragraph);
        //    }
        //    finally
        //    {
        //        document.Close();
        //        DownLoadPdf("PDF_FileName.pdf");
        //    }
        //}
    }
}
