using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace MvcBase.Infrastructure
{
	public static class Utility
	{
		public static string HumanizeCamel(string camelCasedString)
		{
			if (camelCasedString == null)
				return "";

			StringBuilder sb = new StringBuilder();

			char last = char.MinValue;
			foreach (char c in camelCasedString)
			{
				if (char.IsLower(last) && char.IsUpper(c))
				{
					sb.Append(' ');
				}
				sb.Append(c);
				last = c;
			}
			return sb.ToString();
		}

		public static string GetWatermark(ViewDataDictionary<dynamic> data)
		{
			string watermark = data.ModelMetadata.Watermark;
			if (watermark == "")
				watermark = Utility.HumanizeCamel(data.TemplateInfo.HtmlFieldPrefix);
			return watermark;
		}

	}
}
