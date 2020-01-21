using AdviceFirstApi.Domain.Models.Dto;
using FluentValidation;

namespace AdviceFirstApi.Validators
{
    public class AfUserValidator : AbstractValidator<AfUserModelCreate>
    {
        public AfUserValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("Please Specify a name")
            .MaximumLength(250); // User Name must be > 250
            RuleFor(c => c.UserName).Must(CheckDomain).WithMessage("User Name need not have Domain");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckDomain(string name)
        {
            if ((name == null) ||  !name.Contains("\\"))
                return true;
            else
                return false;
        }
    }
}
    
