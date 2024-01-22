using API_BUSESCONTROL.Models;

namespace API_BUSESCONTROL.Repository.Interfaces {
    public interface ILembreteRepository {
        public void CreateLembreteMensagem(Lembrete lembrete);
        public List<Lembrete> GetAllLembreteMensagens(int usuarioId, int roleNumber);
        public List<Lembrete> GetAllLembreteNotificacoes(int usuarioId, int roleNumber);
        public int GetCountLembreteMensagens(int usuarioId, int roleNumber);
        public int GetCountLembreteNotificacoes(int usuarioId, int roleNumber);
        public void CreateLembreteNotification(Lembrete lembrete);
    }
}
