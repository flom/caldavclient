using Fallto.CalDav.Exceptions;
using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class TimeRangeFilter : IFilter
  {
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public string ToXml()
    {
      if (!(Start.HasValue || End.HasValue)) throw new InvalidFilterException("Both Start and End can not be null.");

      string formatString = "yyyyMMdd'T'HHmmss'Z'";
      if (Start.HasValue && End.HasValue)
      {
        if (End < Start) throw new InvalidFilterException("Start has to be greater than End");

        return string.Format("<C:time-range start=\"{0}\" end=\"{1}\" />",
          Start.Value.ToString(formatString), End.Value.ToString(formatString));
      }
      if (Start.HasValue)
      {
        return string.Format("<C:time-range start=\"{0}\" />",
          Start.Value.ToString(formatString));
      }
      return string.Format("<C:time-range end=\"{0}\" />",
        End.Value.ToString(formatString));
    }
  }
}
