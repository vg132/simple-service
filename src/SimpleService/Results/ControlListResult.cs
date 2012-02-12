using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace SimpleService {
    public class ControlListResult<TControl, TModel> : ServiceResult where TControl : Control {
        public string Path { get; set; }
        public IEnumerable<TModel> Model { get; set; }
        public Action<TControl, TModel> ControlAction { get; set; }

        public override void Execute(ServiceContext context) {
            var sb = new StringBuilder();
            foreach (var model in Model) {
                var renderedControlHtml = GetRenderedControlHtml(model);
                sb.Append(renderedControlHtml);
            }
            context.HttpContext.Response.ContentType = "text/html";
            context.HttpContext.Response.Write(sb.ToString());
        }

        private string GetRenderedControlHtml(TModel model) {
            if (string.IsNullOrEmpty(Path))
                Path = ResolveControlPath(typeof(TControl));

            var page = new Page();
            var control = page.LoadControl(Path);
            
            if (control is IModelTemplate<TModel>)
                ((IModelTemplate<TModel>) control).Model = model;

            if (ControlAction != null) {
                ControlAction((TControl)control, model);
            }

            page.Controls.Add(control);
            using (var stringWriter = new StringWriter()) {
                HttpContext.Current.Server.Execute(page, stringWriter, false);
                return stringWriter.ToString();
            }
        }

        public virtual string ResolveControlPath(Type controlType) {
            // TODO move this method into a new class. duplicate exists in ControlResult.cs
            string controlNamespace = controlType.Namespace;
            var assemblyNamespace = controlType.Assembly.FullName.Split(',')[0];
            controlNamespace = controlNamespace.Replace(assemblyNamespace + ".", string.Empty).Replace(".", "/");

            return string.Format("~/{0}/{1}.ascx", controlNamespace, controlType.Name);
        }
    }
}
