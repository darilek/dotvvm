﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotVVM.Framework.Hosting.ErrorPages
{
    public class ExceptionSectionFormatter : IErrorSectionFormatter
    {
        public string DisplayName => "Exception";

        public string Id => "exception";

        public ExceptionModel Exception { get; set; }

        public void WriteBody(IErrorWriter w)
        {
            WriteException(w, Exception);
        }

        protected virtual void WriteException(IErrorWriter w, ExceptionModel model)
        {
            if (model.InnerException != null)
            {
                WriteException(w, model.InnerException);
            }
            w.WriteUnencoded("<div class='exception'><span class='exceptionType'>");
            w.WriteText(model.TypeName);
            w.WriteUnencoded("</span><span class='exceptionMessage'>");
            w.WriteText(model.Message);
            w.WriteUnencoded("</span><hr />");
            if (model.AdditionalInfo != null && model.AdditionalInfo.Length > 0)
            {
                w.WriteUnencoded("<div class='exceptionAdditionalInfo'>");
                foreach (var info in model.AdditionalInfo)
                {
                    w.WriteUnencoded("<div> <h3>");
                    w.WriteText(info.Title);
                    w.WriteUnencoded("</h3>");
                    if (info.Objects != null)
                        foreach (var obj in info.Objects)
                        {
                            if (info.Display == ExceptionAdditionalInfo.DisplayMode.ToString)
                            {
                                w.WriteUnencoded("<p>" + WebUtility.HtmlEncode(obj.ToString()) + "</p>");
                            }
                            else if (info.Display == ExceptionAdditionalInfo.DisplayMode.ObjectBrowser)
                            {
                                w.ObjectBrowser(obj);
                            }
                        }
                    w.WriteUnencoded("</div><hr />");
                }
                w.WriteUnencoded("</div>");
            }
            w.ObjectBrowser(model.OriginalException);
            w.WriteUnencoded("<hr /><div class='exceptionStackTrace'>");
            foreach (var frame in model.Stack)
            {
                w.WriteUnencoded("<div class='frame'><span class='method code'>");
                w.WriteText(FormatMethod(frame.Method));
                w.WriteUnencoded(" </span>");
                if (frame.At.FileName != null) w.WriteText(frame.At.FileName + " +" + frame.At.LineNumber);
                w.WriteUnencoded("<span class='docLinks'>");
                foreach (var icon in frame.MoreInfo)
                {
                    w.WriteUnencoded("<a target=\"_blank\" href='" + icon.Link + "'>");
                    w.WriteUnencoded(icon.ContentHtml);
                    w.WriteUnencoded("</a>");
                }
                w.WriteUnencoded("</span>");
                w.WriteSourceCode(frame.At);
                w.WriteUnencoded("</div>");
            }
            w.WriteUnencoded("</div>");
            w.WriteUnencoded("</div>");
        }

        protected virtual string FormatMethod(MethodBase method)
        {
            var sb = new StringBuilder();
            if (method.DeclaringType != null)
            {
                sb.Append(method.DeclaringType.FullName);
                sb.Append(".");
            }
            sb.Append(method.Name);
            if (method.IsGenericMethod)
            {
                sb.Append("<");
                sb.Append(string.Join(", ", method.GetGenericArguments().Select(t => t.Name)));
                sb.Append(">");
            }
            sb.Append("(");
            var f = false;
            foreach (var p in method.GetParameters())
            {
                if (f) sb.Append(", ");
                else f = true;
                sb.Append(p.ParameterType.Name);
                sb.Append(' ');
                sb.Append(p.Name);
            }
            sb.Append(")");
            return sb.ToString();
        }

        public void WriteHead(IErrorWriter w)
        {
            w.WriteUnencoded(@"
.exception .exceptionType:after { content: ': '; }
.exception .exceptionType { font-size: 1.2em; font-weight: bold; }
.exception .exceptionMessage { font-style: italic; }
.exceptionStackTrace {  }
.exceptionStackTrace .frame { padding: 2px; margin: 0 0 0 30px; border-bottom: 1px #ddd solid; }
.exceptionStackTrace .frame:hover { background-color: #f0f0f0; }
");
        }
    }
}
