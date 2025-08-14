using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Repositories.Interface;

public interface ICategoryRepository : IGenericRepository<Category>
{
}
