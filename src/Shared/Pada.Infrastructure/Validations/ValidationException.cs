using System;

namespace Pada.Infrastructure.Validations
{
    public class AppValidationException : Exception
    {
        public AppValidationException(ValidationResultModel validationResultModel)
        {
            ValidationResultModel = validationResultModel;
        }

        public ValidationResultModel ValidationResultModel { get; }
    }
}