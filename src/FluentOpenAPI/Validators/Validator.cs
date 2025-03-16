using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentOpenAPI.Validators;
public abstract class Validator
{
    public abstract Func<object, bool> GetCondition();
    public abstract string GetErrorMessage(string propertyName);
}
