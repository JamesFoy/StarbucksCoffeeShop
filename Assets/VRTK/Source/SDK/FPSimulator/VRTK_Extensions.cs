using System.Text;
using UnityEngine;

namespace VRTK
{
  public static class VRTK_Extensions
  {
    public static Transform FindChildRecursive(this Transform aParent, string aName)
    {
      var result = aParent.Find(aName);
      if (result != null)
        return result;
      foreach (Transform child in aParent)
      {
        result = child.FindChildRecursive(aName);
        if (result != null)
          return result;
      }
      return null;
    }

    public static StringBuilder AppendKey(this StringBuilder b, KeyCode k)
    {
      b.Append("<b>");
      b.Append(k);
      b.Append("</b>");
      return b;
    }
    public static StringBuilder AppendOptionalKey(this StringBuilder b, KeyCode k)
    {
      b.Append("(<b>");
      b.Append(k);
      b.Append("</b>)");
      return b;
    }
  }
}