using DDay.iCal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Interfaces
{
  public interface ICalDavClient
  {
    void MakeCalendar(string user, string calendar);
    void CreateResource(string user, string calendar, IUniqueComponent resource);
    void UpdateResource(string user, string calendar, IUniqueComponent resource);
    void DeleteResource(string user, string calendar, IUniqueComponent resource);
    IICalendar GetResources(string user, string calendar, params IFilter[] filters);
  }
}
