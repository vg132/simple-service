using System;
using SimpleService.SampleWebForms.Controls;
using SimpleService.SampleWebForms.Controls.SubFolder;

namespace SimpleService.SampleWebForms.Services {
    public class HelloWorldService : SimpleWebService {
        public void HelloWorld(string q) {
            Response.Write("Hello " + q);
        }

        public ServiceResult ControlTest(string format) {
            return Control<NewsPost2>(controlAction: c => c.SetName(format));
        }

        public ServiceResult ControlListTest() {
            string[] names = new[] { "Per", "Anna", "Sarah", "Nisse", "Karl" };

            return ControlList<NewsPost, string>(names, (control, name) => control.SetName(name));
        }

        public ServiceResult JsonTest(string firstName, string lastName) {
            return Json(new { message = "Hello " + firstName + " " + lastName, date = DateTime.Now });
        }

        public ServiceResult ContentTest() {
            return Content("Hello");
        }

        public ServiceResult ErrorTest() {
            throw new Exception("Fail fail fail");
        }
    }
}