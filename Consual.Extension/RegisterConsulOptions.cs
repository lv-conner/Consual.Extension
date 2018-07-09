using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consul.Extension.Options
{
    public class RegisterConsulOptions
    {
        public RegisterConsulOptions()
        {
            ServcieTags = new List<string>();
        }
        //Consul地址
        public Uri ConsulAddress { get; set; }
        //Consul端口
        public string ConsulDataCenter { get; set; }
        //服务Id，需要唯一
        public string ServiceId { get; set; }
        //服务名称
        public  string ServiceName { get; set; }
        //服务地址
        public string ServiceAddress { get; set; }
        //服务端口
        public int ServicePort { get; set; }

        public List<string> ServcieTags { get; set; }
        public TimeSpan DeregisterCriticalServiceAfter { get; set; }
        //心跳检查时间间隔
        public TimeSpan HealthCheckInterval { get; set; }
        //健康检查Url
        public string HealthCheckUrl { get; set;}
        //注册超时时间
        public TimeSpan RegisterTimeOut { get; set; }
    }
}
