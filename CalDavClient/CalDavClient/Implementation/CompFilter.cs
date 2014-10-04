using Fallto.CalDav.Exceptions;
using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class CompFilter : IFilter
  {
    public ResourceType Resource { get; set; }
    private List<IFilter> children;

    public CompFilter(ResourceType resource)
    {
      this.Resource = resource;
      children = new List<IFilter>();
    }

    public void AddChild(CompFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public void AddChild(PropFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public void AddChild(TimeRangeFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(TimeRangeFilter)))
        throw new InvalidFilterException("Only one TimeRangeFilter is allowed");
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public void AddChild(IsNotDefinedFilter filter)
    {
      if (children.Any()) throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public string ToXml()
    {
      string xml = string.Format("<C:comp-filter name=\"{0}\">", Resource.ToString());

      foreach (var child in children)
      {
        xml += child.ToXml();
      }

      xml += "</C:comp-filter>";
      return xml;
    }
  }

  public enum ResourceType
  {
    VCALENDAR, VEVENT, VTODO
  }
}
