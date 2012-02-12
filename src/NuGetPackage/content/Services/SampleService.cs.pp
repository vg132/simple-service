using SimpleService;

// Open a web browser and navigate to /services/sample

namespace $rootnamespace$.Services {
    public class SampleService : SimpleWebService {
        public ServiceResult HelloJson(string name) {
            if (name == "error")
                return Error("Bad name!");

            return Json(new { hello = name });
        }

        public ServiceResult HelloContent(string name) {
            return Content("Hello " + name);
        }
        
        /*
        
        public ServiceResult HelloControl(string name) {
            return Control<MyControl>(control => {
                control.Name = name;
            });
        }
        
        public ServiceResult HelloControlList(string name) {
            string[] names = new[] { "John", "Tom", "Harry", "Richard" };

            return ControlList<MyControl, string>((control, model) => {
                control.Name = model;
            });
        }
        
        public ServiceResult HelloControlList(string name) {
            string[] names = new[] { "John", "Tom", "Harry", "Richard" };
            
            // if the user control implements IModelTemplate, we can simplify the call
            return ControlList<MyControl, string>(names);
        }

        */
    }
}