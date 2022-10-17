using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.Options;

namespace s3_demo.Controllers
{
    [ApiController]
    [Route("documents")]
    public class AwsS3Controller : ControllerBase
    {
        private readonly AWSConfiguration _appConfiguration;
        private readonly IAws3Services _aws3Services;

        public AwsS3Controller(IOptions<AWSConfiguration> appConfiguration)
        {
            _appConfiguration = appConfiguration.Value;
            _aws3Services = new Aws3Services(_appConfiguration.AwsAccessKey, _appConfiguration.AwsSecretAccessKey, _appConfiguration.AwsSessionToken, _appConfiguration.Region, _appConfiguration.BucketName);
        }

        [HttpPost]
        public IActionResult UploadDocumentToS3(IFormFile file)
        {
            try
            {
                if (file is null || file.Length <= 0)
                    return BadRequest("file is required to upload");


                var result = _aws3Services.UploadFileAsync(file);

                return Created(string.Empty, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{documentName}")]
        public IActionResult GetDocumentFromS3(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                    return BadRequest("The 'documentName' parameter is required");

                var document = _aws3Services.DownloadFileAsync(documentName).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
