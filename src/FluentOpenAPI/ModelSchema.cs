using FluentOpenAPI.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentOpenAPI;
public abstract class ModelSchema
{
    public abstract void ApplyTo(FluentOpenApiProvider provider);
}
