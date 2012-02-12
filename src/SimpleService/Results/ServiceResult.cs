using System.Web;

namespace SimpleService {
    public abstract class ServiceResult {
        public abstract void Execute(ServiceContext context);
    }
}