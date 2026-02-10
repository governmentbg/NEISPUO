using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NEISPUORegInstAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dapper;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Dynamic.Core;
using RazorLight;
using DinkToPdf;
using System.IO;
using DinkToPdf.Contracts;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Claims;

namespace NEISPUORegInstAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class DataController : ControllerBase
    {
        IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private bool _DEBUG;
        IConverter _converter;
        public DataController(IConfiguration configuration, IMemoryCache memoryCache, IConverter converter)
        {
            this.configuration = configuration;
            _cache = memoryCache;
            _DEBUG = this.configuration.GetValue<bool>("AppSettings:Debug");
            _converter = converter;
        }

        //[HttpPost("post/{jsonfile}")]
        //public JsonResult post(string jsonfile, [FromBody]JsonElement data)
        //{
        //    string _jsonfile = Constants._JSONPATH + "\\DB\\" + jsonfile + ".json";
        //    JObject obj;
        //    if (!_DEBUG)
        //    {
        //        if (!_cache.TryGetValue(jsonfile, out obj))
        //        {
        //            obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
        //            var cacheEntryOptions = new MemoryCacheEntryOptions()
        //                .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
        //            _cache.Set(jsonfile, obj, cacheEntryOptions);
        //        }
        //    }
        //    else
        //    {
        //        obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
        //    }


        //    dynamic _postdata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), new ExpandoObjectConverter());

        //    List<dynamic> res = new List<dynamic>();
        //    foreach (var property in (IDictionary<String, Object>)_postdata)
        //    {
        //        foreach (var column in obj["columns"])
        //        {
        //            var _col = column["column"].ToString();
        //            if (_col.Contains(property.Key))
        //            {
        //                dynamic _colobj = new ExpandoObject();
        //                _colobj.colname = _col.Split(" AS ")[0].Split('.')[1];
        //                _colobj.alias = _col.Split(" AS ")[0].Split('.')[0];
        //                _colobj.value = property.Value;
        //                res.Add(_colobj);
        //            }
        //        }
        //    }

        //    return new JsonResult(new
        //    {
        //        Status = "1",
        //        Res = res
        //    });
        //}

        [HttpGet("file/certificate/{blobId:int}/{submissionDataId:int}")]
        public ActionResult downloadCertificateFile(int blobId, int submissionDataId)
        {
            var selected_role_json = User.Claims.FirstOrDefault(c => c.Type == "selected_role").Value;
            var claims = (JObject)JsonConvert.DeserializeObject(selected_role_json.ToString());

            var sysUserId = claims["SysUserID"].Type != JTokenType.Null ? (int)claims["SysUserID"] : (int?)null;
            var sysRoleId = claims["SysRoleID"].Type != JTokenType.Null ? (int)claims["SysRoleID"] : (int?)null;
            var institutionId = claims["InstitutionID"].Type != JTokenType.Null ? (int)claims["InstitutionID"] : (int?)null;
            var regionId = claims["RegionID"].Type != JTokenType.Null ? (int)claims["RegionID"] : (int?)null;
            var municipalityId = claims["MunicipalityID"].Type != JTokenType.Null ? (int)claims["MunicipalityID"] : (int?)null;
            var budgetingInstitutionId = claims["BudgetingInstitutionID"].Type != JTokenType.Null ? (int)claims["BudgetingInstitutionID"] : (int?)null;

            using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
            {
                var procname = "inst_basic.generateSOUrlDocument";

                var values = new { sysUserId, sysRoleId, institutionId, regionId, municipalityId, budgetingInstitutionId, blobId, submissionDataId };
                var res = connection.Query(procname, values, commandType: CommandType.StoredProcedure);
                var location = res.First().generatedUrl;

                if (location != null)
                {
                    return Redirect(location);
                }
                else
                {
                    throw new System.InvalidOperationException(string.Format("Нямате права да извършите това действие"));
                }
            }
        }

        [HttpGet("file/building/{buildingId:int}/{initialOwnershipDoc:int}/{latestOwnershipDoc:int}")]
        public ActionResult downloadBuildingFile(int buildingId, int? initialOwnershipDoc, int? latestOwnershipDoc)
        {
            var selected_role_json = User.Claims.FirstOrDefault(c => c.Type == "selected_role").Value;
            var claims = (JObject)JsonConvert.DeserializeObject(selected_role_json.ToString());

            var sysUserId = claims["SysUserID"].Type != JTokenType.Null ? (int)claims["SysUserID"] : (int?)null;
            var sysRoleId = claims["SysRoleID"].Type != JTokenType.Null ? (int)claims["SysRoleID"] : (int?)null;
            var institutionId = claims["InstitutionID"].Type != JTokenType.Null ? (int)claims["InstitutionID"] : (int?)null;
            var regionId = claims["RegionID"].Type != JTokenType.Null ? (int)claims["RegionID"] : (int?)null;
            var municipalityId = claims["MunicipalityID"].Type != JTokenType.Null ? (int)claims["MunicipalityID"] : (int?)null;
            var budgetingInstitutionId = claims["BudgetingInstitutionID"].Type != JTokenType.Null ? (int)claims["BudgetingInstitutionID"] : (int?)null;
            initialOwnershipDoc = initialOwnershipDoc > 0 ? initialOwnershipDoc : null;
            latestOwnershipDoc = latestOwnershipDoc > 0 ? latestOwnershipDoc : null;

            using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
            {
                var procname = "inst_basic.generateBuildingUrlDocument";

                var values = new { sysUserId, sysRoleId, institutionId, regionId, municipalityId, budgetingInstitutionId, buildingId, initialOwnershipDoc, latestOwnershipDoc };
                var res = connection.Query(procname, values, commandType: CommandType.StoredProcedure);
                var location = res.First().generatedUrl;

                if (location != null)
                {
                    return Redirect(location);
                }
                else
                {
                    throw new System.InvalidOperationException(string.Format("Нямате права да извършите това действие"));
                }
            }
        }

        [HttpGet("file/classGroup/{blobId:int}/{classId:int}")]
        public ActionResult downloadClassGroupFile(int blobId, int classId)
        {
            var selected_role_json = User.Claims.FirstOrDefault(c => c.Type == "selected_role").Value;
            var claims = (JObject)JsonConvert.DeserializeObject(selected_role_json.ToString());

            var sysUserId = claims["SysUserID"].Type != JTokenType.Null ? (int)claims["SysUserID"] : (int?)null;
            var sysRoleId = claims["SysRoleID"].Type != JTokenType.Null ? (int)claims["SysRoleID"] : (int?)null;
            var institutionId = claims["InstitutionID"].Type != JTokenType.Null ? (int)claims["InstitutionID"] : (int?)null;
            var regionId = claims["RegionID"].Type != JTokenType.Null ? (int)claims["RegionID"] : (int?)null;
            var municipalityId = claims["MunicipalityID"].Type != JTokenType.Null ? (int)claims["MunicipalityID"] : (int?)null;
            var budgetingInstitutionId = claims["BudgetingInstitutionID"].Type != JTokenType.Null ? (int)claims["BudgetingInstitutionID"] : (int?)null;

            using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
            {
                var procname = "inst_basic.generateClassGroupUrlDocument";

                var values = new { sysUserId, sysRoleId, institutionId, regionId, municipalityId, budgetingInstitutionId, blobId, classId };
                var res = connection.Query(procname, values, commandType: CommandType.StoredProcedure);
                var location = res.First().generatedUrl;

                if (location != null)
                {
                    return Redirect(location);
                }
                else
                {
                    throw new System.InvalidOperationException(string.Format("Нямате права да извършите това действие"));
                }
            }
        }

        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
           "us-ascii",
           new EncoderExceptionFallback(),
           new DecoderExceptionFallback()
        );

        [HttpPost("save")]
        public JsonResult save([FromBody]JsonElement data)
        {
            //try
            //{
                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());
                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    var procname = _data["procedureName"].ToString();
                    var operationtype = Convert.ToInt32(_data["operationType"]);

                    var values = new { _json = _data["data"].ToString(), OperationType = operationtype };
                    var results = JsonConvert.SerializeObject(connection.Query(procname, values, commandType: CommandType.StoredProcedure).ToList());
                    return new JsonResult(new
                    {
                        Status = 1,
                        Data = results
                    });
                }
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

        [HttpPost("getGRAOData")]
        public JsonResult GetGraoData([FromBody] JsonElement data)
        {
            var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());
            var certName = Path.Combine(Constants._CERTPATH, this.configuration.GetValue<string>("AppSettings:CertName")); 
            var certPass = this.configuration.GetValue<string>("AppSettings:CertPass");
            RegiXHelper regiX = new RegiXHelper();
            RegiXHelper.RegixBasicRequest _req = new RegiXHelper.RegixBasicRequest();
            if((int)_data["idType"] == 1)
            {
                _req.Operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.PersonDataSearch";
                _req.Argument = String.Format(@"<PersonDataRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://egov.bg/RegiX/GRAO/NBD/PersonDataRequest""><EGN>{0}</EGN></PersonDataRequest>",
                    _data["idNumber"].ToString());
                RegiXHelper.RegixResponse response = regiX.AskRegix(_req, certName, certPass);
            } else if((int)_data["idType"] == 2)
            {
                _req.Operation = "TechnoLogica.RegiX.MVRERChAdapter.APIService.IMVRERChAPI.GetForeignIdentity";
                _req.Argument = String.Format(@"<ForeignIdentityInfoRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest""><IdentifierType>LNCh</IdentifierType><Identifier>{0}</Identifier></ForeignIdentityInfoRequest>",
                    _data["idNumber"].ToString());
                RegiXHelper.RegixResponse response = regiX.AskRegix(_req, certName, certPass);
            }

            return new JsonResult(new
            {
                Status = 0
            });
        }

        [HttpPost("getTRData")]
        public JsonResult GetTRData(string jsonfile, [FromBody] JsonElement data)
        {
            try
            {
                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());
                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    SQLBuilderObject mainQuery = new SQLBuilderObject();
                    mainQuery.SQL = "SELECT * FROM [" + connection.Database + "].[reginst_basic].[TRDataCheck] WHERE Bulstat = @EIK AND InstKind = @instKind";
                    mainQuery.Params = new DynamicParameters();
                    mainQuery.Params.Add("@EIK", _data["eik"].ToString(), DbType.String, ParameterDirection.Input);
                    mainQuery.Params.Add("@instKind", _data["instKind"].ToString(), DbType.Int32, ParameterDirection.Input);

                    int inst_kind = Convert.ToInt32(_data["instKind"].ToString());

                    IEnumerable<dynamic> res = connection.Query(mainQuery.SQL, mainQuery.Params);

                    if(res.Count() != 0 && (inst_kind != 3 && inst_kind != 7))
                    {
                        return new JsonResult(new
                        {
                            Status = -1,
                            Data = "existEIK"
                        });
                    }
                    else if (res.Count() != 0 && (inst_kind == 3 || inst_kind == 7) && res.First().IsRIActive == 1)
                    {
                        return new JsonResult(new
                        {
                            Status = -1,
                            Data = "existEIK"
                        });
                    }
                    else
                    {
                        // Bulstat Call
                        //String Bresponse = System.IO.File.ReadAllText(Constants._JSONPATH + "\\tr_test\\StateOfPlayResponse.xml");
                        mainQuery.SQL = "SELECT COUNT([RITRDataID]) FROM [" + connection.Database + "].[reginst_basic].[RITRData] WHERE EIK = @EIK";
                        mainQuery.Params = new DynamicParameters();
                        mainQuery.Params.Add("@EIK", _data["eik"].ToString(), DbType.String, ParameterDirection.Input);
                        int count = connection.QuerySingle<int>(mainQuery.SQL, mainQuery.Params);

                        if (count != 0)
                        {
                            return new JsonResult(new
                            {
                                Status = 1,
                                Data = "OK"
                            });
                        }
                        else
                        {
                            // Bulstat Call
                            //String Bresponse = System.IO.File.ReadAllText(Constants._JSONPATH + "\\tr_test\\StateOfPlayResponse.xml");
                            return new JsonResult(new
                            {
                                Status = -2,
                                Data = "missingEIK"
                            });
                        }
                        
                    }
                    
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

        [HttpPost("get/{jsonfile}")]
        public JsonResult GetData(string jsonfile, [FromBody] JsonElement data)
        {
            try
            {
                string _jsonfile = Path.Combine(Constants._JSONPATH,"DB", jsonfile + ".json");
                JObject obj;
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(jsonfile, out obj))
                    {
                        obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(jsonfile, obj, cacheEntryOptions);
                    }
                }
                else
                {
                    obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                }

                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());

               


                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    Dapper.SqlMapper.Settings.CommandTimeout = 0;
                    SQLBuilderObject mainQuery = this.BuildSQL(obj, _data);
                    dynamic res;
                    res = (IEnumerable<dynamic>)connection.Query(mainQuery.SQL, mainQuery.Params,null,true, commandTimeout: 0);
                    foreach(var resultRow in res)
                    {
                        foreach(var inSelect in obj["inSelects"])
                        {
                            SQLBuilderObject subQuery = this.BuildSQL(inSelect, _data);
                            IEnumerable<dynamic> subRes = connection.Query(subQuery.SQL, subQuery.Params);
                            if (((JObject)inSelect).ContainsKey("inSelects"))
                            {
                                foreach(var inSelect2 in inSelect["inSelects"])
                                {
                                    foreach(var subResRow in subRes)
                                    {
                                        SQLBuilderObject subQuery2 = this.BuildSQL(inSelect2, (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(subResRow)));
                                        IEnumerable<dynamic> subRes2 = connection.Query(subQuery2.SQL, subQuery2.Params);
                                        ((IDictionary<string, Object>)subResRow).Add(inSelect2["arrayName"].ToString(), subRes2);
                                    }
                                }
                            }
                            ((IDictionary<string, Object>)resultRow).Add(inSelect["arrayName"].ToString(),subRes);
                        }
                    }

                    if (obj.ContainsKey("arrayName"))
                    {
                        Dictionary<string,Object> subRes = new Dictionary<string, Object>();
                        subRes.Add(obj["arrayName"].ToString(), res);
                        res = subRes;
                    }

                    if (!_DEBUG)
                    {
                        return new JsonResult(new
                        {
                            Status = 1,
                            Data = res
                        });
                    } else
                    {
                        return new JsonResult(new
                        {
                            Status = 1,
                            SQL = mainQuery.SQL,
                            Data = res
                        });
                    }
                   
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine("here");
                if(ex.Message.Contains("Operation is forbidden"))
                {
                    return new JsonResult(new
                    {
                        Status = 403,
                        Exception = "Operation is forbidden"
                    });
                }
                else if(!_DEBUG)
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
                        Exception = ex.ToString()
                    });
                }
            }
           

            
        }

        [HttpPost("getso/{jsonfile}")]
        public JsonResult GetSOData(string jsonfile, [FromBody] JsonElement data)
        {
            try
            {
                string _jsonfile = Path.Combine(Constants._JSONPATH, "DB", jsonfile + ".json");
                JObject obj;
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(jsonfile, out obj))
                    {
                        obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(jsonfile, obj, cacheEntryOptions);
                    }
                }
                else
                {
                    obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                }

                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());




                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    Dapper.SqlMapper.Settings.CommandTimeout = 0;
                    SQLBuilderObject mainQuery = this.BuildSQL(obj, _data, "sotemplate");
                    dynamic res;
                    res = (IEnumerable<dynamic>)connection.Query(mainQuery.SQL, mainQuery.Params, null, true, commandTimeout: 0);
                    foreach (var resultRow in res)
                    {
                        foreach (var inSelect in obj["inSelects"])
                        {
                            SQLBuilderObject subQuery = this.BuildSQL(inSelect, _data, "sotemplate");
                            IEnumerable<dynamic> subRes = connection.Query(subQuery.SQL, subQuery.Params);
                            if (((JObject)inSelect).ContainsKey("inSelects"))
                            {
                                foreach (var inSelect2 in inSelect["inSelects"])
                                {
                                    foreach (var subResRow in subRes)
                                    {
                                        SQLBuilderObject subQuery2 = this.BuildSQL(inSelect2, (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(subResRow)), "sotemplate");
                                        IEnumerable<dynamic> subRes2 = connection.Query(subQuery2.SQL, subQuery2.Params);
                                        ((IDictionary<string, Object>)subResRow).Add(inSelect2["arrayName"].ToString(), subRes2);
                                    }
                                }
                            }
                            ((IDictionary<string, Object>)resultRow).Add(inSelect["arrayName"].ToString(), subRes);
                        }
                    }

                    if (obj.ContainsKey("arrayName"))
                    {
                        Dictionary<string, Object> subRes = new Dictionary<string, Object>();
                        subRes.Add(obj["arrayName"].ToString(), res);
                        res = subRes;
                    }

                    if (!_DEBUG)
                    {
                        return new JsonResult(new
                        {
                            Status = 1,
                            Data = res
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            Status = 1,
                            SQL = mainQuery.SQL,
                            Data = res
                        });
                    }

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
                        Exception = ex.ToString()
                    });
                }
            }



        }


        [HttpPost("generatepdf/{jsonfile}")]
        public async Task<IActionResult> Generate(string jsonfile, [FromBody] JsonElement data)
        {
            try
            {
                string _jsonfile = Path.Combine(Constants._JSONPATH, "DB", jsonfile + ".json");
                JObject obj;
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(jsonfile, out obj))
                    {
                        obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(jsonfile, obj, cacheEntryOptions);
                    }
                }
                else
                {
                    obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                }

                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());

                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    SQLBuilderObject mainQuery = this.BuildSQL(obj, _data);
                    dynamic res;
                    res = (IEnumerable<dynamic>)connection.Query(mainQuery.SQL, mainQuery.Params);
                    foreach (var resultRow in res)
                    {
                        foreach (var inSelect in obj["inSelects"])
                        {
                            SQLBuilderObject subQuery = this.BuildSQL(inSelect, _data);
                            IEnumerable<dynamic> subRes = connection.Query(subQuery.SQL, subQuery.Params);
                            ((IDictionary<string, Object>)resultRow).Add(inSelect["arrayName"].ToString(), subRes);
                        }
                    }

                    if (obj.ContainsKey("arrayName"))
                    {
                        Dictionary<string, Object> subRes = new Dictionary<string, Object>();
                        subRes.Add(obj["arrayName"].ToString(), res);
                        res = subRes;
                    }

                    string _jsonres = JsonConvert.SerializeObject(res);
                    ExpandoObjectConverter exconverter = new ExpandoObjectConverter();


                    dynamic _res = new ExpandoObject();

                    _res.data = JsonConvert.DeserializeObject<List<ExpandoObject>>(_jsonres, exconverter);
                    
                    var uri = new System.Uri(Constants._JSONPATH + "\\PDFTemplates\\" + jsonfile);
                    _res.path = uri.AbsoluteUri;

                    string _pdftemplate = Constants._JSONPATH + "\\PDFTemplates\\" + jsonfile;
                    var engine = new RazorLightEngineBuilder()
                    .UseFileSystemProject(_pdftemplate)
                    .UseMemoryCachingProvider()
                    .Build();
                    string result = await engine.CompileRenderAsync("template.cshtml", _res);

                    //var converter = new SynchronizedConverter(new PdfTools());

                    var doc = new HtmlToPdfDocument()
                    {
                        GlobalSettings = {
                            ColorMode = ColorMode.Color,
                            Orientation = Orientation.Portrait,
                            PaperSize = PaperKind.A4,
                            Margins = new MarginSettings {Top=0,Bottom=0,Left=0,Right=0},

                        },
                        Objects = {
                            new ObjectSettings() {
                                PagesCount = true,
                                HtmlContent = result,
                                WebSettings = { DefaultEncoding = "utf-8" },
                                //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                            }
                        }
                    };

                    byte[] pdf = _converter.Convert(doc);
                    //using (FileStream stream = new FileStream(DateTime.UtcNow.Ticks.ToString() + ".pdf", FileMode.Create))
                    //{
                    //    stream.Write(pdf, 0, pdf.Length);
                    //}
                    return new FileContentResult(pdf, "application/pdf");

                    //return new FileContentResult(pdf, "application/pdf");

                }
            }
            catch (Exception ex)
            {
                if (!_DEBUG)
                {
                    //return new JsonResult(new
                    //{
                    //    Status = 0
                    //});
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                else
                {
                    //return new JsonResult(new
                    //{
                    //    Status = 0,
                    //    Exception = ex.Message
                    //});
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
                
            }
        }

        [HttpPost("generatexls/{jsonfile}")]
        public async Task<IActionResult> GenerateXLS(string jsonfile, [FromBody] JsonElement data)
        {
            try
            {
                string _jsonfile = Path.Combine(Constants._JSONPATH, "DB", jsonfile + ".json");

                JObject obj;
                if (!_DEBUG)
                {
                    if (!_cache.TryGetValue(jsonfile, out obj))
                    {
                        obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(this.configuration.GetValue<int>("AppSettings:CacheDays")));
                        _cache.Set(jsonfile, obj, cacheEntryOptions);
                    }
                }
                else
                {
                    obj = (JObject)JsonConvert.DeserializeObject(System.IO.File.ReadAllText(_jsonfile));
                }

                var _data = (JObject)JsonConvert.DeserializeObject(data.ToString());

                using (var connection = new SqlConnection(this.configuration.GetValue<string>("AppSettings:ConnString")))
                {
                    SQLBuilderObject mainQuery = this.BuildSQL(obj, _data);
                    dynamic res;
                    res = (IEnumerable<dynamic>)connection.Query(mainQuery.SQL, mainQuery.Params);

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var stream = new MemoryStream();
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Report");

                        //add all the content from the DataTable, starting at cell A1
                        worksheet.Cells["A1"].LoadFromCollection(res);
                        worksheet.Cells.AutoFitColumns();
                        //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        excelPackage.SaveAs(stream);
                    }

                    stream.Position = 0;
                    string excelName = $"report-{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.xlsx";

                    //return File(stream, "application/octet-stream", excelName);  
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
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


        private SQLBuilderObject BuildSQL(dynamic obj, JObject data, string templateParam = "template")
        {
            var builder = new SqlBuilder();
            var template = obj[templateParam].ToString();

            foreach (var column in obj["columns"])
            {
                builder.Select(column["column"].ToString());
            }

            DynamicParameters parameters = new DynamicParameters();
            foreach (var param in obj["params"])
            {
                bool from_claim = false;
                if(Convert.ToBoolean((string)param["from_claim"]) == true)
                {
                    from_claim = true;
                }
                if (!data.ContainsKey(param["value"].ToString()) && Convert.ToBoolean(param["optional"]) == true && !from_claim) continue;

                var selected_role_json = User.Claims.FirstOrDefault(c => c.Type == "selected_role").Value;
                ExpandoObjectConverter converter = new ExpandoObjectConverter();
                var claims = (JObject)JsonConvert.DeserializeObject(selected_role_json.ToString());

                //if (User.Claims.FirstOrDefault(c => c.Type == (string)param["value"]) == null && Convert.ToBoolean(param["optional"]) == true && from_claim) continue;

                if(from_claim && !claims.ContainsKey((string)param["value"]) && Convert.ToBoolean(param["optional"]) == true) continue;
                if(from_claim && claims.ContainsKey((string)param["value"]) && claims[(string)param["value"]].ToString() == "" && Convert.ToBoolean(param["optional"]) == true) continue;

                DbType dbType = DbType.Object;
                if (param["type"].ToString() == "int")
                {
                    dbType = DbType.Int64;
                }
                else if (param["type"].ToString() == "string")
                {
                    dbType = DbType.String;
                }
                else if (param["type"].ToString() == "datetime")
                {
                    dbType = DbType.DateTime;
                }
                else if (param["type"].ToString() == "guid")
                {
                    dbType = DbType.Guid;
                }
                else if (param["type"].ToString() == "float")
                {
                    dbType = DbType.Double;
                }

                var instId = claims["InstitutionID"];
                var sysRole = claims["SysRoleID"];

                int[] monRuoRoles = { 1, 2, 3, 4, 9, 10, 11, 12, 13, 15, 16, 17, 19 };
                int[] directorRoles = { 0, 14, 20 };

                if(!monRuoRoles.Contains((int)sysRole) && param["value"].ToString() == "instid")
                {
                    if(!directorRoles.Contains((int)sysRole) || data[param["value"].ToString()].ToString() != instId.ToString())
                    {
                        throw new System.InvalidOperationException(string.Format("Operation is forbidden"));
                    }
                    else
                    {
                        parameters.Add(param["param"].ToString(), instId.ToString(), dbType, ParameterDirection.Input);
                    }
                }
                else if (Convert.ToBoolean(param["static"]) == false)
                {
                    if(!from_claim)
                    {
                        if (param["type"].ToString() == "string" && param["operator"].ToString().ToLower() == "like")
                        {
                            parameters.Add(param["param"].ToString(), "%" + data[param["value"].ToString()].ToString() + "%", dbType, ParameterDirection.Input);
                        } 
                        else
                        {
                            parameters.Add(param["param"].ToString(), data[param["value"].ToString()].ToString(), dbType, ParameterDirection.Input);
                        }
                    } else
                    {
                        //parameters.Add(param["param"].ToString(), User.Claims.FirstOrDefault(c => c.Type == (string)param["value"]).Value, dbType, ParameterDirection.Input);
                        parameters.Add(param["param"].ToString(), claims[(string)param["value"]].ToString(), dbType, ParameterDirection.Input);
                    }
                }
                else
                {
                    parameters.Add(param["param"].ToString(), param["value"].ToString(), dbType, ParameterDirection.Input);
                }

                if (param["logical_operator"].ToString() == "and")
                {
                    if(param["operator"].ToString().ToLower() == "in")
                    {
                        JArray arr = new JArray();
                        if (Convert.ToBoolean(param["static"]) == true)
                        {
                            arr = JArray.Parse(param["value"].ToString());
                        } 
                        else
                        {
                            arr = JArray.Parse(data[param["value"].ToString()].ToString());
                        }

                        List<int> lst = new List<int>();
                        foreach (var item in arr)
                        {
                            lst.Add(Convert.ToInt32(item.ToString())); // safety precaution
                        }
                        var _param = String.Join(",", lst.ToArray());
                        builder.Where(String.Format("{0} {1} ({2})", param["column"], param["operator"], _param));
                    }
                    else
                    {
                        builder.Where(String.Format("{0} {1} {2}", param["column"], param["operator"], param["param"]));
                    }
                }
                else if (param["logical_operator"].ToString() == "or")
                {
                    if (param["operator"].ToString().ToLower() == "in")
                    {
                        JArray arr = new JArray();
                        if (Convert.ToBoolean(param["static"]) == true)
                        {
                            arr = JArray.Parse(param["value"].ToString());
                        }
                        else
                        {
                            arr = JArray.Parse(data[param["value"].ToString()].ToString());
                        }

                        List<int> lst = new List<int>();
                        foreach (var item in arr)
                        {
                            lst.Add(Convert.ToInt32(item.ToString())); // safety precaution
                        }
                        var _param = String.Join(",", lst.ToArray());
                        builder.OrWhere(String.Format("{0} {1} ({2})", param["column"], param["operator"], _param));
                    }
                    else
                    {
                        builder.OrWhere(String.Format("{0} {1} {2}", param["column"], param["operator"], param["param"]));
                    }
                }

            }

            builder.AddParameters(parameters);

            foreach (var order in obj["order"])
            {
                builder.OrderBy(String.Format("{0} {1}", order["column"].ToString(), order["type"].ToString()));
            }

            SQLBuilderObject res = new SQLBuilderObject();
            res.SQL = builder.AddTemplate(template).RawSql;
            res.Params = parameters;
            return res;
        }
    }

    public class SQLBuilderObject
    {
        public string SQL { get; set; }
        public DynamicParameters Params { get; set; }
    }
}
