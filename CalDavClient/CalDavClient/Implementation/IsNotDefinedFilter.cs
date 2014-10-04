using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class IsNotDefinedFilter : IFilter
  {
    public string ToXml()
    {
      return "<C:is-not-defined />";
    }
  }
}
