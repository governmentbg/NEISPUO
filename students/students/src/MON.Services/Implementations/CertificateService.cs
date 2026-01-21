namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Certificate;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Certificate;
    using MON.Shared.Interfaces;
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class CertificateService : BaseService<CertificateService>, ICertificateService
    {
        public CertificateService(DbServiceDependencies<CertificateService> dependencies)
                : base(dependencies)
        {
        }

        public async Task CreateCertificateAsync(CertificateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForCertificatesManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            var certificateWithoutHeaderAndFooter = Encoding.UTF8.GetString(model.Contents).Replace("-----BEGIN CERTIFICATE-----\r\n", string.Empty).Replace("-----END CERTIFICATE-----\r\n", string.Empty);
            X509Certificate2 certificate = null;
            try
            {
                model.Contents = Encoding.UTF8.GetBytes(certificateWithoutHeaderAndFooter);
                certificate = new X509Certificate2(model.Contents);
            }
            catch
            {
                model.Contents = Convert.FromBase64String(certificateWithoutHeaderAndFooter);
                certificate = new X509Certificate2(model.Contents);
            }

            var certificateEntity = new Certificate
            {
                Name = model.Name.Replace(".cer", string.Empty),
                Contents = model.Contents,
                CertificateType = model.CertificateType,
                Issuer = certificate.Issuer,
                IsValid = model.IsValid,
                NotAfter = certificate.NotAfter,
                NotBefore = certificate.NotBefore,
                SerialNumber = certificate.SerialNumber,
                Subject = certificate.Subject,
                Thumbprint = certificate.Thumbprint,
                Description = model.Description
            };

            _context.Certificates.Add(certificateEntity);
            await SaveAsync();
        }

        public async Task<CertificateModel> GetByIdAsync(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForCertificatesRead) && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForCertificatesManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.Certificates.Where(x => x.Id == id).Select(x => new CertificateModel
            {
                Id = x.Id,
                CertificateType = x.CertificateType,
                Contents = x.Contents,
                Issuer = x.Issuer,
                IsValid = x.IsValid,
                Name = x.Name,
                NotAfter = x.NotAfter,
                NotBefore = x.NotBefore,
                SerialNumber = x.SerialNumber,
                Subject = x.Subject,
                Thumbprint = x.Thumbprint,
                Description = x.Description
            }).SingleOrDefaultAsync();
        }

        public async Task<IPagedList<CertificateModel>> GetCertificatesAsync(PagedListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForCertificatesRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var query = _context.Certificates.Select(x => new CertificateModel
            {
                Id = x.Id,
                CertificateType = x.CertificateType,
                Contents = x.Contents,
                Issuer = x.Issuer,
                IsValid = x.IsValid,
                Name = x.Name,
                NotAfter = x.NotAfter,
                NotBefore = x.NotBefore,
                SerialNumber = x.SerialNumber,
                Subject = x.Subject,
                Thumbprint = x.Thumbprint,
                Description = x.Description
            });

            int totalCount = await query.CountAsync();
            var items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task UpdateCertificateAsync(CertificateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForCertificatesManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            var entity = await _context.Certificates.SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"Entity {nameof(Certificate)} cant be null!");
            }

            entity.IsValid = model.IsValid;
            entity.Name = model.Name;
            entity.CertificateType = model.CertificateType;
            entity.Description = model.Description;

            await SaveAsync();
        }

        public async Task DeleteCertificateAsync(int id)
        {
            await _context.Certificates.DeleteByKeyAsync(id);
            await SaveAsync();
        }

        public async Task<CertificateValidationResultModel> VerifyCertificate(X509Certificate2 cert)
        {
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            var allTrustedCertificates = await _context.Certificates.Where(i => i.IsValid).Select(i => new X509Certificate2(i.Contents)).ToArrayAsync();
            chain.ChainPolicy.ExtraStore.AddRange(new X509Certificate2Collection(allTrustedCertificates));
            var isVerified = chain.Build(cert);

            StringBuilder errors = new StringBuilder();
            if (isVerified == false)
            {
                foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                    errors.AppendLine(string.Format("Chain error: {0} {1}", chainStatus.Status, chainStatus.StatusInformation));
                errors.AppendLine($"Certificate: {cert.ToString()}");
            }

            if (chain.ChainStatus.Length == 1 &&
                chain.ChainStatus.First().Status == X509ChainStatusFlags.UntrustedRoot &&
                chain.ChainPolicy.ExtraStore.Contains(chain.ChainElements[chain.ChainElements.Count - 1].Certificate))
            {
                // chain is valid, thus cert signed by root certificate 
                // and we expect that root is untrusted which the status flag tells us
                // but we check that it is a known certificate
                return new CertificateValidationResultModel()
                {
                    IsValid = true,
                    Errors = ""
                };
            }
            else
            {
                return new CertificateValidationResultModel()
                {
                    IsValid = isVerified,
                    Errors = errors.ToString()
                };
            }
        }

        public async Task<CertificateValidationResultModel> VerifyCertificateWithInstitution(X509Certificate2 cert, int? institutionId)
        {
            try
            {
                ParseResult result = DigitalSignatureParser.DecodeCert(cert);
                var institutionDetails = await _context.Institutions.FirstOrDefaultAsync(i => i.InstitutionId == institutionId);
                if (institutionDetails != null)
                {
                    if (result != null && result.Success)
                    {
                        if (!String.IsNullOrWhiteSpace(result.HolderEIK))
                        {
                            if (!result.HolderEIK.Equals(institutionDetails.Bulstat, StringComparison.OrdinalIgnoreCase))
                            {
                                return new CertificateValidationResultModel()
                                {
                                    IsValid = false,
                                    Errors = $"Данните за ЕИК {institutionDetails.Bulstat} в сертификата не отговарят на институцията {institutionId}, {cert}"
                                };
                            }
                        }
                        else
                        {
                            return new CertificateValidationResultModel()
                            {
                                IsValid = false,
                                Errors = $"Липсват данни за ЕИК в сертификата {cert}"
                            };
                        }
                    }
                    else
                    {
                        return new CertificateValidationResultModel()
                        {
                            IsValid = false,
                            Errors = $"Сертификатът не може да се декодира правилно {cert}"
                        };
                    }
                }

                return new CertificateValidationResultModel()
                {
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                return new CertificateValidationResultModel()
                {
                    IsValid = false,
                    Errors = $"Сертификатът не може да се декодира правилно {ex.Message} {cert}"
                };
            }

        }
        public async Task<CertificateValidationResultModel> VerifyXmlWithInstitution(string xml, int? institutionId)
        {
            X509Certificate2 cert = ExtractCertificate(xml);
            if (cert == null)
            {
                return new CertificateValidationResultModel()
                {
                    IsValid = false,
                    Errors = "Подпис не е намерен в данните"
                };
            }
            else
            {
                var result = await VerifyCertificateWithInstitution(cert, institutionId);
                return result;
            }
        }

        /// <summary>
        /// Проверка дали XML е подписан правилно и е валиден спрямо сертификатите регистритани в операционната система и сертификатите в локалния store
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public async Task<CertificateValidationResultModel> VerifyXml(string xml)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();

            // Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xml);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            // Load the signature node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            if (!signedXml.CheckSignature())
            {
                return new CertificateValidationResultModel()
                {
                    IsValid = false,
                    Errors = "Подписът не съответства"
                };
            }

            X509Certificate2 x509 = null;

            foreach (var keyInfo in signedXml.KeyInfo)
            {
                if (keyInfo is KeyInfoX509Data)
                {
                    x509 = ((KeyInfoX509Data)keyInfo).Certificates[0] as X509Certificate2;
                }
            }

            if (x509 == null)
            {
                return new CertificateValidationResultModel()
                {
                    IsValid = false,
                    Errors = "Невалидни данни за сертификат"
                };
            }

            var certVerifyResult = await VerifyCertificate(x509);
            return certVerifyResult;
        }

        /// <summary>
        /// Извлича сертификат от подписан xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>Сертификат или null, ако не може да го извлече</returns>
        private X509Certificate2 ExtractCertificate(string xml)
        {
            // Create a new XML document.
            XmlDocument xmlDoc = new XmlDocument();

            // Load an XML file into the XmlDocument object.
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xml);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
            // Load the signature node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            if (!signedXml.CheckSignature())
            {
                return null;
            }

            X509Certificate2 x509 = null;

            foreach (var keyInfo in signedXml.KeyInfo)
            {
                if (keyInfo is KeyInfoX509Data)
                {
                    x509 = ((KeyInfoX509Data)keyInfo).Certificates[0] as X509Certificate2;
                }
            }

            return x509;
        }
    }
}