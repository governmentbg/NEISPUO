using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NEISPUORegInstAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEISPUORegInstAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class SysController : ControllerBase
    {
        IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private bool _DEBUG;
        public SysController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            _cache = memoryCache;
            _DEBUG = this.configuration.GetValue<bool>("AppSettings:Debug");
        }

        [HttpGet("resetcache")]
        public JsonResult ResetCache()
        {
            try
            {
                MemoryCache cache = (MemoryCache)this._cache;
                cache.Compact(1.0);
                return new JsonResult(new
                {
                    Status = 1
                });
            }
            catch (Exception ex)
            {
                if (!_DEBUG)
                {
                    return new JsonResult(new
                    {
                        Status = 0
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        Status = 0,
                        Exception = ex.Message
                    });
                }
            }
            
        }
        [HttpPost("savemetafile/{filename}")]
        public JsonResult savemetafile(string filename, [FromBody] JsonElement data)
        {
            //try
            //{
            
            string _jsonfile = Constants._JSONPATH + filename + ".json";
            string _backuppath = Constants._JSONPATH + "\\JSONMetaBackups\\";
            string _backupjsonfile = _backuppath + filename + ".jsonback_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            JsonDocument _json;
            _json = (JsonDocument)JsonDocument.Parse(data.ToString());
            if (System.IO.File.Exists(_jsonfile))
            {
                if (!System.IO.Directory.Exists(_backuppath)) System.IO.Directory.CreateDirectory(_backuppath);
                System.IO.File.Copy(_jsonfile, _backupjsonfile);
                var dataStr = data.ToString();
                System.IO.File.WriteAllText(_jsonfile, PrettyJson(data.ToString()));
            } else
            {
                return new JsonResult(new
                {
                    Status = 0
                });
            }
            MemoryCache cache = (MemoryCache)this._cache;
            cache.Compact(1.0);
            return new JsonResult(new
            {
                Status = 1
            });
            //} catch (Exception ex)
            //{
            //    if(!_DEBUG)
            //    {
            //        return new JsonResult(new
            //        {
            //            Status = 0
            //        });
            //    } else
            //    {
            //        return new JsonResult(new
            //        {
            //            Status = 0,
            //            Exception = ex.Message
            //        });
            //    }
            //}

        }

        public string PrettyJson(string unPrettyJson)
        {
            JToken jt = JToken.Parse(unPrettyJson);
            string formatted = jt.ToString(Newtonsoft.Json.Formatting.Indented);
            return formatted;
        }
    }
}