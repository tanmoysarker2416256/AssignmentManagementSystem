using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string Token, string FullName, string Email, IList<string> Roles);
