using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorApp.Domain.Common.Contracts;
public interface IHasConcurrencyVersion
{
    long Version { get; set; }
}
