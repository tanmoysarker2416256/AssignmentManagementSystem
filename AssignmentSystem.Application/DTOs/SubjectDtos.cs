using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record CreateSubjectDto(string Name , int ClassId);
public record SubjectDto(int Id, string Name, int ClassId, string ClassName);