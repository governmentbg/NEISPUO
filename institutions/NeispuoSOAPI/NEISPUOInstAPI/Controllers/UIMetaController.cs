using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NEISPUORegInstAPI.Helpers;
using Dapper;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json.Converters;
using System.IO;

namespace NEISPUORegInstAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class UIMetaController : ControllerBase
    {
        IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private bool _DEBUG;
        public UIMetaController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            _cache = memoryCache;
            _DEBUG = this.configuration.GetValue<bool>("AppSettings:Debug");
        }

        [HttpGet("get/{json}")]
        public JsonResult getmeta(string json)
        {
            string _jsonfile = Path.Combine(Constants._JSONPATH, json + ".json");
            JsonDocument _json;
            try
            {
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(_jsonfile, out _json))
                    {
                        _json = (JsonDocument)JsonDocument.Parse(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(_jsonfile, _json, cacheEntryOptions);
                    }
                }
                else
                {
                    _json = (JsonDocument)JsonDocument.Parse(System.IO.File.ReadAllText(_jsonfile));
                }

                return new JsonResult(_json.RootElement);

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

        [HttpPost("getnav/{json}")]
        public JsonResult getNavigation(string json, [FromBody]JsonElement data)
        {
            string _jsonfile = Path.Combine(Constants._JSONPATH, "navigation", json + ".json");
            JObject _json;
            try
            {
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(_jsonfile, out _json))
                    {
                        _json = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(_jsonfile, _json, cacheEntryOptions);
                    }
                }
                else
                {
                    _json = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                }

                var selected_role_json = User.Claims.FirstOrDefault(c => c.Type == "selected_role").Value;
                ExpandoObjectConverter converter = new ExpandoObjectConverter();
                dynamic claims =  JsonConvert.DeserializeObject<ExpandoObject>(selected_role_json, converter);

                long role = claims.SysRoleID;
                var filters = ((IEnumerable<dynamic>)_json["filters"]).Where(x => x.role == role).ToList();

                var claims_array = (JObject)JsonConvert.DeserializeObject(selected_role_json.ToString());

                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());

                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    var builder = new SqlBuilder();
                    DynamicParameters parameters = new DynamicParameters();
                    foreach (var param in _json["params"])
                    {
                        if (Convert.ToBoolean(param["static"]) == false)
                        {
                            parameters.Add(param["param"].ToString(), _data[param["value"].ToString()].ToString(), System.Data.DbType.Int32, ParameterDirection.Input);
                            builder.Where(String.Format("{0} {1} {2}", param["column"], "=", param["param"]));
                        }
                        else
                        {
                            parameters.Add(param["param"].ToString(), int.Parse(param["value"].ToString()), System.Data.DbType.Int32, ParameterDirection.Input);
                            builder.Where(String.Format("{0} {1} {2}", param["column"], "=", param["value"]));
                        }

                    }
                    foreach (var filter in filters)
                    {
                        int claim = int.Parse(claims_array[(string)filter["claim"]].ToString());
                        parameters.Add(filter["param"].ToString(), claim, System.Data.DbType.Int32, ParameterDirection.Input);
                        builder.Where(String.Format("{0} {1} {2}", filter["column"], "=", filter["param"]));
                    }
                    builder.AddParameters(parameters);
                    var sql = builder.AddTemplate(_json["template"].ToString()).RawSql;
                    var db = connection.Query(builder.AddTemplate(_json["template"].ToString()).RawSql, parameters).AsQueryable();

                    var levels = ((IEnumerable<dynamic>)_json["levels"]).Where(x => x.role == role).First();
                    var res = parseLevels(db, (JToken)levels["level"], (string)_json["outputColumns"]);

                    return new JsonResult(new
                    {
                        Status = 1,
                        Data = res
                    });
                }

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

        private dynamic parseLevels(IQueryable db, JToken _json, string outputColumns, string parentKey = null, int? parentValue = null)
        {
            List<dynamic> res = new List<dynamic>();
            if (!((JArray)_json["sublevel"]).HasValues && parentKey == null)
            {
                res = db.OrderBy((string)_json["orderBy"]).Select("new { " + outputColumns + "}").ToDynamicList();
                return res;
            }
            if (!((JArray)_json["sublevel"]).HasValues)
            {
                res = db.Where("Convert.ToInt64(" + parentKey + ") = " + parentValue).OrderBy((string)_json["orderBy"]).Select("new { " + outputColumns + "}").ToDynamicList();
                return res;
            }
            string _select = "new { " + (string)_json["id"] + " as id, " + (string)_json["name"] + " as name }";
            IQueryable disDBRes = db;
            
            if(parentKey != null)
            {
                string _where = "Convert.ToInt64(" + parentKey + ") = " + parentValue;
                disDBRes = disDBRes.Where(_where).OrderBy((string)_json["orderBy"]).Select(_select).Distinct();
            } else
            {
                disDBRes = disDBRes.OrderBy((string)_json["orderBy"]).Select(_select).Distinct();
            }
            foreach(var row in disDBRes.ToDynamicList())
            {
                dynamic obj = new ExpandoObject();
                obj.name = row.name;

                foreach (JObject subs in _json["sublevel"])
                {
                    obj.children = parseLevels(db, subs, outputColumns, (string)subs["parentKey"], row.id);
                }
                
                res.Add(obj);
            }
            
            return res;
        }
    }
}