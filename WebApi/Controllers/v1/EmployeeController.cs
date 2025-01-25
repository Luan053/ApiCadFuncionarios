using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Application.ViewModel;
using WebApi.Domain.DTOs;
using WebApi.Domain.Model.EmployeeAggregate;

namespace WebApi.Controllers.v1
{
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/employee")]
    [Authorize]
    [Produces("application/json")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;
        private readonly string _storageBasePath;

        public EmployeeController(IEmployeeRepository employeeRepository, 
            ILogger<EmployeeController> logger, 
            IMapper mapper,
            IConfiguration configuration)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _storageBasePath = configuration["Storage:BasePath"] ?? "Storage";
            
            if (!Directory.Exists(_storageBasePath))
                Directory.CreateDirectory(_storageBasePath);
        }

        /// <summary>
        /// Adiciona um novo funcionário
        /// </summary>
        /// <param name="employeeView">Dados do funcionário</param>
        /// <returns>Funcionário criado</returns>
        /// <response code="201">Funcionário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromForm] EmployeeViewModel employeeView)
        {
            try
            {
                if (employeeView.Photo == null || employeeView.Photo.Length == 0)
                    return BadRequest("Foto é obrigatória");

                if (employeeView.Photo.Length > 5 * 1024 * 1024) // 5MB
                    return BadRequest("Arquivo muito grande. Máximo permitido: 5MB");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(employeeView.Photo.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Formato de arquivo não suportado. Use: .jpg, .jpeg ou .png");

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_storageBasePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employeeView.Photo.CopyToAsync(stream);
                }

                var employee = new Employee(employeeView.Name, employeeView.Age, fileName, employeeView.Role);
                _employeeRepository.Add(employee);

                var employeeDto = _mapper.Map<EmployeeDTO>(employee);
                _logger.LogInformation("Funcionário criado com sucesso. ID: {EmployeeId}", employee.Id);

                return CreatedAtAction(nameof(Search), new { id = employee.Id }, employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar funcionário");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Baixa a foto do funcionário
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <returns>Arquivo da foto</returns>
        [HttpGet]
        [Route("{id}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DownloadPhoto(int id)
        {
            try
            {
                var employee = _employeeRepository.Get(id);
                if (employee == null)
                    return NotFound("Funcionário não encontrado");

                var filePath = Path.Combine(_storageBasePath, employee.photo);
                if (!System.IO.File.Exists(filePath))
                    return NotFound("Foto não encontrada");

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var extension = Path.GetExtension(employee.photo).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return File(fileBytes, contentType, employee.photo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao baixar foto do funcionário. ID: {EmployeeId}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Lista funcionários com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (começa em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Lista de funcionários</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDTO>), StatusCodes.Status200OK)]
        public IActionResult Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var (employees, totalCount) = _employeeRepository.Get(pageNumber, pageSize);
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());
                Response.Headers.Add("X-Total-Pages", Math.Ceiling((double)totalCount / pageSize).ToString());

                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar funcionários");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Busca um funcionário pelo ID
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <returns>Dados do funcionário</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Search(int id)
        {
            try
            {
                var employee = _employeeRepository.Get(id);
                if (employee == null)
                    return NotFound("Funcionário não encontrado");

                var employeeDto = _mapper.Map<EmployeeDTO>(employee);
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionário. ID: {EmployeeId}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Atualiza os dados de um funcionário
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <param name="employeeView">Novos dados do funcionário</param>
        /// <returns>Funcionário atualizado</returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] EmployeeViewModel employeeView)
        {
            try
            {
                var employee = _employeeRepository.Get(id);
                if (employee == null)
                    return NotFound("Funcionário não encontrado");

                string fileName = employee.photo;
                if (employeeView.Photo != null && employeeView.Photo.Length > 0)
                {
                    if (employeeView.Photo.Length > 5 * 1024 * 1024)
                        return BadRequest("Arquivo muito grande. Máximo permitido: 5MB");

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(employeeView.Photo.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                        return BadRequest("Formato de arquivo não suportado. Use: .jpg, .jpeg ou .png");

                    // Delete old photo
                    var oldFilePath = Path.Combine(_storageBasePath, employee.photo);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    // Save new photo
                    fileName = $"{Guid.NewGuid()}{extension}";
                    var newFilePath = Path.Combine(_storageBasePath, fileName);
                    using (var stream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await employeeView.Photo.CopyToAsync(stream);
                    }
                }

                employee.Update(employeeView.Name, employeeView.Age, fileName, employeeView.Role);
                _employeeRepository.Update(employee);

                var employeeDto = _mapper.Map<EmployeeDTO>(employee);
                _logger.LogInformation("Funcionário atualizado com sucesso. ID: {EmployeeId}", id);

                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar funcionário. ID: {EmployeeId}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Remove um funcionário
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            try
            {
                var employee = _employeeRepository.Get(id);
                if (employee == null)
                    return NotFound("Funcionário não encontrado");

                _employeeRepository.Delete(id);

                var imagePath = Path.Combine(_storageBasePath, employee.photo);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);

                _logger.LogInformation("Funcionário removido com sucesso. ID: {EmployeeId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover funcionário. ID: {EmployeeId}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }
    }
}