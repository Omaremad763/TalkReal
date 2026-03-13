using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;

using AutoMapper;

namespace Infra.Contracts_Imp;
public class TalkRealServices(IUnitofWork unitofWork,IMapper mapper) : ITalkRealServices
{
    public IUserService UseService =>  new UserService(unitofWork, mapper);
}
