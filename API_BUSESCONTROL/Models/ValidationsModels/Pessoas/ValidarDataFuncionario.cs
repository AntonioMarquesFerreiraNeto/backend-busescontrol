using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models.ValidationsModels.Pessoas
{
    public class ValidarDataFuncionario : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return ValidaDataFuncionario(value!.ToString());
        }

        public bool ValidaDataFuncionario(string data)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataNascimento = DateTime.Parse(data);
            long dias = (int)dataAtual.Subtract(dataNascimento).TotalDays;
            int ano = (int)dias / 365;
            if (ano < 18 || ano > 130)
                return false;
            return true;
        }
    }
}
