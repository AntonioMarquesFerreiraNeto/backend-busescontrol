namespace API_BUSESCONTROL.Helpers {
    public interface IEmail {
        public bool EnviarEmail(string email, string tema, string msg);
    }
}
