using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAndDistributionSystem.Areas.SalesAndDistribution.Controllers;
using SalesAndDistributionSystem.Common;
using SalesAndDistributionSystem.Domain.Utility;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution;
using SalesAndDistributionSystem.Services.Business.SalesAndDistribution.IManager;
using SalesAndDistributionSystem.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace SalesAndDistributionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDashboardManager _service;
        private readonly ICommonServices _comservice;

        private readonly ILogger<SalesOrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider Provider = new ServiceProvider();

        public DataController(ICommonServices comservice, IDashboardManager service, ILogger<SalesOrderController> logger, IConfiguration configuration)
        {
            _comservice = comservice;
            _service = service;
            _logger = logger;
            _configuration = configuration;
        }
        private string GetDbConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "SalesAndDistribution");
        private string GetDbSecurityConnectionString() => Provider.GetConnectionString(User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString(), "Security");

        public string GetCompany() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.CompanyId).Value.ToString();

        public string GetUnit() => User.Claims.FirstOrDefault(x => x.Type == ClaimsType.UnitId).Value.ToString();
        [HttpGet("chart-area-data")]
        public async Task<IActionResult> GetChartAreaData()
        {
            try
            {
                // Define month labels
                var monthLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                // Fetch data from service
                DataTable dt = await _service.GetChartAreaData(GetDbConnectionString());

                // Create a dictionary to store the data for the months present
                var dataDict = new Dictionary<string, double>();

                // Populate data dictionary based on DataTable
                foreach (DataRow row in dt.Rows)
                {
                    var month = row["MONTH"].ToString();
                    var amount = Convert.ToDouble(row["AMOUNT"]);
                    var monthKey = month.Substring(0, 3); // Get the first 3 letters for matching

                    if (monthLabels.Contains(monthKey))
                    {
                        dataDict[monthKey] = amount;
                    }
                }

                // Extract labels and data from the dictionary
                var labels = dataDict.Keys.ToArray();
                var data = labels.Select(label => dataDict[label]).ToArray();

                // Create the result object
                var result = new
                {
                    Labels = labels,
                    Data = data
                };

                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (implement your own logging mechanism)
                // For example, using a logger:
                // _logger.LogError(ex, "An error occurred while getting chart area data.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // Optionally include exception details
                });
            }
        }

        [HttpGet("chart-bar-data")]
        public async Task<IActionResult> GetChartBarData()
        {
            try
            {
                // Define month labels
                var monthLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                // Fetch data from service
                DataTable dt = await _service.GetChartBarData(GetDbConnectionString());

                // Create a dictionary to store the data for the months present
                var dataDict = new Dictionary<string, double>();

                // Populate data dictionary based on DataTable
                foreach (DataRow row in dt.Rows)
                {
                    var month = row["MONTH"].ToString();
                    var amount = Convert.ToDouble(row["AMOUNT"]);
                    var monthKey = month.Substring(0, 3); // Get the first 3 letters for matching

                    if (monthLabels.Contains(monthKey))
                    {
                        dataDict[monthKey] = amount;
                    }
                }

                // Extract labels and data from the dictionary
                var labels = dataDict.Keys.ToArray();
                var data = labels.Select(label => dataDict[label]).ToArray();

                // Create the result object
                var result = new
                {
                    Labels = labels,
                    Data = data
                };

                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (implement your own logging mechanism)
                // For example, using a logger:
                // _logger.LogError(ex, "An error occurred while getting chart area data.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // Optionally include exception details
                });
            }
        }
        [HttpGet("chart-pie-data")]
        public async Task<IActionResult> GetChartPieData()
        {
            try
            {
                // Define month labels
                var monthLabels = new[] { "PAB", "RUP", "ADH", "CTG" };

                // Fetch data from service
                DataTable dt = await _service.GetChartPieData(GetDbConnectionString());

                // Create a dictionary to store the data for the months present
                var dataDict = new Dictionary<string, double>();

                // Populate data dictionary based on DataTable
                foreach (DataRow row in dt.Rows)
                {
                    var month = row["MONTH"].ToString();
                    var amount = Convert.ToDouble(row["AMOUNT"]);
                    if (monthLabels.Contains(month))
                    {
                        dataDict[month] = amount;
                    }
                }

                // Extract labels and data from the dictionary
                var labels = dataDict.Keys.ToArray();
                var data = labels.Select(label => dataDict[label]).ToArray();

                // Create the result object
                var result = new
                {
                    Labels = labels,
                    Data = data
                };

                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (implement your own logging mechanism)
                // For example, using a logger:
                // _logger.LogError(ex, "An error occurred while getting chart area data.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // Optionally include exception details
                });
            }
        }

        [HttpGet("chart-dash-data")]
        public async Task<IActionResult> GetDashBoardData()
        {
            try
            {
                var result = await _service.DashBoardData(GetDbConnectionString());
                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (implement your own logging mechanism)
                // For example, using a logger:
                // _logger.LogError(ex, "An error occurred while getting chart area data.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // Optionally include exception details
                });
            }
        }
        [HttpGet("chart-sbu-data")]
        public async Task<IActionResult> GetSbuData()
        {
            try
            {
                var result = await _service.GetSbuData(GetDbConnectionString());
                // Return the result
                return Ok(result);

                //var labels = new List<string>
                //{
                //   "January","February","March","April","May","June","July","August","September","October","November","December"
                //};

                //var dataset1 = new List<int> { 34, 56, 23, 78, 66, 45, 120, 120, 11, 120, 15, 23 };
                //var dataset2 = new List<int> { 23, 45, 67, 12, 89, 34, 56 , 111, 113, 11, 120, 11 };
                //var dataset3 = new List<int> { 12, 34, 23, 45, 67, 89, 90 , 120, 23, 45, 11, 93 };

                //var response = new
                //{
                //    labels = labels,
                //    data = new List<List<int>> { dataset1, dataset2, dataset3 }
                //};

                //return Ok(response);

            }
            catch (Exception ex)
            {
                // Log the exception (implement your own logging mechanism)
                // For example, using a logger:
                // _logger.LogError(ex, "An error occurred while getting chart area data.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Details = ex.Message // Optionally include exception details
                });
            }
        }
    }
}
