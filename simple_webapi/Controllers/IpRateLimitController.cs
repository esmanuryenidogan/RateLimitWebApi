using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simple_webapi.Controllers
{
    [Route("Api/IpRateLimit")]
    [ApiController]
    public class IpRateLimitController : ControllerBase
    {
        private readonly IpRateLimitOptions _options;
        private readonly IIpPolicyStore _ipPolicyStore;

        public IpRateLimitController(IOptions<IpRateLimitOptions> optionsAccessor, IIpPolicyStore ipPolicyStore)
        {
            _options = optionsAccessor.Value;
            _ipPolicyStore = ipPolicyStore;
        }

        [Route("GetIpRateLimitPolicies"),HttpGet]
        public async Task<IpRateLimitPolicies> Get()
        {
            return await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);
        }

        [Route("PostIpRateLimitPolicy"), HttpPost]
        public async void Post()
        {
            var pol = await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);

            pol.IpRules.Add(new IpRateLimitPolicy
            {
                Ip = "8.8.4.4",
                Rules = new List<RateLimitRule>(new RateLimitRule[] {
                new RateLimitRule {
                    Endpoint = "*:/Api/WeatherForecast",
                    Limit = 10,
                    Period = "1d" }
            })
            });

            await _ipPolicyStore.SetAsync(_options.IpPolicyPrefix, pol);
        }
    }
}
