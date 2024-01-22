namespace API_BUSESCONTROL.Models.Enums {
    public class LembreteEnum {

        public enum TypeLembrete : int {
            Notificacao = 0,
            Mensagem = 1
        }
        public enum NivelAcesso: int {
            Individual = 0,
            Assistentes = 1,
            Administradores = 2,
            Todos = 3
        }
    }
}
