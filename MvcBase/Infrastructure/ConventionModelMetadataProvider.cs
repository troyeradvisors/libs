using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace MvcBase.Infrastructure
{
	public class ConventionModelMetadataProvider : DataAnnotationsModelMetadataProvider
	{
		protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
		{
			var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

			HumanizePropertyNamesAsDisplayName(metadata);


			if (metadata.DisplayName.Length > 2)
				metadata.DisplayName.Replace("ID", "");

			if (string.IsNullOrWhiteSpace(metadata.Watermark))
				metadata.Watermark = metadata.DisplayName;

			return metadata;
		}

		private void HumanizePropertyNamesAsDisplayName(ModelMetadata metadata)
		{
			metadata.DisplayName = Utility.HumanizeCamel((metadata.DisplayName ?? metadata.PropertyName));
		}


	}
}
