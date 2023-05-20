using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.ComponentModel.DataAnnotations;

namespace API_BUSESCONTROL.Models.ValidationsModels
{
    public class ValidarAnoFab : ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            return ValidAnoFab(value.ToString());
        }
        public bool ValidAnoFab(string value)
        {
            int ano = int.Parse(value);
            if (ano > DateTime.Now.Year || ano < 1975)
            {
                return false;
            }
            return true;
        }
    }
}
