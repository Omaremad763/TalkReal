using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS;
public record RegisterDto(
    string Username,
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);