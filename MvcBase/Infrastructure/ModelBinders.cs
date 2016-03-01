using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
namespace MvcBase.Infrastructure
{


	public abstract class ArrayBinder : IModelBinder
	{
		private static IEnumerable<string> GetKeys(ControllerContext context)
		{
			List<string> keys = new List<string>();
			HttpRequestBase request = context.HttpContext.Request;
			keys.AddRange(((IDictionary<string,
				object>)context.RouteData.Values).Keys.Cast<string>());
			keys.AddRange(request.QueryString.Keys.Cast<string>());
			keys.AddRange(request.Form.Keys.Cast<string>());
			return keys;
		}

		public static int TotalLength(int dimension, int totalDimensions, string key, ControllerContext context)
		{
			string request = string.Join(" ", GetKeys(context));
			string s = key + ".";
			for (int i=0; i<totalDimensions; ++i)
			{
				if (i != dimension)
					s += @"\d+";
				else
					s += @"(\d+)";
				if (i < totalDimensions-1)
					s += ".";
			}
			int max = int.MinValue;
			foreach (Match m in Regex.Matches(request, s, RegexOptions.IgnoreCase))
			{
				max = Math.Max(int.Parse(m.Groups[1].Value), max);
			}
			return (int) (max == int.MinValue ? 0 : max+1); // Add 1 to max because it's the total number of elements in the 0 based max index.
		}

		public static string Format(string key, params int[] index)
		{
			return key + "." + string.Join(".", index);
		}

		public abstract object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext);
	}

	/// <summary>
	/// Model binder for a 3 dimensional array
	/// </summary>
	public class Binder2D<T> : ArrayBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var values = bindingContext.ValueProvider;
			string key = bindingContext.ModelName;

			int p = TotalLength(0, 2, key, controllerContext);
			int q = TotalLength(1, 2, key, controllerContext);
			if (p == 0 || q == 0) return null;

			T[,] A = new T[p, q];
			for (int i = 0; i < p; ++i)
				for (int j = 0; j < q; ++j)
				{
					ValueProviderResult result = values.GetValue(Format(key, i, j));
					try
					{
						if (result != null && result.AttemptedValue != "")
							A[i, j] = (T)Convert.ChangeType(result.AttemptedValue, typeof(T));
					}
					catch { }
				}

			bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(A, null, System.Globalization.CultureInfo.InvariantCulture));
			return A;
		}
	}

	public class Binder3D<T> : ArrayBinder
	{

		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var values = bindingContext.ValueProvider;
			string key = bindingContext.ModelName;

			int p = TotalLength(0, 3, key, controllerContext);
			int q = TotalLength(1, 3, key, controllerContext);
			int r = TotalLength(2, 3, key, controllerContext);
			if (p == 0 || q == 0 || r == 0) return null;

			T[, ,] A = new T[p, q, r];
			for (int i = 0; i < p; ++i)
				for (int j = 0; j < q; ++j)
					for (int k = 0; k < r; ++k)
					{
						ValueProviderResult result = values.GetValue(Format(key, i, j, k));
						try
						{
							if (result != null && result.AttemptedValue != "")
								A[i, j, k] = (T)Convert.ChangeType(result.AttemptedValue, typeof(T));
						}
						catch { }
					}

			bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(A, null, System.Globalization.CultureInfo.InvariantCulture));
			return A;
		}
	}
}


