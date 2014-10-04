using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Fallto.CalDav.Implementation;
using System.Net;
using System.IO;
using DDay.iCal;
using System.Collections;
using System.Collections.Generic;

namespace Fallto.CalDav.Test
{
  [TestClass]
  public class CalDavClientTest
  {
    public string RadicaleCollectionDir { get; set; }
    private CalDavClient uut { get; set; }
    private string user;
    private string calendar;
    private string calendarFile;

    [TestInitialize]
    public void Init()
    {
      RadicaleCollectionDir = ConfigurationManager.AppSettings["radicaleCollectionDir"];
      if (!Directory.Exists(RadicaleCollectionDir)) Directory.CreateDirectory(RadicaleCollectionDir);
      uut = new CalDavClient(ConfigurationManager.AppSettings["radicaleUrl"]);

      user = "foo";
      calendar = "calendar";
      calendarFile = Path.Combine(RadicaleCollectionDir, user, calendar);

      CleanUp();
    }

    public void CleanUp()
    {
      foreach (var dir in Directory.GetDirectories(RadicaleCollectionDir))
      {
        Directory.Delete(dir, true);
      }
    }

    [TestMethod]
    public void TestMakeCalendar()
    {
      uut.MakeCalendar(user, calendar);

      uut.CreateResource(user, calendar, new Event() { Summary = "foobar" });

      Assert.IsTrue(File.Exists(calendarFile));
      IICalendar iCal = iCalendar.LoadFromFile(calendarFile)[0];

      uut.MakeCalendar(user, calendar);
    }

    [TestMethod]
    public void TestCreateResource()
    {
      uut.MakeCalendar(user, calendar);

      var theEvent = new Event();
      theEvent.Description = "bar event";
      theEvent.Start = new iCalDateTime(2000, 1, 1, 20, 0, 0);
      theEvent.End = new iCalDateTime(2000, 1, 1, 21, 0, 0);
      theEvent.Summary = "partying hard!";
      var todo = new Todo()
      {
        Description = "stuff todo",
        Summary = "bar todo"
      };

      uut.CreateResource(user, calendar, theEvent);
      uut.CreateResource(user, calendar, todo);

      IICalendar iCal = iCalendar.LoadFromFile(calendarFile)[0];

      Assert.AreEqual(1, iCal.Events.Count);
      Assert.AreEqual(theEvent.Description, iCal.Events[0].Description);
      Assert.AreEqual(theEvent.Start, iCal.Events[0].Start);
      Assert.AreEqual(theEvent.End, iCal.Events[0].End);
      Assert.AreEqual(theEvent.Summary, iCal.Events[0].Summary);

      Assert.AreEqual(1, iCal.Todos.Count);
      Assert.AreEqual(todo.Description, iCal.Todos[0].Description);

      try
      {
        uut.CreateResource(user, calendar, theEvent);
        Assert.Fail("duplicate creation");
      }
      catch (WebException)
      {
      }
    }

    [TestMethod]
    public void TestUpdateResource()
    {
      uut.MakeCalendar(user, calendar);

      var theEvent = new Event();
      theEvent.Description = "bar event";
      theEvent.Start = new iCalDateTime(2000, 1, 1, 20, 0, 0);
      theEvent.End = new iCalDateTime(2000, 1, 1, 21, 0, 0);
      theEvent.Summary = "partying hard!";

      uut.CreateResource(user, calendar, theEvent);

      theEvent.Summary = "or not";
      uut.UpdateResource(user, calendar, theEvent);

      IICalendar iCal = iCalendar.LoadFromFile(calendarFile)[0];

      Assert.AreEqual(1, iCal.Events.Count);
      Assert.AreEqual(theEvent.Description, iCal.Events[0].Description);
      Assert.AreEqual(theEvent.Start, iCal.Events[0].Start);
      Assert.AreEqual(theEvent.End, iCal.Events[0].End);
      Assert.AreEqual(theEvent.Summary, iCal.Events[0].Summary);
    }

    [TestMethod]
    public void TestDeleteResource()
    {
      uut.MakeCalendar(user, calendar);

      var theEvent = new Event();
      theEvent.Description = "bar event";
      theEvent.Start = new iCalDateTime(2000, 1, 1, 20, 0, 0);
      theEvent.End = new iCalDateTime(2000, 1, 1, 21, 0, 0);
      theEvent.Summary = "partying hard!";

      uut.CreateResource(user, calendar, theEvent);

      theEvent.Summary = "or not";
      uut.DeleteResource(user, calendar, theEvent);

      IICalendar iCal = iCalendar.LoadFromFile(calendarFile)[0];
      Assert.AreEqual(0, iCal.Events.Count);
    }

    [TestMethod]
    public void TestGetEvents()
    {
      uut.MakeCalendar(user, calendar);

      var evt1 = new Event()
      {
        Summary = "evt1",
        Start = new iCalDateTime(2000, 1, 1, 20, 0, 0),
        End = new iCalDateTime(2000, 1, 1, 21, 0, 0),
      };
      var evt2 = new Event()
      {
        Summary = "evt2",
        Start = new iCalDateTime(2000, 2, 1, 20, 0, 0),
        End = new iCalDateTime(2000, 2, 1, 21, 0, 0),
      };
      var evt3 = new Event()
      {
        Summary = "evt3",
        Location = "adh",
        Start = new iCalDateTime(2000, 3, 1, 20, 0, 0),
        End = new iCalDateTime(2000, 3, 1, 21, 0, 0),
      };
      var todo = new Todo()
      {
        Summary = "todo"
      };

      uut.CreateResource(user, calendar, evt1);
      uut.CreateResource(user, calendar, evt2);
      uut.CreateResource(user, calendar, evt3);
      uut.CreateResource(user, calendar, todo);

      IICalendar events = uut.GetResources(user, calendar);
      Assert.AreEqual(4, events.UniqueComponents.Count());

      var filter = new CompFilter(ResourceType.VCALENDAR);
      Assert.AreEqual(0, uut.GetResources(user, calendar, filter).UniqueComponents.Count());

      var eventFilter = new CompFilter(ResourceType.VEVENT);
      Assert.AreEqual(3, uut.GetResources(user, calendar, eventFilter).UniqueComponents.Count());

      var todoFilter = new CompFilter(ResourceType.VTODO);
      Assert.AreEqual(1, uut.GetResources(user, calendar, todoFilter).UniqueComponents.Count());
    }

  }
}
