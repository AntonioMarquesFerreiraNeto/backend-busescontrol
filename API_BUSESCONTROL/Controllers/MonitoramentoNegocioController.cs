using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BUSESCONTROL.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Assistente, Administrador")]
    public class MonitoramentoNegocioController : ControllerBase {
        private readonly IFinanceiroRepository _financeiroRepository;

        public MonitoramentoNegocioController(IFinanceiroRepository financeiroRepository) {
            _financeiroRepository = financeiroRepository;
        }

        [HttpHead("MonitorarNegocio")]
        public IActionResult MonitorarNegocio() {
            _financeiroRepository.TaskMonitorParcelas();
            _financeiroRepository.TaskMonitorParcelasLancamento();
            _financeiroRepository.TaskMonitorPdfRescisao();
            return NoContent();
        }
    }
}
