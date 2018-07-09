using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Consul.Extension.Options;
using Consul;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsulExtension
    {
        //将服务注册到Consul中
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder application)
        {
            var options = application.ApplicationServices.GetService<IOptions<RegisterConsulOptions>>()?.Value;
            if(options == null)
            {
                throw new ArgumentNullException("RegisterConsulOptions");
            }
            IApplicationLifetime applicationLifetime = application.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                using (var client = new ConsulClient(c => { c.Address = options.ConsulAddress; c.Datacenter = options.ConsulDataCenter; }))
                {
                    //此处需要以同步运行，因为client包含在using中，并且即使不在using中，在此方法运行完成后，client也会被GC，中断注册请求。因此需要显式await。
                    client.Agent.ServiceRegister(new AgentServiceRegistration()
                    {
                        ID = options.ServiceId,//服务编号保证不重复
                        Name = options.ServiceName,//服务的名称
                        Address = options.ServiceAddress,//服务ip地址
                        Port = options.ServicePort,//服务端口
                        Check = new AgentServiceCheck //健康检查
                        {
                            DeregisterCriticalServiceAfter = options.DeregisterCriticalServiceAfter,//服务启动多久后反注册
                            Interval = options.HealthCheckInterval,//健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                            HTTP = options.HealthCheckUrl,//健康检查地址
                            Timeout = options.RegisterTimeOut//服务的注册时间
                        },
                        Tags = options.ServcieTags.ToArray()
                    }).GetAwaiter().GetResult();
                }
            });
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                using (var client = new ConsulClient(c => { c.Address = options.ConsulAddress; c.Datacenter = options.ConsulDataCenter; }))
                {
                    client.Agent.ServiceDeregister(options.ServiceId);
                }
            });
            return application;
        }
    }
}
