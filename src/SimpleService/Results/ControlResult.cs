using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace SimpleService {
    public class ControlResult<TControl> : ServiceResult where TControl : Control {
        public string Path { get; set; }
        public Action<TControl> ControlAction { get; set; }

        public override void Execute(ServiceContext context) {
            string controlHtml = GetRenderedControlHtml(Path, ControlAction);

            context.HttpContext.Response.ContentType = "text/html";
            context.HttpContext.Response.Write(controlHtml);
        }

        public virtual string GetRenderedControlHtml<T>(string path = null, Action<T> controlAction = null) where T : Control {
            if (string.IsNullOrEmpty(path))
                path = ResolveControlPath(typeof(TControl));

            Page page = new Page();
            Control control = page.LoadControl(path);
            
            if (controlAction != null)
                controlAction((T)control);

            page.Controls.Add(control);

            using (StringWriter output = new StringWriter()) {
                HttpContext.Current.Server.Execute(page, output, false);
                return output.ToString();
            }
        }

        public virtual string ResolveControlPath(Type controlType) {
            // TODO move this method into a new class. duplicate exists in ControlListResult.cs
            string controlNamespace = controlType.Namespace;
            var assemblyNamespace = controlType.Assembly.FullName.Split(',')[0];
            controlNamespace = controlNamespace.Replace(assemblyNamespace + ".", string.Empty).Replace(".", "/");

            return string.Format("~/{0}/{1}.ascx", controlNamespace, controlType.Name);
        }
    }
}