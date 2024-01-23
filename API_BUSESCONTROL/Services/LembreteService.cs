using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Repository.Interfaces;
using API_BUSESCONTROL.Services.Interfaces;
using static API_BUSESCONTROL.Models.Enums.LembreteEnum;

namespace API_BUSESCONTROL.Services {
    public class LembreteService : ILembreteService {

        private readonly ILembreteRepository _lembreteRepository;

        public LembreteService(ILembreteRepository lembreteRepository) {
            _lembreteRepository = lembreteRepository;
        }

        public void PostNotiFuncionarioEnabled(int id) {
            Lembrete lembrete = new Lembrete {
                NivelAcesso = NivelAcesso.Administradores
            };
            lembrete.Conteudo = $"Identificamos que um funcionário com identificador n.° {id} que estava inativado foi ativado recentemente. Caso deseja ativar os acessos ao sistema deste funcionário, é necessário ativá-lo no papel de usuário no módulo de funcionários.";
            _lembreteRepository.CreateLembreteNotification(lembrete);
        }

        public void PostNotiNewContrato(int contratoId) {
            Lembrete lembrete = new Lembrete {
                NivelAcesso = NivelAcesso.Administradores
            };
            lembrete.Conteudo = $"Identificamos que um novo contrato com o identificador n.º {contratoId} foi adicionado, e está pendente de análise de aprovação o mais rápido possível.";
            _lembreteRepository.CreateLembreteNotification(lembrete);
        }

        public void PostNotiProcessRescisao(string nameCliente, int? clienteId, int? contratoId) {
            Lembrete lembrete = new Lembrete {
                NivelAcesso = NivelAcesso.Administradores
            };
            lembrete.Conteudo = $"Olá, tudo bem? O cliente {nameCliente}, com o identificador n.º {clienteId}, começou o processo de rescisão do contrato n.º {contratoId}. A confirmação está pendente por 24 horas. Se não for confirmada, será encerrada.";
            _lembreteRepository.CreateLembreteNotification(lembrete);
        }

        public void PostNotiContratoEncerrado(int contratoId) {
            Lembrete lembrete = new Lembrete {
                NivelAcesso = NivelAcesso.Todos
            };
            lembrete.Conteudo = $"Olá, tudo bem? Indentificamos que o contrato n.º {contratoId} foi encerrado hoje em conformidade com os critérios estabelecidos.";
            _lembreteRepository.CreateLembreteNotification(lembrete);
        }
    }
}
