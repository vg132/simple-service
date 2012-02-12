using System;
using System.Web.UI;

namespace SimpleService.SampleWebForms.Controls {
    public partial class NewsPost : UserControl, IModelTemplate<string> {
        protected void Page_Load(object sender, EventArgs e) {
            string[] names = new [] {"Per", "Anna", "Sarah", "Nisse", "Karl"};
            MyList.DataSource = names;
            MyList.DataBind();
        }

        public void SetName(string name) {
            literalName.Text = name;
        }

        public string Model {
            get { return literalName.Text; }
            set { SetName(value); }
        }
    }
}