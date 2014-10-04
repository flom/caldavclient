using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fallto.CalDav.Implementation;
using Fallto.CalDav.Exceptions;
using System.Xml;

namespace CalDavClientTest
{
  [TestClass]
  public class FilterTest
  {
    [TestMethod]
    public void TestTimeRangeFilter()
    {
      var uut = new TimeRangeFilter();

      try
      {
        uut.ToXml();
        Assert.Fail();
      }
      catch (InvalidFilterException) { }

      uut.Start = new DateTime(2000, 1, 1, 20, 0, 0);

      var xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:time-range").Count);
      var element = xmlDoc.GetElementsByTagName("C:time-range")[0];
      Assert.AreEqual(uut.Start.Value.ToString("yyyyMMdd'T'HHmmss'Z'"), element.Attributes["start"].Value);
      Assert.IsNull(uut.End);

      uut.End = new DateTime(2000, 1, 1, 21, 0, 0);
      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));
      element = xmlDoc.GetElementsByTagName("C:time-range")[0];

      Assert.AreEqual(uut.Start.Value.ToString("yyyyMMdd'T'HHmmss'Z'"), element.Attributes["start"].Value);
      Assert.AreEqual(uut.End.Value.ToString("yyyyMMdd'T'HHmmss'Z'"), element.Attributes["end"].Value);

      uut.Start = DateTime.Now.AddHours(1);
      uut.End = DateTime.Now;
      try
      {
        uut.ToXml();
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
    }

    [TestMethod]
    public void TestCompFilter()
    {
      var uut = new CompFilter(ResourceType.VEVENT);
      var xmlDoc = new XmlDocument();

      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:comp-filter").Count);
      var element = xmlDoc.GetElementsByTagName("C:comp-filter")[0];
      Assert.AreEqual(ResourceType.VEVENT.ToString(), element.Attributes["name"].Value);
      Assert.AreEqual(0, element.ChildNodes.Count);

      uut.AddChild(new TimeRangeFilter() { Start = new DateTime(2000, 1, 1, 21, 0, 0) });
      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));
      element = xmlDoc.GetElementsByTagName("C:comp-filter")[0];

      Assert.AreEqual(1, element.ChildNodes.Count);

      try
      {
        uut.AddChild(new TimeRangeFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }

      uut = new CompFilter(ResourceType.VCALENDAR);
      uut.AddChild(new IsNotDefinedFilter());
      try
      {
        uut.AddChild(new CompFilter(ResourceType.VEVENT));
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
    }

    [TestMethod]
    public void TestIsNotDefinedFilter()
    {
      var uut = new IsNotDefinedFilter();
      var xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:is-not-defined").Count);
    }

    [TestMethod]
    public void TestPropFilter()
    {
      var uut = new PropFilter("foo");
      var xmlDoc = new XmlDocument();
      uut.AddChild(new IsNotDefinedFilter());

      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:prop-filter").Count);
      var element = xmlDoc.GetElementsByTagName("C:prop-filter")[0];
      Assert.AreEqual("FOO", element.Attributes["name"].Value);
      Assert.AreEqual(1, element.ChildNodes.Count);

      try
      {
        uut.AddChild(new TimeRangeFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TextMatchFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new ParamFilter("bar"));
        Assert.Fail();
      }
      catch (InvalidFilterException) { }

      uut = new PropFilter("foo");
      uut.AddChild(new TimeRangeFilter());
      try
      {
        uut.AddChild(new IsNotDefinedFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TextMatchFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TimeRangeFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      uut.AddChild(new ParamFilter("bar"));

      uut = new PropFilter("foo");
      uut.AddChild(new TextMatchFilter());
      try
      {
        uut.AddChild(new IsNotDefinedFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TextMatchFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TimeRangeFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      uut.AddChild(new ParamFilter("bar"));

      uut = new PropFilter("foo");
      uut.AddChild(new ParamFilter("bar"));
      try
      {
        uut.AddChild(new IsNotDefinedFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
    }

    [TestMethod]
    public void TestParamFilter()
    {
      var uut = new ParamFilter("bar");
      var xmlDoc = new XmlDocument();
      uut.AddChild(new IsNotDefinedFilter());

      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:param-filter").Count);
      var element = xmlDoc.GetElementsByTagName("C:param-filter")[0];
      Assert.AreEqual("BAR", element.Attributes["name"].Value);
      Assert.AreEqual(1, element.ChildNodes.Count);

      try
      {
        uut.AddChild(new IsNotDefinedFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TextMatchFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }

      uut = new ParamFilter("bar");
      uut.AddChild(new TextMatchFilter());
      try
      {
        uut.AddChild(new IsNotDefinedFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
      try
      {
        uut.AddChild(new TextMatchFilter());
        Assert.Fail();
      }
      catch (InvalidFilterException) { }
    }

    [TestMethod]
    public void TestTextMatchFilter()
    {
      var uut = new TextMatchFilter("foobar", "colfoo", YesNo.yes);
      var xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(AddNamespace(uut.ToXml()));

      Assert.AreEqual(1, xmlDoc.GetElementsByTagName("C:text-match").Count);
      var element = xmlDoc.GetElementsByTagName("C:text-match")[0];
      Assert.AreEqual("foobar", element.InnerText);
      Assert.AreEqual("colfoo", element.Attributes["collation"].Value);
      Assert.AreEqual("yes", element.Attributes["negate-condition"].Value);
    }

    private string AddNamespace(string xml)
    {
      return string.Format("<foo xmlns:C=\"{0}\">{1}</foo>", Constants.NS_XML_ELEMENTS, xml);
    }
  }
}
