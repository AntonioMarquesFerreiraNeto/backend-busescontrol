using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Models;
using API_BUSESCONTROL.Models.Enums;
using API_BUSESCONTROL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using static API_BUSESCONTROL.Models.Enums.LembreteEnum;

namespace API_BUSESCONTROL.Repository {

    public class LembreteRepository : ILembreteRepository {

        private readonly BancoContext _bancoContext;

        public LembreteRepository(BancoContext bancoContext) {
            _bancoContext = bancoContext;
        }

        public void CreateLembreteMensagem(Lembrete lembrete) {
            lembrete.TypeLembrete = TypeLembrete.Mensagem;
            lembrete.Data = DateTime.UtcNow.AddHours(-3);
            _bancoContext.Lembrete.Add(lembrete);
            _bancoContext.SaveChanges();
        }

        public void CreateLembreteNotification(Lembrete lembrete) {
            lembrete.TypeLembrete = TypeLembrete.Notificacao;
            lembrete.Data = DateTime.UtcNow.AddHours(-3);
            _bancoContext.Lembrete.Add(lembrete);
            _bancoContext.SaveChanges();
        }

        public List<Lembrete> GetAllLembreteMensagens(int usuarioId, int roleNumber) {
            return _bancoContext.Lembrete.Include(x => x.Remetente).Include(x => x.Funcionario).AsNoTracking().Where(x => x.TypeLembrete == TypeLembrete.Mensagem && (x.FuncionarioId == usuarioId || x.NivelAcesso == NivelAcesso.Todos || x.NivelAcesso == (NivelAcesso)roleNumber)).ToList();
        }

        public List<Lembrete> GetAllLembreteNotificacoes(int usuarioId, int roleNumber) {
            return _bancoContext.Lembrete.Where(x => x.TypeLembrete ==  TypeLembrete.Notificacao && (x.NivelAcesso == NivelAcesso.Todos || x.NivelAcesso == (NivelAcesso)roleNumber)).ToList();
        }

        public int GetCountLembreteMensagens(int usuarioId, int roleNumber) {
            return _bancoContext.Lembrete.Count(x => x.TypeLembrete == TypeLembrete.Mensagem && (x.FuncionarioId == usuarioId || x.NivelAcesso == NivelAcesso.Todos || x.NivelAcesso == (NivelAcesso)roleNumber));
        }

        public int GetCountLembreteNotificacoes(int usuarioId, int roleNumber) {
            return _bancoContext.Lembrete.Count(x => x.TypeLembrete == TypeLembrete.Notificacao && (x.NivelAcesso == NivelAcesso.Todos || x.NivelAcesso == (NivelAcesso)roleNumber));
        }
        public List<Lembrete> GetAllEnviadasByRemetenteId(int remetenteId) { 
            return _bancoContext.Lembrete.Include(x => x.Funcionario).Include(x => x.Remetente).AsNoTracking().Where(x => x.RemetenteId == remetenteId).ToList();
        }

        public void DeleteMensagemById(int id) {
            Lembrete lembrete = _bancoContext.Lembrete.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Nenhum registro encontrado!");
            DateTime dataAtual = DateTime.UtcNow.AddHours(-3);
            TimeSpan resultado = dataAtual - lembrete.Data!.Value;
            if (resultado.TotalMinutes > 60) throw new Exception("Não é possível excluir esta mensagem, pois ela foi enviada há mais de uma hora.");
            _bancoContext.Lembrete.Remove(lembrete);
            _bancoContext.SaveChanges();
        }
    }
}
