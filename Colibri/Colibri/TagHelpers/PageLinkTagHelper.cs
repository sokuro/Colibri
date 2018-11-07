using Colibri.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.TagHelpers
{
    /*
     * Custom Tag Helper Main Class
     */
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        // using Helper Factory
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        // ViewContext-Object provides Access to Http (Request, Context, Response etc.)
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        // overriding the existing Method
        // add Pagination to the Output
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            // for each Page there will be added a Page Number
            for (int i = 1; i <= PageModel.totalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                // replace the Tags (with i)
                string url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                // if CSS enabled
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }

                // append the Tag to (inner) Html
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }

            // append the whole Result to the Html
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

}
