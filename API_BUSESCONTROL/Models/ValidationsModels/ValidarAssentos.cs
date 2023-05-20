using System;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models.ValidationsModels
{
    public class ValidarAssentos : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            return ValAssentos(value.ToString());
        }

        public bool ValAssentos(string value)
        {
            int assentos = int.Parse(value.ToString());
            if (assentos < 10 || assentos > 200)
            {
                return false;
            }
            return true;
        }

    }
}
