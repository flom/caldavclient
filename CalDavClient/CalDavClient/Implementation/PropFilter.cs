using Fallto.CalDav.Exceptions;
using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class PropFilter : IFilter
  {
    private string property;
    public string Property
    {
      get
      {
        return property;
      }
      set
      {
        property = value.ToUpper();
      }
    }
    private List<IFilter> children;

    public PropFilter(string property)
    {
      this.Property = property;
      children = new List<IFilter>();
    }

    public void AddChild(IsNotDefinedFilter filter)
    {
      if (children.Any()) 
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public void AddChild(TimeRangeFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");
      if (children.Any(e => e.GetType() == typeof(TimeRangeFilter)))
        throw new InvalidFilterException("Children already contains TimeRangeFilter");
      if (children.Any(e => e.GetType() == typeof(TextMatchFilter)))
        throw new InvalidFilterException("Children already contains TextMatchFilter");

      children.Add(filter);
    }

    public void AddChild(TextMatchFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");
      if (children.Any(e => e.GetType() == typeof(TimeRangeFilter)))
        throw new InvalidFilterException("Children already contains TimeRangeFilter");
      if (children.Any(e => e.GetType() == typeof(TextMatchFilter)))
        throw new InvalidFilterException("Children already contains TextMatchFilter");

      children.Add(filter);
    }

    public void AddChild(ParamFilter filter)
    {
      if (children.Any(e => e.GetType() == typeof(IsNotDefinedFilter)))
        throw new InvalidFilterException("IsNotDefinedFilter cannot have siblings");

      children.Add(filter);
    }

    public string ToXml()
    {
      string xml = string.Format("<C:prop-filter name=\"{0}\">", Property);

      foreach (var child in children)
      {
        xml += child.ToXml();
      }

      xml += "</C:prop-filter>";
      return xml;
    }
  }
}
