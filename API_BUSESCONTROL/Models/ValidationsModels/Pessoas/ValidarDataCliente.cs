using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace API_BUSESCONTROL.Models.ValidationsModels.Pessoas {
    public class ValidarDataCliente : ValidationAttribute {
        public override bool IsValid(object? value) {
            return ValidarDate(value!.ToString());
        }

        public bool ValidarDate(string value) {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dateCliente = DateTime.Parse(value);

            long dias = (int)dataAtual.Subtract(dateCliente).TotalDays;
            int idade = (int)dias / 365;
            if (dateCliente > dataAtual || idade > 132) {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
