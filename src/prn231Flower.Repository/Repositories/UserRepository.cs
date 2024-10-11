using prn231Flower.Data.Base;
using prn231Flower.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prn231Flower.Repository.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(DatabaseContext context) => _context = context;
}
