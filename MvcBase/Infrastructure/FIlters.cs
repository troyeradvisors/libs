using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcBase.Infrastructure
{
	public class LocalLoginAttribute : AuthorizeAttribute
	{
		private string user;
		public LocalLoginAttribute(string user)
		{
			Order = -1;
			this.user = user;
		}

		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			if (!httpContext.Request.IsAuthenticated && httpContext.Request.IsLocal)
				FormsAuthentication.SetAuthCookie(user, true);				
			return true;
		}
	}
}
