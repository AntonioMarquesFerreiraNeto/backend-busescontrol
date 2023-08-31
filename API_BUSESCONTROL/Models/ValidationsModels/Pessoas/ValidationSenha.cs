using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API_BUSESCONTROL.Models.ValidationsModels.Pessoas {
    public class ValidationSenha : ValidationAttribute {
        public override bool IsValid(object value) {
            if (value == null) {
                return true; 
            }
            var senha = value.ToString();
            var senhaForteRegex = new Regex("^(?=.*[a-z])(?=.*\\d)(?=.*[@#$%^&+=]).{12,}$");
            return senhaForteRegex.IsMatch(senha);
        }
    }
}
