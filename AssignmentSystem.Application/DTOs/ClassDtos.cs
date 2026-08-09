using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record CreateClassDto(string Name);
public record ClassDto(int Id, string Name);
