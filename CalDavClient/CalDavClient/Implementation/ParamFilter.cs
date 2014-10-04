using Fallto.CalDav.Exceptions;
using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class ParamFilter : IFilter
  {
    private string name;
    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value.ToUpper();
      }
    }
    private IFilter child;

    public ParamFilter(string name)
    {
      this.Name = name;
    }

    public void AddChild(IsNotDefinedFilter filter)
    {
      if (child != null)
        throw new InvalidFilterException("only one child allowed");

      child = filter;
    }

    public void AddChild(TextMatchFilter filter)
    {
      if (child != null)
        throw new InvalidFilterException("only one child allowed");

      child = filter;
    }

    public string ToXml()
    {
      string childXml = child == null ? "" : child.ToXml();
      return string.Format("<C:param-filter name=\"{0}\">{1}</C:param-filter>", Name, childXml);
    }
  }
}
