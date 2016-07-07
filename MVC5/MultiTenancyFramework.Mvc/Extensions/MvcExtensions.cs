using MultiTenancyFramework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MultiTenancyFramework
{
    public static class MvcExtensions
    {
        public static async Task<FileContentResult> ExportFile(this MyDataTable dataTable, string exportType, string entityName, IDictionary<string, object> headerItems = null)
        {
            byte[] theBytes = null;
            string fileName = DateTime.Now.GetLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + "_" + entityName;
            string mimeType = "";
            switch (exportType)
            {
                //case "pdf":
                //    mimeType = "application/pdf";
                //    fileName += ".pdf";
                //    theBytes = new byte[0];
                //    break;
                case "csv":
                default:
                    mimeType = "text/csv";
                    fileName += ".csv";
                    theBytes = await dataTable.ToCSV(headerItems);
                    break;
                    //default:
                    //    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // "application/ms-excel"; //
                    //    fileName += ".xlsx";
                    //    theBytes = ToExcel(dataTable, headerItems);
                    //    break;
            }
            return new FileContentResult(theBytes, mimeType) { FileDownloadName = fileName };
        }

        public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
        {
            var concatKeys = new string[] { "class" };

            var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
            var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

            RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
            RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
                ? new RouteValueDictionary(defaultHtmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

            foreach (var item in htmlAttributes)
            {
                if (concatKeys.Contains(item.Key))
                {
                    defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null)
                        ? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value)
                        : item.Value;
                }
                else
                {
                    defaultHtmlAttributes[item.Key] = item.Value;
                }
            }

            return defaultHtmlAttributes;
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return htmlHelper.MyEnumDropDownListFor(expression, null, null);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return htmlHelper.MyEnumDropDownListFor(expression, null, htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionalLabel, object htmlAttributes)
        {
            var enumType = typeof(TEnum);
            if (enumType.IsNullable())
            {
                enumType = Nullable.GetUnderlyingType(enumType);
            }
            var list = EnumHelper.GetEnumNames(enumType);
            return htmlHelper.MyEnumDropDownListFor(expression, list, null, htmlAttributes);
        }

        public static MvcHtmlString MyEnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IEnumerable enumList, string optionalLabel, object htmlAttributes)
        {
            string expressionName = ExpressionHelper.GetExpressionText(expression);
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionName);

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata == null)
            {
                throw new InvalidOperationException("Could not get metadata from expression");
            }

            if (metadata.ModelType == null)
            {
                throw new InvalidOperationException("Null value for metadata's ModelType");
            }
            Enum currentValue = null;
            if (!string.IsNullOrWhiteSpace(expressionFullName))
            {
                currentValue = GetValueFromModel(htmlHelper, expressionFullName, metadata.ModelType) as Enum;
            }

            if (currentValue == null && !string.IsNullOrWhiteSpace(expressionName))
            {
                currentValue = htmlHelper.ViewData.Eval(expressionName) as Enum;
            }

            if (currentValue == null)
            {
                currentValue = metadata.Model as Enum;
            }

            var selectList = new SelectList(enumList, "Value", "Name", currentValue);

            var attr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (attr.ContainsKey("class"))
            {
                if (!attr["class"].ToString().Contains("form-control"))
                {
                    attr["class"] = "form-control " + attr["class"];
                }
            }
            else
            {
                attr["class"] = "form-control";
            }
            if (metadata.IsRequired)
            {
                attr["required"] = "required";
                return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, attr);
            }
            return htmlHelper.DropDownListFor(expression, selectList, optionalLabel, htmlAttributes);
        }

        private static object GetValueFromModel(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    destinationType = destinationType.GetEnumUnderlyingType();
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }
    }
}
