using Kontrax.Regix.Core;
using Kontrax.Regix.Core.RegixModels.AZ.JobSeekerContracts;
using Kontrax.Regix.Core.RegixModels.AZ.JobSeekerStatus;
using Kontrax.Regix.Core.RegixModels.GRAO;
using Kontrax.Regix.Core.RegixModels.MoI;
using Kontrax.Regix.Core.RegixModels.NRA;
using Kontrax.Regix.Core.RegixModels.NSSI;
using Kontrax.RegiX.Core.RegiXModels.AV;
using Kontrax.RegiX.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.Models.Certificate;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using Org.BouncyCastle.Asn1.X509.SigI;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ZXing.OneD;

namespace MON.API.Controllers
{
    public class RegixController : BaseApiController
    {
        private readonly RegixConfig _config;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUserInfo _userInfo;

        public RegixController(ILogger<RegixController> logger, IOptionsSnapshot<RegixConfig> config, INomenclatureService nomenclatureService, IUserInfo userInfo
            )
        {
            _logger = logger;
            _config = config.Value;
            _nomenclatureService = nomenclatureService;
            _userInfo = userInfo;
        }

        /// <summary>
        /// Списък на трудови договори регистрирани за лицето в НАП
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<EmploymentContractsResponse>> GetEmploymentContracts(string egn)
        {
            _logger.LogInformation($"Извикване на regix NRA/EmploymentContracts за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new NRAService(_config);
            return await service.GetEmploymentContractsAsync(egn);
        }

        /// <summary>
        /// Състояние на фирма
        /// </summary>
        /// <param name="eik">ЕИК</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Kontrax.Regix.Core.RegixModels.AV.StateOfPlay.StateOfPlay>> GetStateOfPlay(string eik)
        {
            _logger.LogInformation($"Извикване на regix AV/StateOfPlay за идентификатор {eik} от {_userInfo.SysUserID}");
            using var service = new AVService(_config);
            return await service.GetStateOfPlayAsync(eik);
        }

        /// <summary>
        /// Актуално състояние
        /// </summary>
        /// <param name="eik">ЕИК</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Kontrax.Regix.Core.RegixModels.AV.ActualResponseType.ActualStateResponse>> GetActualState(string eik)
        {
            _logger.LogInformation($"Извикване на regix AV/ActualState за идентификатор {eik} от {_userInfo.SysUserID}");
            using var service = new AVService(_config);
            return await service.GetActualStateAsync(eik);
        }

        [HttpGet]
        public async Task<ActionResult<CompanyDetailsModel>> GetCompanyDetails(string eik)
        {
            _logger.LogInformation($"Извикване на regix AV за идентификатор {eik} от {_userInfo.SysUserID}");
            using var service = new AVService(_config);

            var actualState = await service.GetActualStateAsync(eik);
            if (actualState != null)
            {
                return CompanyDetailsModel.From(actualState);
            }

            var stateOfPlay = await service.GetStateOfPlayAsync(eik);

            return CompanyDetailsModel.From(stateOfPlay);

        }


        /// <summary>
        /// Справка за валидност на физическо лице
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ValidPersonResponse>> GetValidPerson(string egn)
        {
            _logger.LogInformation($"Извикване на regix GRAO/ValidPerson за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new GRAOService(_config);
            return await service.GetValidPersonAsync(egn);

        }

        /// <summary>
        /// Свързани лица
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<RelationsResponse>> GetRelations(string egn)
        {
            _logger.LogInformation($"Извикване на regix GRAO/GetRelations за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new GRAOService(_config);
            var relationResponse = await service.GetRelationsAsync(egn);
            if (relationResponse != null)
            {
                relationResponse.PersonRelations = relationResponse.PersonRelations.Where(i => i.RelationCode == RelationType.Баща || i.RelationCode == RelationType.Майка || i.RelationCode == RelationType.Осиновител || i.RelationCode == RelationType.Осиновителка).ToArray();
            }
            return relationResponse;

        }

        /// <summary>
        /// Настоящ адрес
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <param name="searchDate">Дата, към която се взима адреса</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<TemporaryAddressResponse>> GetTemporaryAddress(string egn, DateTime searchDate)
        {
            _logger.LogInformation($"Извикване на regix GRAO/GetTemporaryAddress за идентификатор {egn} към дата {searchDate.ToString("dd.MM.yyyy")} от {_userInfo.SysUserID}");
            using var service = new GRAOService(_config);
            return await service.GetTemporaryAddressAsync(egn, searchDate);
        }

        /// <summary>
        /// Постоянен адрес
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <param name="searchDate">Дата, към която се взима адреса</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PermanentAddressResponse>> GetPermanentAddress(string egn, DateTime searchDate)
        {
            _logger.LogInformation($"Извикване на regix GRAO/PermanentAddress за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new GRAOService(_config);
            return await service.GetPermanentAddressAsync(egn, searchDate);
        }

        /// <summary>
        /// Данни за човека
        /// </summary>
        /// <param name="egn"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PersonDataResponse>> GetPersonData(string egn)
        {
            _logger.LogInformation($"Извикване на regix GRAO/PersonData за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new GRAOService(_config);
            var personData = await service.GetPersonSearchAsync(egn);
            personData.Nationality.NationalityId = (await _nomenclatureService.GetCountryByCode(personData.Nationality.NationalityCode))?.Id;
            personData.Nationality.NationalityId2 = (await _nomenclatureService.GetCountryByCode(personData.Nationality.NationalityCode2))?.Id;
            var birthPlace = (await _nomenclatureService.GetTownByName(personData.PlaceBirth));
            personData.PlaceBirthId = birthPlace?.Id;
            personData.PlaceBirthName = birthPlace?.Name;

            return personData;
        }

        /// <summary>
        /// Статус на лицето търсещо работа
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<JobSeekerStatusResponse>> GetJobSeekerStatus(string egn)
        {
            _logger.LogInformation($"Извикване на regix AZ/JobSeeker за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new AZService(_config);
            return await service.GetJobSeekerStatusAsync(egn);
        }

        /// <summary>
        /// Договори на лицето търсещо работа
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<JobSeekerContractsResponse>> GetJobSeekerContracts(string egn)
        {
            _logger.LogInformation($"Извикване на regix AZ/JobSeekerContracts за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new AZService(_config);
            return await service.GetJobSeekerContractsAsync(egn);
        }

        /// <summary>
        /// Упражнено право за пенсиониране
        /// </summary>
        /// <param name="egn">ЕГН</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PensionRightResponse>> GetPensionRight(string egn)
        {
            _logger.LogInformation($"Извикване на regix NSSI/PensionRights за идентификатор {egn} от {_userInfo.SysUserID}");
            using var service = new NSSIService(_config);
            return await service.GetPensionRightAsync(egn);
        }

        /// <summary>
        /// Информация за лицето с ЛНЧ
        /// </summary>
        /// <param name="lnch"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ForeignIdentityInfoResponse>> GetForeignIdentityInfo(string lnch)
        {
            _logger.LogInformation($"Извикване на regix MoI/ForeignIdentityInfo за идентификатор {lnch} от {_userInfo.SysUserID}");
            using var service = new MoIService(_config);
            //var foreignIdentityInfo = new ForeignIdentityInfoResponse()
            //{
            //    PersonNames = new Kontrax.Regix.Core.RegixModels.MoI.PersonNames()
            //    {
            //        FirstName = "Али",
            //        Surname = "Мустафа",
            //        FamilyName = "Ибрахим"
            //    },
            //    NationalityList = new Kontrax.Regix.Core.RegixModels.MoI.Nationality[]
            //    {
            //        new Kontrax.Regix.Core.RegixModels.MoI.Nationality()
            //        {
            //            NationalityCode = "AF",
            //            NationalityName = "Афганистан",
            //            NationalityNameLatin = "Afganistan"
            //        }
            //    },
            //    PermanentAddress = new AddressBG()
            //    {
            //        SettlementCode = "68134",
            //        SettlementName  = "гр.София",
            //        DistrictName = "ж.к. Изток"
            //    },
            //    TemporaryAddress = new AddressBG()
            //    {
            //        SettlementCode = "68134",
            //        SettlementName = "гр. Пловдив",
            //        DistrictName = "Тракия"
            //    },
            //    BirthPlace = new BirthPlace()
            //    {
                    
            //    },
            //    BirthDate = "2013-12-04"
            //};

            var foreignIdentityInfo = await service.GetForeignIdentityInfoAsync(lnch);
            if (foreignIdentityInfo?.NationalityList != null)
            {
                foreach (var nationality in foreignIdentityInfo.NationalityList)
                {
                    nationality.NationalityId = (await _nomenclatureService.GetCountryByCode(nationality.NationalityCode))?.Id;
                }
            }
            return foreignIdentityInfo;
        }

        [HttpGet]
        [AllowAnonymous]
        public Task<SignatureContents> Sign(string data)
        {
            X509Certificate2 cert = new X509Certificate2(_config.FileLocation, _config.Password, X509KeyStorageFlags.Exportable);
            var result = RSAWithSha256(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(data))), cert);
            return Task.FromResult(new SignatureContents()
            {
                SignedContents = Convert.ToBase64String(result),
                SignedContentsCert = Convert.ToBase64String(cert.Export(X509ContentType.Cert))
            });
        }

        private byte[] RSAWithSha256(byte[] data, X509Certificate2 signCertificate)
        {
            RSA key = (RSA)signCertificate.PrivateKey;

            //Sign the data
            byte[] sig = key.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return sig;
        }

        private byte[] SignData(byte[] data, X509Certificate2 signCertificate, bool isAttached)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data to sign is missing", nameof(data));
            }
            if (signCertificate == null)
            {
                throw new ArgumentException("Certificate is missing", nameof(signCertificate));
            }
            var signer = new CmsSigner(signCertificate)
            {
                IncludeOption = X509IncludeOption.None,
                // SHA1 OID = 1.3.14.3.2.26, SHA256 OID = 2.16.840.1.101.3.4.2.1
                DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1")
            };

            var signedCms = new SignedCms(new ContentInfo(data), !isAttached);

            signedCms.ComputeSignature(signer, false);
            return signedCms.Encode();
        }
    }
}
