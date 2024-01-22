namespace API_BUSESCONTROL.Services.Interfaces {
    public interface ILembreteService {
        public void PostNotiFuncionarioEnabled(int funcionarioId);
        public void PostNotiNewContrato(int contratoId);
        public void PostNotiProcessRescisao(string nameCliente, int? clienteId, int? contratoId);
        public void PostNotiContratoEncerrado(int contratoId);
    }
}
