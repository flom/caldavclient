using DDay.iCal;
using DDay.iCal.Serialization;
using DDay.iCal.Serialization.iCalendar;
using Fallto.CalDav.Interfaces;
using Fallto.CalDav.Util;
using Fallto.CalDav.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fallto.CalDav.Implementation
{
  public class CalDavClient : ICalDavClient
  {
    public Uri Url { get; set; }
    public ICredentials Credentials { get; set; }

    public CalDavClient(string url, ICredentials credentials = null)
    {
      this.Url = new Uri(url);
      this.Credentials = credentials;
    }

    public void MakeCalendar(string user, string calendar)
    {
      var request = WebRequest.Create(GetRequestUri(user, calendar));
      request.Credentials = Credentials;

      request.Method = "MKCALENDAR";
      request.ContentType = "application/xml";
      request.ContentLength = 0;

      var file = TransportXml.MKCALENDAR;
      var bytes = Encoding.UTF8.GetBytes(file);

      SendRequest(request, bytes);
    }


    public void CreateResource(string user, string calendar, IUniqueComponent resource)
    {
      var request = WebRequest.Create(new Uri(GetRequestUri(user, calendar), resource.UID + ".ics"));
      request.Credentials = Credentials;

      request.Method = "PUT";
      request.ContentType = "text/calendar";
      request.Headers.Add("If-None-Match: *");

      var bytes = WrapResource(resource);

      SendRequest(request, bytes);
    }

    public void UpdateResource(string user, string calendar, IUniqueComponent resource)
    {
      var request = WebRequest.Create(new Uri(GetRequestUri(user, calendar), resource.UID + ".ics"));
      request.Credentials = Credentials;

      request.Method = "PUT";
      request.ContentType = "text/calendar";

      var bytes = WrapResource(resource);

      SendRequest(request, bytes);
    }

    public void DeleteResource(string user, string calendar, IUniqueComponent resource)
    {
      var request = WebRequest.Create(new Uri(GetRequestUri(user, calendar), resource.UID + ".ics"));
      request.Credentials = Credentials;

      request.Method = "DELETE";
      request.ContentType = "text/calendar";

      var bytes = WrapResource(resource);

      SendRequest(request, bytes);
    }

    public IICalendar GetResources(string user, string calendar, params IFilter[] filters)
    {
      var resources = new iCalendar();

      var request = WebRequest.Create(GetRequestUri(user, calendar));
      request.Credentials = Credentials;

      request.Method = "REPORT";
      request.ContentType = "application/xml";

      string filterXml = "";
      foreach (var filter in filters)
      {
        filterXml += filter.ToXml();
      }

      var file = TransportXml.REPORT;
      file = file.Replace("{{FILTER}}", filterXml);
      var bytes = Encoding.UTF8.GetBytes(file);

      HttpWebResponse response = SendRequest(request, bytes);
      byte[] buffer = new byte[response.ContentLength];
      using (Stream stream = response.GetResponseStream())
      {
        stream.Read(buffer, 0, (int)response.ContentLength);
      }
      var xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(Encoding.UTF8.GetString(buffer));
      foreach (XmlNode item in xmlDoc.GetElementsByTagName("C:calendar-data"))
      {
        IICalendar iCal = iCalendar.LoadFromStream(new StringReader(item.InnerText))[0];
        foreach (var comp in iCal.UniqueComponents)
        {
          resources.UniqueComponents.Add(comp);
        }
      }

      return resources;
    }

    private Uri GetRequestUri(string user, string calendar)
    {
      return new Uri(Url, string.Format("{0}/{1}/", user, calendar));
    }

    private HttpWebResponse SendRequest(WebRequest request, byte[] data)
    {
      request.ContentLength = data.Length;
      using (Stream stream = request.GetRequestStream())
      {
        stream.Write(data, 0, data.Length);
      }

      return (HttpWebResponse)request.GetResponse();
    }

    private byte[] WrapResource(IUniqueComponent resource)
    {
      var iCal = new iCalendar();
      iCal.UniqueComponents.Add(resource);
      ISerializationContext ctx = new SerializationContext();
      ISerializerFactory factory = new SerializerFactory();
      IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

      return Encoding.UTF8.GetBytes(serializer.SerializeToString(iCal));
    }
  }
}
