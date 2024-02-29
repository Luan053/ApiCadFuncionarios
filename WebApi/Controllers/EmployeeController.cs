using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.ViewModel;
using WebApi.Domain.DTOs;
using WebApi.Domain.Model;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/v1/employee")]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeRepository employeeRepository, ILogger<EmployeeController> logger, IMapper mapper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //[Authorize]
        [HttpPost]
        public IActionResult Add([FromForm] EmployeeViewModel employeeView)
        {
            var filePath = Path.Combine("Storage", employeeView.Photo.FileName);
            using Stream fileStream = new FileStream(filePath, FileMode.Create);
            employeeView.Photo.CopyTo(fileStream);

            var employee = new Employee(employeeView.Name, employeeView.Age, filePath, employeeView.Role);
            _employeeRepository.Add(employee);

            return Ok();
        }

        //[Authorize]
        [HttpPost]
        [Route("{id}/download")]
        public IActionResult DownloadPhoto (int id)
        {
            var employee = _employeeRepository.Get(id);
            var dataBytes = System.IO.File.ReadAllBytes(employee.photo);

            return File(dataBytes, "image/png");
        }

        //[Authorize]
        [HttpGet]
        public IActionResult Get(int pageNumber, int pageQuantity) 
        {
            //_logger.Log(LogLevel.Error, "Ocorreu um Erro");

            //throw new Exception("Erro Teste");

            var employees = _employeeRepository.Get(pageNumber, pageQuantity);

            //_logger.LogInformation("Passou pelo Get!");

            return Ok(employees);
        }

        //[Authorize]
        [HttpGet]
        [Route("{id}")]
        public IActionResult Search(int id)
        {
            var employees = _employeeRepository.Get(id);
            var employeesDTOS = _mapper.Map<EmployeeDTO>(employees);
            return Ok(employeesDTOS);
        }


        //[Authorize]
        [HttpDelete]
        [Route("{id}/delete")]
        public IActionResult DeleteEmployee(int id)
        {
            // Busca o funcionário pelo ID
            var employee = _employeeRepository.Get(id);

            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                _employeeRepository.Delete(id);

                string imagePath = Path.Combine("Storage", employee.photo);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir o funcionário: {ex.Message}");
            }
        }

    }
}