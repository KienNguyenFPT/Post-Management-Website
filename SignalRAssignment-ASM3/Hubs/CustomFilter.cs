using Microsoft.AspNetCore.SignalR;

namespace SignalRAssignment_ASM3.Hubs
{
    public class CustomFilter :IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            Console.WriteLine($"Calling hub method: {invocationContext.HubMethodName}");
            try
            {
                return await next(invocationContext);
            }
            catch(Exception e)
            {
                throw new Exception($"Exception calling {invocationContext.HubMethodName}: {e}");
            }
        }

        public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next){
            return next(context);
        }

        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception e, Func<HubLifetimeContext, Exception, Task> next)
        {
            return next(context, e); 
        }
    }
}
