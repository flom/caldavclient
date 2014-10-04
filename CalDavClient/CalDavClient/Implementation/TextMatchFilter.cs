using Fallto.CalDav.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallto.CalDav.Implementation
{
  public class TextMatchFilter : IFilter
  {
    private string value;
    private string collation;
    private YesNo negateCondition;

    public TextMatchFilter(string value = "", string collation = "i;ascii-casemap", YesNo negateCondition = YesNo.no)
    {
      this.value = value;
      this.collation = collation;
      this.negateCondition = negateCondition;
    }

    public string ToXml()
    {
      return string.Format("<C:text-match collation=\"{0}\" negate-condition=\"{1}\">{2}</C:text-match>",
        collation, negateCondition.ToString(), value);
    }
  }

  public enum YesNo
  {
    yes, no
  }
}
