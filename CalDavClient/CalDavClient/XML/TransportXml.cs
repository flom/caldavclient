using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.XML
{
  public static class TransportXml
  {
    public static string MKCALENDAR = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:mkcalendar xmlns:D=""DAV:""
              xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:set>
    <D:prop>
    </D:prop>
  </D:set>
</C:mkcalendar>
";
    public static string REPORT = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:D=""DAV:""
                 xmlns:C=""urn:ietf:params:xml:ns:caldav"">
  <D:prop>
    <D:getetag/>
    <C:calendar-data>
      <C:comp name=""VCALENDAR"">
        <C:prop name=""VERSION""/>
        <C:comp name=""VEVENT"" />
        <C:comp name=""VTIMEZONE""/>
      </C:comp>
    </C:calendar-data>
  </D:prop>
  <C:filter>
  {{FILTER}}
  </C:filter>
</C:calendar-query>
";
  }
}
