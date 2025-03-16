using FluentValidation;
using ModelLayer.model;

namespace BusinessLayer.Validators
{
    public class RequestModelValidator : AbstractValidator<RequestModel>
    {
        public RequestModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
        }
    }
}
