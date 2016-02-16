using System;
using Castle.DynamicProxy;

namespace EntityFrameworkLogger.Library
{
    public class LoggerServiceInterceptor : IInterceptor
    {
        private readonly LoggerService _loggerService;

        public LoggerServiceInterceptor(LoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public void Intercept(IInvocation invocation)
        {
            var attribute = (SaveChangesAttribute)Attribute.GetCustomAttribute(invocation.Method, typeof(SaveChangesAttribute));

            if (attribute != null)
                _loggerService.LogChanges();

            invocation.Proceed();
        }
    }
}
