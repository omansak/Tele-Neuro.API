using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using PlayCore.Core.Extension;
using PlayCore.Core.LocalizationString;
using PlayCore.Core.Model;

namespace PlayCore.Core.ActionFilter.ModelStateValidator
{
    public class ModelStateValidator : IAsyncActionFilter
    {
        private readonly ModelStateValidatorStrings _strings;

        public ModelStateValidator(ModelStateValidatorStrings strings)
        {
            _strings = strings;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.ModelState.IsValid)
            {
                Exception exception = new Exception()
                    .SetResultType(nameof(ModelStateError))
                    .SetResultMessage(_strings.IsNotValid)
                    .SetResult(new ModelStateError(context.ModelState).MessagesWithKeys());
                throw exception;
            }

            await next();
        }

    }

    public class ModelStateError
    {
        private readonly ModelStateDictionary _entry;

        public ModelStateError(ModelStateDictionary entry)
        {
            _entry = entry;
        }

        public List<BaseModel> MessagesWithKeys()
        {
            List<BaseModel> list = new List<BaseModel>();
            foreach (var item in _entry)
            {
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    list.Add(new BaseModel
                    {
                        Key = item.Key,
                        Value = string.Join(null, item.Value.Errors.Select(i => i.ErrorMessage))
                    });
                }
            }

            return list;
        }
    }
}